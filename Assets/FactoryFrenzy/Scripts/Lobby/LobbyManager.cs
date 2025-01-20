using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    public GameObject playerItemPrefab;
    public Transform contentParent;
    public Text nbPlayersText;
    public TMP_Text NameLobby;
    public TMP_Text Timer;

    private List<string> playerNames = new List<string>();

    public int duration = 15;
    public int timeRemaining;
    public bool isCountingDown = false;



    
    void Start()
    {
        playerNames.Add("Anna");
        playerNames.Add("Priscilla");
        playerNames.Add("Lola");
        UpdatePlayerList();
        string Name = PlayerPrefs.GetString("Lobby Name", "Aucune Donnée");
        NameLobby.text = Name;
    }

    public void UpdatePlayerList()
    {
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }


        foreach (string playerName in playerNames)
        {
            GameObject playerItem = Instantiate(playerItemPrefab, contentParent);
            Text playerText = playerItem.GetComponentInChildren<Text>();
            if (playerText != null)
            {
                playerText.text = playerName;
            }
        }

        UpdateNbPlayers();
    }

    //à modifier plutard pour prendre aucun paramètres ou d'autres paramètres
    public void AddPlayer(string playerName)
    {
        playerNames.Add(playerName);
        UpdatePlayerList();
    }

    //à modifier plutard pour prendre aucun paramètres ou d'autres paramètres
    public void RemovePlayer(string playerName)
    {
        playerNames.Remove(playerName);
        UpdatePlayerList();
    }

    private void UpdateNbPlayers()
    {
        if (nbPlayersText != null)
        {
            nbPlayersText.text = $"Players : {playerNames.Count}";
        }
    }

    public void StartTimer()
    {
        if(isCountingDown)
        {
            isCountingDown = true;
            timeRemaining = duration;
            Invoke("_tick",1f) ;
        }
    }


    private void _tick ()
    {
        timeRemaining--;
        Timer.text = timeRemaining.ToString();
        if(timeRemaining>0)
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

    private void OnPlayerJoined(string playerName)
    {
        bool isHost = LobbyDataManager.Instance.Players.Count == 0;
        LobbyDataManager.Instance.AddPlayer(playerName, playerName, isHost);
    }

}
