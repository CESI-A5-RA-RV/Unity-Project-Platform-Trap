using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class RelayClient : MonoBehaviour
{

    private string code;

    public void OnInputFieldValueChanged(string inputText){
        code = inputText;
    }
    public async Task StartClientWithHost(string joinCode){
        await UnityServices.InitializeAsync();
        if(!AuthenticationService.Instance.IsSignedIn){
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
        Debug.Log("Unity Services Initialized");
        try{
            joinCode = CleanLobbyCode(joinCode);
            var joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode : joinCode);
            Debug.Log($"Joined relay session with allocation ID: {joinAllocation.AllocationId}");
            //NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));
            
        }catch(Exception e){
            Debug.LogError($"Error joining relay session: {e.Message}");
            
        }
        
    }

    private string CleanLobbyCode(string lobbyCode)
    {
        // Remove zero-width space (U+200B) and any other non-printable characters
        return lobbyCode.Replace("\u200B", "").Trim();
    }

    public IEnumerator startRelayCoroutine(){
        yield return StartClientWithHost(code);
    }

    public void OnClick(){
        if(code != null){
            Debug.Log(code);
        StartCoroutine(startRelayCoroutine());
        }
        else{
            Debug.Log("Lobby field empty");
        }
        
    }
}
