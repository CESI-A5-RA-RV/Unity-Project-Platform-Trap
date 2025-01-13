using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject HomeLayout;
    [SerializeField] private GameObject SettingsLayout;
    [SerializeField] private GameObject QuitLayout;
    [SerializeField] private GameObject LobbyLayout;
    [SerializeField] private TMP_Text Version_TMP;
    [SerializeField] public TMP_InputField NameInput;
    [SerializeField] public TMP_Text NameLobby;
    [SerializeField] public GameObject LobbyItemPrefab;
    private  static GameObject _previousLayout;
    private static GameObject _currentLayout;
    private List<string> ListLobbies = new List<string>();
    public Transform contentParent;

    // Start is called before the first frame update
    void Start()
    {
        HomeLayout.SetActive(true);
        SettingsLayout.SetActive(false);
        QuitLayout.SetActive(false);
        LobbyLayout.SetActive(false);

        _currentLayout = HomeLayout;
        Version_TMP.text = "V."+ Application.version;
        UpdateLobbiesList();
    }

    public void PlayGame(){
        HomeLayout.SetActive(false);
        //SceneManager.LoadScene("TrapTest");
        SwitchLayout(LobbyLayout);
    }


    public void OnSettings(){
        SwitchLayout(SettingsLayout);
    }

    public void OnQuit(){
        SwitchLayout(QuitLayout);
    }

    public void confirmQuit(){
        Application.Quit();
    }

    public void onBack(){
        SwitchLayout(_previousLayout);
    }

    public void OnCreate()
    {
        string NameLobby = NameInput.text;
        AddLobby(NameLobby);
        //PlayerPrefs.SetString("Lobby Name", NameLobby);
        //SceneManager.LoadSceneAsync("Lobby");
    }

    public void OnJoin()
    {
        string Lobby = NameLobby.text;
        PlayerPrefs.SetString("Lobby Name", Lobby);
        SceneManager.LoadSceneAsync("Lobby");//doesn't load, why ?
    }

    private void SwitchLayout(GameObject layout)
    {
        //TO-DO : Faite en sorte de switch de scene on fonction de la derniere et de la nouvelle
        _previousLayout = _currentLayout;
        layout.SetActive(true);
        _currentLayout = layout;
        _previousLayout.SetActive(false);
        
    } 

    public void AddLobby(string nameLobby)
    {
        ListLobbies.Add(nameLobby);
        UpdateLobbiesList();
    }
    public void UpdateLobbiesList()
    {
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }
        foreach (string LobbyName in ListLobbies)
        {
            GameObject lobbyItem = Instantiate(LobbyItemPrefab, contentParent);
            TMP_Text lobbyName = lobbyItem.GetComponentInChildren<TMP_Text>();
            if (lobbyName != null)
            {
                lobbyName.text = LobbyName;
            }
        }
    }
}
