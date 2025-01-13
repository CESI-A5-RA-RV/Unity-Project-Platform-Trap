using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSpawn : NetworkBehaviour
{
    
    void Start(){
        if(IsServer){
            SpawnAllPlayers();
        }
    }

    // void OnEnable(){
    //     SceneManager.sceneLoaded += OnSceneLoaded;
    // }
    // void OnDisable(){
    //     SceneManager.sceneLoaded -= OnSceneLoaded;
    // }

    // private void OnSceneLoaded(Scene scene, LoadSceneMode mode){
    //     if(NetworkManager.Singleton.IsHost){
    //         SpawnAllPlayers();
    //     }
    // }

    private void SpawnAllPlayers(){
        GameObject startPlatform = GameObject.FindWithTag("Start");
        
        foreach (var player in NetworkManager.Singleton.ConnectedClients){
            SpawnClient(player.Value.PlayerObject, startPlatform);
        }
    }

    private void SpawnClient(NetworkObject playerObject, GameObject startLine){
        if(playerObject.IsOwner){
            playerObject.transform.position = startLine.transform.position;
        }
    }
}
