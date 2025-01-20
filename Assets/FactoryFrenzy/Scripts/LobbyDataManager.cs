using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Services.CloudSave; // For Unity Cloud Save
using Unity.Services.Authentication; // For player authentication


public class LobbyDataManager : MonoBehaviour
{
    public static LobbyDataManager Instance; // Singleton instance

    [System.Serializable]
    public class PlayerData
    {
        public string PlayerID;  // Unique ID for each player
        public string PlayerName;  // Player's chosen name
        public bool IsHost;  // Indicates if the player is the lobby host
    }

    public List<PlayerData> Players = new List<PlayerData>();
    public GameObject PlayerListScrollView; // The UI container for players
    public GameObject PlayerUIPrefab; // Prefab for a player in the list
    public InputField NameInputField; // InputField to change the player's name

    private string CurrentPlayerID; // Unique ID for this instance
    private string LobbyHostID; // ID of the lobby host

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
        // Ensure the player is authenticated
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        // Generate a unique ID for the current player
        CurrentPlayerID = AuthenticationService.Instance.PlayerId;

        // If this is the host, set the host ID
        if (Players.Count == 0) // No players in the lobby yet
        {
            LobbyHostID = CurrentPlayerID;
            AddPlayer(CurrentPlayerID, "Host Player", true); // Add this player as host
        }

        // Load lobby state from the cloud
        await LoadLobbyDataFromCloud();
        UpdateLobbyUI();
    }

    public void AddPlayer(string playerID, string playerName, bool isHost)
    {
        if (!Players.Exists(p => p.PlayerID == playerID))
        {
            Players.Add(new PlayerData { PlayerID = playerID, PlayerName = playerName, IsHost = isHost });
            SaveLobbyDataToCloud();
            UpdateLobbyUI();
        }
    }

    public void UpdatePlayerName(string playerID, string newName)
    {
        PlayerData player = Players.Find(p => p.PlayerID == playerID);
        if (player != null)
        {
            player.PlayerName = newName;
            SaveLobbyDataToCloud();
            UpdateLobbyUI();
        }
    }

    private void UpdateLobbyUI()
    {
        // Clear current UI
        foreach (Transform child in PlayerListScrollView.transform)
        {
            Destroy(child.gameObject);
        }

        // Populate the player list UI
        foreach (PlayerData player in Players)
        {
            GameObject playerUI = Instantiate(PlayerUIPrefab, PlayerListScrollView.transform);
            playerUI.transform.Find("PlayerName").GetComponent<Text>().text = player.PlayerName;

            // Show "Lobby Host" tag if applicable
            if (player.IsHost)
            {
                playerUI.transform.Find("HostTag").gameObject.SetActive(true);
            }
            else
            {
                playerUI.transform.Find("HostTag").gameObject.SetActive(false);
            }

            // Allow the current player to edit their own name
            if (player.PlayerID == CurrentPlayerID)
            {
                NameInputField = playerUI.transform.Find("NameInputField").GetComponent<InputField>();
                NameInputField.text = player.PlayerName;
                NameInputField.onEndEdit.AddListener((newName) => UpdatePlayerName(CurrentPlayerID, newName));
            }
        }
    }

    private async void SaveLobbyDataToCloud()
    {
        // Convert player list to JSON and save to Unity Cloud
        string playersJson = JsonUtility.ToJson(new { Players = this.Players });
        await CloudSaveService.Instance.Data.ForceSaveAsync(new Dictionary<string, object>
        {
            { "LobbyData", playersJson }
        });
    }

    private async System.Threading.Tasks.Task LoadLobbyDataFromCloud()
    {
        try
        {
            var savedData = await CloudSaveService.Instance.Data.LoadAsync();
            if (savedData.ContainsKey("LobbyData"))
            {
                string playersJson = savedData["LobbyData"];
                Players = JsonUtility.FromJson<Wrapper>(playersJson).Players;
            }
        }
        catch
        {
            Debug.Log("Failed to load lobby data from the cloud.");
        }
    }

    [System.Serializable]
    private class Wrapper
    {
        public List<PlayerData> Players;
    }
}
