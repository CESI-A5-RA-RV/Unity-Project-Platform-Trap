using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{
    public TMP_Text lobbyCode;
    public TMP_Text lobbyName;

    public RelayManager relayManager;
    public RelayClient relayClient;

    private Lobby lobby;

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

    private void OnDestroy(){
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
            options.IsPrivate = true;
            options.Data = new Dictionary<string, DataObject>(){
                {"relayJoinCode", new DataObject(
                    visibility: DataObject.VisibilityOptions.Member,
                    value : relayManager.GetRelayJoinCode()
                )},
            };

            lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName.text.ToString(), 10, options);
            StartCoroutine(HeartbeatLobbyCoroutine(lobby.Id, 15));
            Debug.Log($"Lobby created with code: {lobby.LobbyCode}");

            PlayerPrefs.SetString("Lobby Code", lobby.LobbyCode);
            PlayerPrefs.Save();

            NetworkManager.Singleton.SceneManager.LoadScene("LobbyEmpty", LoadSceneMode.Single);

        }catch(Exception e){
            Debug.LogError(e);
        }
    }

    private async void JoinLobby(string lobbyCode){
        await UnityServices.InitializeAsync();
        if(!AuthenticationService.Instance.IsSignedIn){
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
        Debug.Log("Unity Services Initialized");
        try{
            lobbyCode = CleanLobbyCode(lobbyCode);
            Lobby joinedLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode);
            string relayJoinCode = joinedLobby.Data["relayJoinCode"].Value;
            Debug.Log(relayJoinCode);
            await relayClient.StartClientWithHost(relayJoinCode);
            
            NetworkManager.Singleton.SceneManager.OnSceneEvent += sceneEvent =>
            {
            Debug.Log($"[Client] Scene event: {sceneEvent.SceneName}, Type: {sceneEvent.SceneEventType}");
            };
            
        }catch(LobbyServiceException e){
            Debug.Log(e.Message);
        }
    }

    public void OnPlayerJoined(Lobby lobby){
    //     NetworkManager.Singleton.SceneManager.LoadScene("LobbyEmpty", LoadSceneMode.Additive);
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

}
