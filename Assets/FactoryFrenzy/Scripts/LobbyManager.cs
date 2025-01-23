using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEditor.AssetImporters;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    public TMP_Text lobbyCode;
    public TMP_Text lobbyName;
    public TMP_Text username;
    [SerializeField] public TMP_Dropdown TypeLobby;
    [SerializeField] public TMP_InputField NameInput;

    public RelayManager relayManager;
    public RelayClient relayClient;

    private Lobby lobby;

    public Transform lobbyListContent;
    public GameObject lobbyListItemPrefab;

    private void Start(){
        if(NetworkManager.Singleton != null){
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.OnServerStopped += OnHostStopped;
            NetworkManager.Singleton.OnClientDisconnectCallback += clientId =>
            {
                if (clientId == NetworkManager.Singleton.LocalClientId)
                {
                Debug.LogWarning("Client disconnected from the host.");
                }
            };
        }
    }

    private void OnDisable(){
        if(NetworkManager.Singleton != null){
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            NetworkManager.Singleton.OnServerStopped -= OnHostStopped;
        }
    }

    private void OnClientConnected(ulong clientId){
        if(clientId == NetworkManager.Singleton.LocalClientId){
            Debug.Log("Client successfully connected to the host.");
        }
        if(NetworkManager.Singleton.IsClient){
            Debug.Log($"Client ID {clientId} and local client ID {NetworkManager.Singleton.LocalClientId}" );
        }
    }


    private void OnHostStopped(bool state){
        if(NetworkManager.Singleton.IsHost){
            Debug.Log("Lobby deleted");
            LobbyService.Instance.DeleteLobbyAsync(lobby.Id);
        }
    }

    public async void createLobby(){
        await UnityServices.InitializeAsync();
        if(!AuthenticationService.Instance.IsSignedIn){
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
        Debug.Log("Unity Services Initialized");
        try{
            await relayManager.StartHostWithRelay();
            PlayerPrefs.SetString("Relay Code", relayManager.GetRelayJoinCode());
            PlayerPrefs.Save();

            CreateLobbyOptions options = new CreateLobbyOptions();

            if (TypeLobby.options[TypeLobby.value].text == "Private")
            {
                options.IsPrivate = true;
            }
            
            options.Data = new Dictionary<string, DataObject>(){
                {"relayJoinCode", new DataObject(
                    visibility: DataObject.VisibilityOptions.Member,
                    value : relayManager.GetRelayJoinCode()
                )},
            };

            lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName.text.ToString(), 10, options);
            StartCoroutine(HeartbeatLobbyCoroutine(lobby.Id, 15));

            Debug.Log($"Lobby created with code: {lobby.LobbyCode}");

            if (TypeLobby.options[TypeLobby.value].text == "Private")
            {
                PlayerPrefs.SetString("Lobby Code", lobby.LobbyCode);
            }
                        
            PlayerPrefs.SetString("Lobby ID", lobby.Id);
            PlayerPrefs.Save();
            

            NetworkManager.Singleton.SceneManager.LoadScene("Lobby", LoadSceneMode.Single);

        }catch(Exception e){
            Debug.LogError(e);
        }
    }

    private async void JoinLobby(string lobbyCode){
        await UnityServices.InitializeAsync();
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();

        }
        Debug.Log("Unity Services Initialized");
        try
        {
            lobbyCode = CleanLobbyCode(lobbyCode);
            Lobby joinedLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode);
            PlayerPrefs.SetString("Lobby ID", joinedLobby.Id);
            PlayerPrefs.Save();

            if (joinedLobby.Data.ContainsKey("relayJoinCode"))
            {
                string relayJoinCode = joinedLobby.Data["relayJoinCode"].Value;
                Debug.Log($"Found relayJoinCode: {relayJoinCode}");
                await relayClient.StartClientWithHost(relayJoinCode);

            }
            else
            {
                Debug.LogError("The key 'relayJoinCode' was not found in the lobby data.");
            }

        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e.Message);
        }
    }

   
    private string CleanLobbyCode(string lobbyCode)
    {
        // Remove zero-width space (U+200B) and any other non-printable characters
        return lobbyCode.Replace("\u200B", "").Trim();
    }

    public void OnClick(){
        Debug.Log("Join Lobby");
        Debug.Log(lobbyCode.text.ToString());
        JoinLobby(lobbyCode.text.ToString());
    }

    IEnumerator HeartbeatLobbyCoroutine(string lobbyId, float waitTimeSeconds)
    {
        var delay = new WaitForSecondsRealtime(waitTimeSeconds);

        while (true)
        {
            LobbyService.Instance.SendHeartbeatPingAsync(lobbyId);
            yield return delay;
        }
    }

    public async void FetchPublicLobbies()
    {
        try
        {
            Debug.Log("Fetching public lobbies...");

            
            await UnityServices.InitializeAsync();

            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                Debug.Log($"Signed in as: {AuthenticationService.Instance.PlayerId}");
            }

            
            QueryLobbiesOptions options = new QueryLobbiesOptions
            {
                Count = 25, 
                Filters = new List<QueryFilter>
            {
                
                new QueryFilter(
                    field: QueryFilter.FieldOptions.AvailableSlots,
                    op: QueryFilter.OpOptions.GT,
                    value: "0")
            },
                Order = new List<QueryOrder>
            {
                
                new QueryOrder(
                    asc: false,
                    field: QueryOrder.FieldOptions.Created)
            }
            };

            QueryResponse lobbyListQueryResponse = await LobbyService.Instance.QueryLobbiesAsync(options);

            foreach (Transform child in lobbyListContent)
            {
                Destroy(child.gameObject);
            }

            foreach (Lobby lobby in lobbyListQueryResponse.Results)
            {
                GameObject lobbyItem = Instantiate(lobbyListItemPrefab, lobbyListContent);

                TMP_Text lobbyNameText = lobbyItem.GetComponentInChildren<TMP_Text>();
                lobbyNameText.text = lobby.Name;

                Button joinButton = lobbyItem.GetComponentInChildren<Button>();
                joinButton.onClick.AddListener(() => JoinLobbyById(lobby));
            }

            Debug.Log("Public lobbies fetched and displayed.");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to fetch public lobbies: {e.Message}");
        }
    }

    
    private async void JoinLobbyById(Lobby lobby)
    {
        try
        {
            Debug.Log($"Joining lobby: {lobby.Name} (ID: {lobby.Id})");

            await UnityServices.InitializeAsync();
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                Debug.Log($"Signed in as: {AuthenticationService.Instance.PlayerId}");
            }

            Lobby joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobby.Id);
            Debug.Log($"Successfully joined lobby: {joinedLobby.Name}");

            if (joinedLobby.Data.ContainsKey("relayJoinCode"))
            {
                string relayJoinCode = joinedLobby.Data["relayJoinCode"].Value;
                await relayClient.StartClientWithHost(relayJoinCode);
            }
            else
            {
                Debug.LogWarning("Relay join code not found in the lobby data.");
            }

            PlayerPrefs.SetString("Lobby ID", joinedLobby.Id);
            PlayerPrefs.Save();

            NetworkManager.Singleton.SceneManager.LoadScene("Lobby", LoadSceneMode.Single);
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to join lobby: {e.Message}");
        }
    }

    public void OnRefreshButtonClicked()
    {
        FetchPublicLobbies();
    }
}
