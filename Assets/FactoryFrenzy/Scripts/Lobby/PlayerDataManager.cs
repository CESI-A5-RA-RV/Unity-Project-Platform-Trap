using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Services.CloudSave; 
using Unity.Services.Authentication;
using Unity.Services.Core;

public class PlayerDataManager : MonoBehaviour
{
    public static PlayerDataManager Instance;

    [System.Serializable]
    public class PlayerData
    {
        public string PlayerID;
        public string PlayerName;
        public bool IsHost;
    }

    public List<PlayerData> Players = new List<PlayerData>();

    public GameObject PlayerListScrollView;
    public GameObject PlayerUIPrefab;

    public string CurrentPlayerID;
    private string LobbyHostID;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private async void Start()
    {
        UnityServices.InitializeAsync();
        // Authenticate player
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        CurrentPlayerID = AuthenticationService.Instance.IsSignedIn
            ? AuthenticationService.Instance.PlayerId
            : System.Guid.NewGuid().ToString();

        // First player is the host
        if (Players.Count == 0)
        {
            LobbyHostID = CurrentPlayerID;
            AddPlayer(CurrentPlayerID, "HostPlayer", true);
        }
        else
        {
            AddPlayer(CurrentPlayerID, "Player", false);
        }

        await LoadPlayerDataFromCloud();
        UpdatePlayerUI();
    }

    public void AddPlayer(string playerID, string playerName, bool isHost)
    {
        if (!Players.Exists(p => p.PlayerID == playerID))
        {
            Players.Add(new PlayerData { PlayerID = playerID, PlayerName = playerName, IsHost = isHost });
            SavePlayerDataToCloud();
            UpdatePlayerUI();
        }
    }

    public void UpdatePlayerUI()
    {
        foreach (Transform child in PlayerListScrollView.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (PlayerData player in Players)
        {
            GameObject playerUI = Instantiate(PlayerUIPrefab, PlayerListScrollView.transform);

            Text playerNameText = playerUI.transform.Find("PlayerName").GetComponent<Text>();
            playerNameText.text = player.PlayerName;

            GameObject hostTag = playerUI.transform.Find("HostTag").gameObject;
            hostTag.SetActive(player.IsHost);
        }
    }

    private async void SavePlayerDataToCloud()
    {
        string playersJson = JsonUtility.ToJson(new Wrapper { Players = Players });
        await CloudSaveService.Instance.Data.ForceSaveAsync(new Dictionary<string, object>
        {
            { "PlayerData", playersJson }
        });
    }

    private async System.Threading.Tasks.Task LoadPlayerDataFromCloud()
    {
        try
        {
            var savedData = await CloudSaveService.Instance.Data.LoadAsync();
            if (savedData.ContainsKey("PlayerData"))
            {
                string playersJson = savedData["PlayerData"];
                Players = JsonUtility.FromJson<Wrapper>(playersJson).Players;
            }
        }
        catch
        {
            Debug.LogWarning("Failed to load player data from the cloud.");
        }

        UpdatePlayerUI();
    }

    [System.Serializable]
    private class Wrapper
    {
        public List<PlayerData> Players;
    }
}
