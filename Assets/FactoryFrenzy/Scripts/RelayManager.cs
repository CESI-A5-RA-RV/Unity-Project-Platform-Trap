using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Services.Core;
using UnityEngine;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using System;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using System.Collections;

public class RelayManager : MonoBehaviour
{
    private Allocation allocation;
    private string relayJoinCode;
    // Start is called before the first frame update
    public async Task<string> StartHostWithRelay(int maxConnections=10)
    {
        await UnityServices.InitializeAsync();
        if(!AuthenticationService.Instance.IsSignedIn){
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
        Debug.Log("Unity Services Initialized");

        try{
            allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections);
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(allocation, "dtls"));
            relayJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            Debug.Log($"Relay Allocation created: {allocation.AllocationId}");
            Debug.Log($"Join Code: {relayJoinCode}");

            return NetworkManager.Singleton.StartHost() ? relayJoinCode : null ;

        }catch(Exception e){
            Debug.LogError($"Error creating relay allocation: {e.Message}");
            return null;
        }
    }

    public string GetRelayJoinCode(){
        return relayJoinCode;
    }

    public IEnumerator startRelayCoroutine(){
        yield return StartHostWithRelay();
    }

    public void OnClick(){
        StartCoroutine(startRelayCoroutine());
    }
}
