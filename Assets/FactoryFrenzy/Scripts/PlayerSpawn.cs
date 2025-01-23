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
                var networkTransform = player.GetComponent<NetworkTransform>();
                if(networkTransform != null){
                    player.transform.position = spawns[i].transform.position;
                    player.transform.rotation = spawns[i].transform.rotation;
                }
            }
        }
    }
    
    public void AssignSpawnPositions(){
       int playerIndex = 0;

       foreach(ulong clientId in NetworkManager.Singleton.ConnectedClientsIds){
        Vector3 spawnPosition = GetSpawnPosition(playerIndex);
        AssignSpawnPositionToPlayer(clientId, spawnPosition);
        playerIndex++;
        Debug.LogWarning($"Spawn assisgned for {clientId}");
       }
    }

    private Vector3 GetSpawnPosition(int index){
        //Range between startLine x
        return transform.position;
    }

    private void AssignSpawnPositionToPlayer(ulong clientId, Vector3 position){
        var clientParams = new ClientRpcParams{
            Send = new ClientRpcSendParams{
                TargetClientIds = new[] {clientId}
            }
        };
        SpawnClientRpc(position, clientParams);
    }


    [ClientRpc]
    private void SpawnClientRpc(Vector3 position, ClientRpcParams clientRpcParams = default){
        if(IsOwner){
            Debug.LogWarning("Teleporting player");
            transform.position = position;
        }
    }
}
