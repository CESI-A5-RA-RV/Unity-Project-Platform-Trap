using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Unity.Netcode;

public class LobbyManager : MonoBehaviour
{
    public GameObject playerItemPrefab;
    public Transform contentParent;
    public Text nbPlayersText;
    public TMP_Text NameLobby;
    public TMP_Text Timer;
    [SerializeField] private PlayerDataManager PlayerDataManager;
    public InputField NewNameInputField;
    public Button Server;
    public Button Host;
    public Button Client;
    [SerializeField] private GameObject NetworkLayout;
    [SerializeField] private GameObject Lobby;

    //private List<string> playerNames = new List<string>();

    public int duration = 15;
    public int timeRemaining;
    public bool isCountingDown = false;


    private static GameObject _previousLayout;
    private static GameObject _currentLayout;



    void Start()
    {
        NetworkLayout.SetActive(true);

        if (NewNameInputField != null)
        {
            NewNameInputField.onEndEdit.AddListener(OnNameChanged);
        }
        //playerNames.Add("Anna");
        //playerNames.Add("Priscilla");
        //playerNames.Add("Lola");
        //UpdatePlayerList();
        string Name = PlayerPrefs.GetString("Lobby Name", "Aucune Donnée");
        NameLobby.text = Name;
        OnRefresh();
        
    }
    

    //public void UpdatePlayerList()
    //{
    //foreach (Transform child in contentParent)
    //{
    //Destroy(child.gameObject);
    //}


    //foreach (string playerName in playerNames)
    //{
    //GameObject playerItem = Instantiate(playerItemPrefab, contentParent);
    //Text playerText = playerItem.GetComponentInChildren<Text>();
    //if (playerText != null)
    //{
    //playerText.text = playerName;
    //}
    //}

    //UpdateNbPlayers();
    //}

    //à modifier plutard pour prendre aucun paramètres ou d'autres paramètres
    //public void AddPlayer(string playerName)
    //{
    //playerNames.Add(playerName);
    //UpdatePlayerList();
    //}

    //à modifier plutard pour prendre aucun paramètres ou d'autres paramètres
    //public void RemovePlayer(string playerName)
    //{
    //playerNames.Remove(playerName);
    //UpdatePlayerList();
    //}

    //private void UpdateNbPlayers()
    //{
    //if (nbPlayersText != null)
    //{
    //nbPlayersText.text = $"Players : {playerNames.Count}";
    //}
    //}

    public void StartTimer()
    {
        if (isCountingDown)
        {
            isCountingDown = true;
            timeRemaining = duration;
            Invoke("_tick", 1f);
        }
    }

    public void _tick()
    {
        timeRemaining--;
        Timer.text = timeRemaining.ToString();
        if (timeRemaining > 0)
        {
            Invoke("_tick", 1f);
        }
        else
        {
            isCountingDown = false;
            SceneManager.LoadSceneAsync("TrapTest");
        }
    }

    public void OnLaunch()
    {
        isCountingDown = true;
        StartTimer();
        gameObject.GetComponent<UnityEngine.UI.Button>().interactable = false;
    }

    public void OnPlayerJoined(string playerName)
    {
        if (PlayerDataManager != null)
        {
            bool isHost = PlayerDataManager.Players.Count == 0;
            PlayerDataManager.AddPlayer(System.Guid.NewGuid().ToString(), playerName, isHost);
        }
    }

    public void OnRefresh()
    {
        if (PlayerDataManager != null)
        {
            PlayerDataManager.UpdatePlayerUI();
            Debug.Log("Player list refreshed.");
        }
        else
        {
            Debug.LogWarning("PlayerDataManager instance not found!");
        }
    }

    private void OnNameChanged(string newName)
    {
        if (!string.IsNullOrWhiteSpace(newName))
        {
            PlayerDataManager.UpdatePlayerName(PlayerDataManager.Instance.CurrentPlayerID, newName);
            OnRefresh(); // Refresh the player list UI to reflect the updated name
        }
    }

    public void StartServer()
    {
        NetworkManager.Singleton.StartServer();
        NetworkLayout.SetActive(false);
        Lobby.SetActive(true);
    }

    public void StartHost()
    {
        NetworkManager.Singleton.StartHost();
        NetworkLayout.SetActive(false);
        Lobby.SetActive(true);
    }

    public void StartClient()
    {
        NetworkManager.Singleton.StartClient();        
        NetworkLayout.SetActive(false);
        Lobby.SetActive(true);
    }

   
}
