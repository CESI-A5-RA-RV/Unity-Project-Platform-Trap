using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Services.CloudSave; 
using Unity.Services.Authentication; 

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
    public InputField NameInputField;

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
        // Ensure the player is authenticated
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            Debug.Log("Signing in anonymously...");
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        // Retrieve the player's unique ID
        if (AuthenticationService.Instance.IsSignedIn)
        {
            CurrentPlayerID = AuthenticationService.Instance.PlayerId;
            Debug.Log("Player authenticated. Player ID: " + CurrentPlayerID);
        }
        else
        {
            // Fallback: Generate a unique ID for testing purposes
            CurrentPlayerID = System.Guid.NewGuid().ToString();
            Debug.LogWarning("Authentication failed. Generated fallback Player ID: " + CurrentPlayerID);
        }

        // Add the current player to the list
        if (Players.Count == 0)
        {
            LobbyHostID = CurrentPlayerID; // First player is the host
            AddPlayer(CurrentPlayerID, CurrentPlayerID, true); // Use UID as the default name
        }
        else
        {
            AddPlayer(CurrentPlayerID, CurrentPlayerID, false);
        }

        // Load existing player data from the cloud
        await LoadPlayerDataFromCloud();
        UpdatePlayerUI();

        // Set up the input field listener
        if (NameInputField != null)
        {
            NameInputField.onEndEdit.AddListener(OnNameChanged);
        }
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

    public void UpdatePlayerName(string playerID, string newName)
    {
        PlayerData player = Players.Find(p => p.PlayerID == playerID);
        if (player != null)
        {
            player.PlayerName = newName;
            SavePlayerDataToCloud();
            UpdatePlayerUI();
        }
    }

    public void UpdatePlayerUI()
    {
        Debug.Log("Updating player UI. Player count: " + Players.Count);

        // Clear current UI
        foreach (Transform child in PlayerListScrollView.transform)
        {
            Destroy(child.gameObject);
        }

        // Populate the player list UI
        foreach (PlayerData player in Players)
        {
            GameObject playerUI = Instantiate(PlayerUIPrefab, PlayerListScrollView.transform);

            // Set the player name or UID
            Text playerNameText = playerUI.transform.Find("PlayerName").GetComponent<Text>();
            playerNameText.text = string.IsNullOrWhiteSpace(player.PlayerName) ? player.PlayerID : player.PlayerName;

            // Show or hide the host tag
            GameObject hostTag = playerUI.transform.Find("HostTag").gameObject;
            hostTag.SetActive(player.IsHost);

            // If the player is the current player, position the input field
            if (player.PlayerID == CurrentPlayerID && string.IsNullOrWhiteSpace(player.PlayerName))
            {
                NameInputField.gameObject.SetActive(true);
                NameInputField.transform.SetParent(playerUI.transform, false);
                NameInputField.transform.localPosition = Vector3.zero; // Center position
                NameInputField.text = ""; // Clear input field
            }
        }
    }

    private async void SavePlayerDataToCloud()
    {
        string playersJson = JsonUtility.ToJson(new Wrapper { Players = this.Players });
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

    private void OnNameChanged(string newName)
    {
        if (!string.IsNullOrWhiteSpace(newName))
        {
            UpdatePlayerName(CurrentPlayerID, newName);
            NameInputField.gameObject.SetActive(false); // Hide input field
        }
    }

    [System.Serializable]
    private class Wrapper
    {
        public List<PlayerData> Players;
    }
}
