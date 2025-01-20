using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using System;
using System.Threading.Tasks;

namespace MainMenu
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private GameObject HomeLayout;
        [SerializeField] private GameObject SettingsLayout;
        [SerializeField] private GameObject QuitLayout;
        [SerializeField] private GameObject LobbyLayout;
        [SerializeField] private GameObject LobbyCode;
        [SerializeField] private GameObject PopUpPanel;
        [SerializeField] private TMP_Text errorText;
        [SerializeField] private TMP_Text Version_TMP;
        [SerializeField] public TMP_InputField NameInput;
        [SerializeField] public TMP_Dropdown TypeLobby;
        [SerializeField] public TMP_InputField PrivateCodeInput;
        [SerializeField] public GameObject LobbyItemPrefab;
        [SerializeField] private LobbyDataManager lobbyDataManager;
        private static GameObject _previousLayout;
        private static GameObject _currentLayout;
        public Transform contentParent;

        // Start is called before the first frame update
        async void Start()
        {
            HomeLayout.SetActive(true);
            SettingsLayout.SetActive(false);
            QuitLayout.SetActive(false);
            LobbyLayout.SetActive(false);
            LobbyCode.SetActive(false);
            PopUpPanel.SetActive(false);

            _currentLayout = HomeLayout;
            Version_TMP.text = "V." + Application.version;
            UpdateLobbiesList();
            await InitializeUnityServicesAsync();
        }

        private async Task InitializeUnityServicesAsync()
        {
            try
            {
                // Initialize Unity Services
                await UnityServices.InitializeAsync();

                // Authenticate the player
                if (!AuthenticationService.Instance.IsSignedIn)
                {
                    await AuthenticationService.Instance.SignInAnonymouslyAsync();
                    Debug.Log("Player signed in with ID: " + AuthenticationService.Instance.PlayerId);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to initialize Unity Services: {ex.Message}");
            }
        }

        public void PlayGame()
        {
            HomeLayout.SetActive(false);
            //SceneManager.LoadScene("TrapTest");
            SwitchLayout(LobbyLayout);
        }


        public void OnSettings()
        {
            SwitchLayout(SettingsLayout);
        }

        public void OnQuit()
        {
            SwitchLayout(QuitLayout);
        }

        public void confirmQuit()
        {
            Application.Quit();
        }

        public void onBack()
        {
            SwitchLayout(_previousLayout);
        }

        public async void OnCreate()
        {
            try
            {
                string lobbyName = NameInput.text;
                string lobbyType = TypeLobby.options[TypeLobby.value].text;
                bool isPrivate = lobbyType == "Private";

                // Validate inputs
                if (string.IsNullOrWhiteSpace(lobbyName))
                {
                    ShowError("Veuillez entrer un nom pour le lobby.");
                    return;
                }

                // Create lobby options
                CreateLobbyOptions options = new CreateLobbyOptions
                {
                    IsPrivate = isPrivate,
                    Data = new Dictionary<string, DataObject>
            {
                { "Type", new DataObject(DataObject.VisibilityOptions.Public, lobbyType) }
            }
                };

                // Create the lobby
                Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, 10, options);
                Debug.Log($"Lobby créé avec succès : {lobby.Name}");

                if (isPrivate)
                {
                    ShowError($"Code du lobby privé : {lobby.LobbyCode}");
                }

                // Update public lobbies list
                UpdateLobbiesList();
            }
            catch (Exception ex)
            {
                ShowError($"Erreur lors de la création du lobby : {ex.Message}");
                Debug.LogError($"Erreur lors de la création du lobby : {ex.Message}");
            }
        }


        public void OnJoinPrivate()
        {
            string code = PrivateCodeInput.text;
            JoinPrivateLobby(code);
        }

        public void OnJoin()
        {
            string lobbyName = NameInput.text;
            string lobbyType = TypeLobby.options[TypeLobby.value].text;


            if (lobbyType == "Private")
            {
                ShowCodeInput(true);
            }
            else
            {
                PlayerPrefs.SetString("Lobby Name", lobbyName);
                SceneManager.LoadSceneAsync("Lobby");
            }
        }

        private void SwitchLayout(GameObject layout)
        {
            //TO-DO : Faite en sorte de switch de scene on fonction de la derniere et de la nouvelle
            _previousLayout = _currentLayout;
            layout.SetActive(true);
            _currentLayout = layout;
            _previousLayout.SetActive(false);

        }

        public void AddLobby(string nameLobby, string typeLobby)
        {
            Lobby newLobby = new Lobby(nameLobby, typeLobby);
            //lobbies.Add(nameLobby); //newLobby or nameLobby ? it worked before tho
            UpdateLobbiesList();
        }

        public async void UpdateLobbiesList()
        {
            try
            {
                // Clear the current list
                foreach (Transform child in contentParent)
                {
                    Destroy(child.gameObject);
                }

                // Query public lobbies
                QueryResponse queryResponse = await LobbyService.Instance.QueryLobbiesAsync(new QueryLobbiesOptions
                {
                    Filters = new List<QueryFilter>
            {
                // Filter only public lobbies
                new QueryFilter(
                    field: QueryFilter.FieldOptions.AvailableSlots,
                    op: QueryFilter.OpOptions.GT,
                    value: "0"
                )
            }
                });

                // Populate the list with lobbies
                foreach (Lobby lobby in queryResponse.Results)
                {
                    GameObject lobbyItem = Instantiate(LobbyItemPrefab, contentParent);
                    TMP_Text lobbyName = lobbyItem.GetComponentInChildren<TMP_Text>();
                    if (lobbyName != null)
                    {
                        string type = lobby.Data.ContainsKey("Type") ? lobby.Data["Type"].Value : "Unknown";
                        lobbyName.text = $"{lobby.Name} ({type})";
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Erreur lors de la mise à jour de la liste des lobbies : {ex.Message}");
                ShowError("Impossible de récupérer la liste des lobbies.");
            }
        }



        public void ShowPrivateLobbyPanel(bool show)
        {
            LobbyCode.SetActive(show);
        }

        private void ShowCodeInput(bool show)
        {
            SwitchLayout(LobbyCode);
            // GameObject codeInputPanel = GameObject.Find("LobbyCode");
            //if (codeInputPanel != null)
            //{
                
                // codeInputPanel.SetActive(show);
            //}
        }

        public async void JoinPrivateLobby(string code)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(code))
                {
                    ShowError("Veuillez entrer un code de lobby.");
                    return;
                }

                // Attempt to join the lobby
                Lobby lobby = await LobbyService.Instance.JoinLobbyByCodeAsync(code);
                Debug.Log($"Rejoint le lobby avec succès : {lobby.Name}");

                // Navigate to the Lobby scene
                SceneManager.LoadScene("LobbyScene");
            }
            catch (Exception ex)
            {
                ShowError($"Erreur lors de la tentative de rejoindre le lobby : {ex.Message}");
                Debug.LogError($"Erreur lors de la tentative de rejoindre le lobby : {ex.Message}");
            }
        }


        // Show the popup with the provided error message
        public void ShowError(string message)
        {
            errorText.text = message;
            PopUpPanel.SetActive(true);
            //SwitchLayout(PopUpPanel);
        }

        // Close the popup
        public void ClosePopup()
        {
            PopUpPanel.SetActive(false);
        }

    }
}


