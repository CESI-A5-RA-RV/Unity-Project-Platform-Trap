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

    private void Start(){
        if(NetworkManager.Singleton != null){
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        }
    }

    private void OnDestroy(){
        if(NetworkManager.Singleton != null){
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        }
    }

    private void OnClientConnected(ulong clientId){
        if(NetworkManager.Singleton.IsHost){
            NetworkManager.Singleton.SceneManager.LoadScene("LobbyEmpty", LoadSceneMode.Single);
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

            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName.text.ToString(), 10, options);
            Debug.Log($"Lobby created with code: {lobby.LobbyCode}");

            PlayerPrefs.SetString("Lobby Code", lobby.LobbyCode);
            PlayerPrefs.Save();

            if(NetworkManager.Singleton.SceneManager != null){
                NetworkManager.Singleton.SceneManager.LoadScene("LobbyEmpty", LoadSceneMode.Single);
            }
            else{
                Debug.Log("Network Manager is empty");
            }
            
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
            await relayClient.StartClientWithHost(relayJoinCode);
            
            
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

}
