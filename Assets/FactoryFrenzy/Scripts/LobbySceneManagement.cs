using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbySceneManagement : NetworkBehaviour
{
    [ServerRpc(RequireOwnership = false)]
    public void RequestChangeSceneServerRpc(){
        if(!IsServer) return;
        if(IsOwner){
            Debug.Log($"ServerRpc called by: {NetworkManager.Singleton.LocalClientId}");
            Debug.Log("Changing scenes");
            NetworkManager.Singleton.SceneManager.LoadScene("LobbyEmpty", LoadSceneMode.Single);
            NotifyClientsSceneChangedClientRpc();
        }
        
    }

    [ClientRpc]
    public void NotifyClientsSceneChangedClientRpc(){
        NetworkManager.Singleton.SceneManager.LoadScene("LobbyEmpty", LoadSceneMode.Single);
    }
}
