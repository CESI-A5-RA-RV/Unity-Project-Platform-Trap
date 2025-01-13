using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    public GameObject playerItemPrefab;
    public Transform contentParent;
    public Text nbPlayersText;
    public TMP_Text NameLobby;

    private List<string> playerNames = new List<string>();

    
    void Start()
    {
        playerNames.Add("Anna");
        playerNames.Add("Priscilla");
        playerNames.Add("Lola");
        UpdatePlayerList();
        string Name = PlayerPrefs.GetString("Lobby Name", "Aucune Donn�e");
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

    //� modifier plutard pour prendre aucun param�tres ou d'autres param�tres
    public void AddPlayer(string playerName)
    {
        playerNames.Add(playerName);
        UpdatePlayerList();
    }

    //� modifier plutard pour prendre aucun param�tres ou d'autres param�tres
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
}
