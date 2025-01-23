using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSpawn : NetworkBehaviour
{
    GameObject startLine;

    void Start(){
        startLine = GameObject.FindGameObjectWithTag("Start");
        AssignSpawnPositions();
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
        return startLine.transform.position;
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
