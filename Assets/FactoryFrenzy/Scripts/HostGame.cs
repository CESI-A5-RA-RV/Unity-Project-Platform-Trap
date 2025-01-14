using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using TMPro;

public class HostGame : MonoBehaviour
{
    public GameObject menu;
    [SerializeField] TMP_Text usernameText;

    void Start(){
        menu.SetActive(true);
    }
    public void StartHost(){
        if(NetworkManager.Singleton != null){
            // string username = usernameText.text;

            // if(string.IsNullOrWhiteSpace(username)){
            //     Debug.Log("Username cannot be empty");
            //     return;
            // }

            // PlayerPrefs.SetString("Username", username);
            
            menu.SetActive(false);
            NetworkManager.Singleton.StartHost();

            
        }
        else{
            Debug.LogError("NetworkManager not found");
        }
    }
    public void StartClient(){
        if(NetworkManager.Singleton != null){
            // string username = usernameText.text;

            // if(string.IsNullOrWhiteSpace(username)){
            //     Debug.Log("Username cannot be empty");
            //     return;
            // }

            // PlayerPrefs.SetString("Username", username);
            
            menu.SetActive(false);
            NetworkManager.Singleton.StartClient();

            
        }
        else{
            Debug.LogError("NetworkManager not found");
        }
    }
}



