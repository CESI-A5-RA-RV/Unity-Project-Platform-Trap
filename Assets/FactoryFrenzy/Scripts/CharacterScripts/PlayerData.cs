using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class PlayerData : NetworkBehaviour
{
    public NetworkVariable<string> Username = new NetworkVariable<string>("Guest");
    public NetworkVariable<string> playerID = new NetworkVariable<string>();

    public override void OnNetworkSpawn()
    {
        if(IsOwner){
            string playerUsername = PlayerPrefs.GetString("Username", "Guest" + Random.Range(1000, 9999));
            string localUserID = System.Guid.NewGuid().ToString();
            SetPlayerDataServerRpc(playerUsername, localUserID);
        }
        
    }


    [ServerRpc]
    private void SetPlayerDataServerRpc(string username, string uid){
        Username.Value = username;
        playerID.Value = uid;

    }

    private void OnEnable(){
        Username.OnValueChanged += OnUsernameChanged;
    }

    private void OnDisable(){
        Username.OnValueChanged -= OnUsernameChanged;
    }

    private void OnUsernameChanged(string oldName, string newName){
        Debug.Log($"Name changed: {oldName} -> {newName}");
    }
}
