using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class HostGame : MonoBehaviour
{
    public GameObject menu;

    void Start(){
        menu.SetActive(true);
    }
    public void StartHost(){
        if(NetworkManager.Singleton != null){
            NetworkManager.Singleton.StartHost();
            menu.SetActive(false);
        }
        else{
            Debug.LogError("NetworkManager not found");
        }
    }
    public void StartClient(){
        if(NetworkManager.Singleton != null){
            NetworkManager.Singleton.StartClient();
            menu.SetActive(false);
        }
        else{
            Debug.LogError("NetworkManager not found");
        }
    }
}



