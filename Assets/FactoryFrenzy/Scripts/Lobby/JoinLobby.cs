using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using MainMenu;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEditor.AssetImporters;

public class JoinLobby : MonoBehaviour
{

    [SerializeField] public TMP_Text NameLobby;
    [SerializeField] public TMP_Text TypeLobby;

    public RelayClient relayClient;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private async void OnJoin(Lobby lobby)
    {
        await UnityServices.InitializeAsync();
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();

        }
        Debug.Log("Unity Services Initialized");
        try
        {            
            Lobby joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobby.Id);
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
            NetworkManager.Singleton.SceneManager.LoadScene("Lobby", LoadSceneMode.Single);

        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e.Message);
        }
    }

    private void ShowCodeInput(bool show)
    {

        GameObject codeInputPanel = GameObject.Find("CodeInputPanel");
        if (codeInputPanel != null)
        {
            codeInputPanel.SetActive(show);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
