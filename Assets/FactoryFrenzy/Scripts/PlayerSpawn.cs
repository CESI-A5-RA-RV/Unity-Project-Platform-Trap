using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSpawn : NetworkBehaviour
{
    GameObject[] spawns;

    private void OnEnable(){
        if(NetworkManager.Singleton != null){
            NetworkManager.Singleton.SceneManager.OnLoadComplete += OnSceneLoaded;
        }
        
    }

    private void OnDisable(){
        if(NetworkManager.Singleton != null){
            NetworkManager.Singleton.SceneManager.OnLoadComplete -= OnSceneLoaded;
        }
    }

    private void OnSceneLoaded(ulong clientId, string sceneName, LoadSceneMode mode){
        if(!IsServer) return;
        Debug.LogWarning("Scene loaded, teleporting players");
        spawns = GameObject.FindGameObjectsWithTag("SpawnTag");
        TeleportPlayersToStartingLine();
    }

    private void TeleportPlayersToStartingLine(){
        int i = 0;
        foreach(var clientId in NetworkManager.Singleton.ConnectedClientsIds){
            var player = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject;
            if(player != null && i < spawns.Length){
                TeleportPlayersClientRpc(player.NetworkObjectId, spawns[i].transform.position, spawns[i].transform.rotation);
                i++;
            }
        }
    }

    [ClientRpc]
    private void TeleportPlayersClientRpc(ulong objectId, Vector3 position, Quaternion rotation){
        var player = NetworkManager.Singleton.SpawnManager.SpawnedObjects[objectId];
        if(player != null){
            player.transform.position = position;
            player.transform.rotation = rotation;
        }               
    }
    
}
