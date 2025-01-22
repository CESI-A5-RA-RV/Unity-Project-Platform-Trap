using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Unity.Netcode;
//using Unity.Services.Analytics;
using Unity.Services.Core;

public class LobbyManager : MonoBehaviour
{
    public GameObject playerItemPrefab;
    public Transform contentParent;
    public Text nbPlayersText;
    public TMP_Text NameLobby;
    public TMP_Text Timer;
    [SerializeField] private PlayerDataManager PlayerDataManager;
    public Button LaunchButton;
    public Button Server;
    public Button Host;
    public Button Client;
    [SerializeField] private GameObject NetworkLayout;
    [SerializeField] private GameObject Lobby;

    private List<string> availableNames = new List<string>()
    {
        "PlayerAlpha",
        "PlayerBeta",
        "PlayerGamma",
        "PlayerDelta",
        "PlayerEpsilon",
        "PlayerZeta",
        "PlayerEta",
        "PlayerTheta",
        "PlayerOmega",
        "PlayerHydrius"
    };

    public int duration = 15;
    public int timeRemaining;
    public bool isCountingDown = false;

    private static GameObject _previousLayout;
    private static GameObject _currentLayout;

    async void Start()
    {
        await UnityServices.InitializeAsync();
        NetworkLayout.SetActive(true);
        Lobby.SetActive(false);

        string lobbyName = PlayerPrefs.GetString("Lobby Name", "No Name");
        NameLobby.text = lobbyName;
        OnRefresh();
        bool isHost = PlayerDataManager.Players.Exists(p => p.PlayerID == PlayerDataManager.CurrentPlayerID && p.IsHost);
        LaunchButton.gameObject.SetActive(isHost);
    }

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
            TransportToScene("TrapTest");
        }
    }

    public void OnLaunch()
    {
        isCountingDown = true;
        StartTimer();
        gameObject.GetComponent<Button>().interactable = false;
        //SceneManager.LoadSceneAsync("TrapTest");
    }

    private void TransportToScene(string sceneName)
    {
        if (NetworkManager.Singleton.IsServer)
        {
            foreach (var clientId in NetworkManager.Singleton.ConnectedClientsIds)
            {
                NetworkManager.Singleton.SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
            }
        }
    }

    public void OnPlayerJoined()
    {
        if (PlayerDataManager != null)
        {
            // Get a unique name
            string playerName = GetUniquePlayerName();

            if (playerName != null)
            {
                bool isHost = PlayerDataManager.Players.Count == 0;
                PlayerDataManager.AddPlayer(System.Guid.NewGuid().ToString(), playerName, isHost);
            }
            else
            {
                Debug.LogWarning("No unique names available!");
            }
        }
    }

    private string GetUniquePlayerName()
    {
        if (availableNames.Count == 0) return null;

        int randomIndex = Random.Range(0, availableNames.Count);
        string name = availableNames[randomIndex];
        availableNames.RemoveAt(randomIndex);
        return name;
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

    

   
}
