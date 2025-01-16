using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{
    public TMP_Text lobbyCode;
    public async void createLobby(){
        await UnityServices.InitializeAsync();
        if(!AuthenticationService.Instance.IsSignedIn){
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
        Debug.Log("Unity Services Initialized");
        try{
            CreateLobbyOptions options = new CreateLobbyOptions();
            options.IsPrivate = true;
            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync("LobbyName", 10, options);
            Debug.Log($"Lobby created with ID: {lobby.LobbyCode}");

            PlayerPrefs.SetString("LobbyID", lobby.LobbyCode);
            PlayerPrefs.Save();

            SceneManager.LoadScene("LobbyEmpty");

        }catch(Exception e){
            Debug.LogError(e.Message);
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
            SceneManager.LoadScene("LobbyEmpty");
        }catch(LobbyServiceException e){
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
}
