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

        private TaskCompletionSource<bool> _taskCompletionSource;

        // Start is called before the first frame update
        async void Start()
        {
            await UnityServices.InitializeAsync();
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

                // Create lobby options
                CreateLobbyOptions options = new CreateLobbyOptions
                {
                    IsPrivate = isPrivate,
                    Data = new Dictionary<string, DataObject>
            {
                { "Type", new DataObject(DataObject.VisibilityOptions.Public, lobbyType) },
                { "Password", new DataObject(DataObject.VisibilityOptions.Member, GenerateRandomCode()) }
            }
                };

                // Create the lobby in Unity's backend
                Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, 10, options);
                Debug.Log($"Lobby created successfully: {lobby.Name}, Code: {lobby.LobbyCode}");

                if (isPrivate)
                {

                    ShowError($"Private Lobby Code: {lobby.LobbyCode}");
                    await WaitForPopupToClose();
                }
                //await Task.Delay(15000);
                SceneManager.LoadSceneAsync("Lobby");

            }
            catch (Exception ex)
            {
                //ShowError($"Error creating lobby: {ex.Message}");
                Debug.LogError($"Error creating lobby: {ex.Message}");
            }
        }

        private async Task WaitForPopupToClose()
        {
            TaskCompletionSource<bool> taskCompletionSource = new TaskCompletionSource<bool>();

            _taskCompletionSource = taskCompletionSource;

            await taskCompletionSource.Task;
        }

        private string GenerateRandomCode()
        {
            return UnityEngine.Random.Range(100000, 999999).ToString();
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
            await UnityServices.InitializeAsync();
            try
            {
                // Clear the current UI
                foreach (Transform child in contentParent)
                {
                    Destroy(child.gameObject);
                }

                // Query public lobbies
                QueryResponse response = await LobbyService.Instance.QueryLobbiesAsync();
                foreach (Lobby lobby in response.Results)
                {
                    if (!lobby.Data.ContainsKey("Type") || lobby.Data["Type"].Value != "Public")
                        continue;

                    // Add to UI
                    GameObject lobbyItem = Instantiate(LobbyItemPrefab, contentParent);
                    TMP_Text lobbyName = lobbyItem.GetComponentInChildren<TMP_Text>();
                    if (lobbyName != null)
                    {
                        lobbyName.text = $"{lobby.Name} ({lobby.Data["Type"].Value})";
                    }
                }
            }
            catch (Exception ex)
            {
                //ShowError($"Error updating lobby list: {ex.Message}");
                Debug.LogError($"Error updating lobby list: {ex.Message}");
            }
        }




        public void ShowPrivateLobbyPanel()
        {
            SwitchLayout(LobbyCode);
            //LobbyCode.SetActive(show);
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
                Debug.Log($"Attempting to join private lobby with code: {code}");

                // Join the lobby using the provided code
                Lobby lobby = await LobbyService.Instance.JoinLobbyByCodeAsync(code);
                Debug.Log($"Joined private lobby successfully: {lobby.Name}");

                // Navigate to the lobby scene
                SceneManager.LoadSceneAsync("Lobby");
                //SceneManager.LoadScene("LobbyScene");
            }
            catch (Exception ex)
            {
                //ShowError($"Failed to join private lobby: {ex.Message}");
                Debug.LogError($"Error joining private lobby: {ex.Message}");
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
            _taskCompletionSource?.SetResult(true);
        }
                
    }
}


