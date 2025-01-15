using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using TMPro;

public class HostGame : MonoBehaviour
{
    public GameObject menu;

    void Start(){
        menu.SetActive(true);
    }
    public void StartHost(){
        if(NetworkManager.Singleton != null){
            menu.SetActive(false);
            NetworkManager.Singleton.StartHost();
        }
        else{
            Debug.LogError("NetworkManager not found");
        }
    }
    public void StartClient(){
        if(NetworkManager.Singleton != null){
            menu.SetActive(false);
            NetworkManager.Singleton.StartClient();
        }
        else{
            Debug.LogError("NetworkManager not found");
        }
    }
}



