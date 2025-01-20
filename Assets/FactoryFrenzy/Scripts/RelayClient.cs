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
using Unity.Services.Relay.Models;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class RelayClient : MonoBehaviour
{

    public async Task<bool> StartClientWithHost(string joinCode){
        await UnityServices.InitializeAsync();
        if(!AuthenticationService.Instance.IsSignedIn){
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
        Debug.Log("Unity Services Initialized");
        try{
            joinCode = CleanLobbyCode(joinCode);
            var joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode : joinCode);
            Debug.Log($"Joined relay session with allocation ID: {joinAllocation.AllocationId}");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));
            bool success = NetworkManager.Singleton.StartClient();
            Debug.Log(success);
            return !string.IsNullOrEmpty(joinCode) && success;
        }catch(Exception e){
            Debug.LogError($"Error joining relay session: {e.Message}");
            return false;
        }
        
    }

    private string CleanLobbyCode(string lobbyCode)
    {
        // Remove zero-width space (U+200B) and any other non-printable characters
        return lobbyCode.Replace("\u200B", "").Trim();
    }

}
