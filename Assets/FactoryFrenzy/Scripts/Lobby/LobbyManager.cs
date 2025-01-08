using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    public GameObject playerItemPrefab;
    public Transform contentParent;
    public Text nbPlayersText;

    private List<string> playerNames = new List<string>();

    
    void Start()
    {
        playerNames.Add("Anna");
        playerNames.Add("Priscilla");
        playerNames.Add("Lola");
        UpdatePlayerList();
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
