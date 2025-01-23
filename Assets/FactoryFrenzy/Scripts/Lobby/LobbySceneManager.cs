using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Unity.Netcode;
//using Unity.Services.Analytics;
using Unity.Services.Core;

public class LobbySceneManager : NetworkBehaviour
{
    public GameObject playerItemPrefab;
    public Transform contentParent;
    public Text nbPlayersText;
    public TMP_Text NameLobby;
    public TMP_Text lobbyCode;
    public TMP_Text Timer;
    [SerializeField] private PlayerDataManager PlayerDataManager;
    public Button LaunchButton;
    [SerializeField] private GameObject Lobby;

    

    public int duration = 15;
    public int timeRemaining;
    public bool isCountingDown = false;

    private static GameObject _previousLayout;
    private static GameObject _currentLayout;

    async void Start()
    {
        await UnityServices.InitializeAsync();
        //NetworkLayout.SetActive(true);
        Lobby.SetActive(true);

        string lobbyName = PlayerPrefs.GetString("Lobby Name", "Lobby");

        NameLobby.text = lobbyName;
        //string Username = PlayerPrefs.GetString("username");
        
        string LobbyCode = PlayerPrefs.GetString("Lobby Code", "Public Lobby!");
        lobbyCode.text = LobbyCode;
        //AddPlayer(Username);
        UpdatePlayerCount();
        //bool isHost = PlayerDataManager.Players.Exists(p => p.PlayerID == PlayerDataManager.CurrentPlayerID && p.IsHost);
        //LaunchButton.gameObject.SetActive(isHost);
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
        //gameObject.GetComponent<Button>().interactable = false;
        SceneManager.LoadSceneAsync("TrapTest");
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


    
    public void UpdatePlayerList()
    {
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }
        foreach (var pClient in NetworkManager.Singleton.ConnectedClientsList)
        {
            NetworkObject pObject = pClient.PlayerObject;
            if (pObject != null)
            {
                PlayerData pData = pObject.GetComponent<PlayerData>();
                if (pData != null)
                {
                    GameObject playerItem = Instantiate(playerItemPrefab, contentParent);
                    Text playerText = playerItem.GetComponentInChildren<Text>();
                    if (playerText != null)
                    {
                        playerText.text = pData.playerName.text;
                    }
                }
            }
        }
        UpdatePlayerCount();

    }

    public void UpdatePlayerCount()
    {
        // Count the number of connected clients
        int playerCount = NetworkManager.Singleton.ConnectedClientsList.Count;

        // Update the Text component with the player count
        if (nbPlayersText != null)
        {
            nbPlayersText.text = "Players in Lobby: " + playerCount.ToString();
        }
    }


}
