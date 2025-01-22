using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerData : NetworkBehaviour
{   public string defaultName;
    
    public TextMeshPro playerName;
    public NetworkVariable<FixedString32Bytes> networkPlayerName = new NetworkVariable<FixedString32Bytes>("Player", NetworkVariableReadPermission.Everyone ,NetworkVariableWritePermission.Owner);

    public override void OnNetworkSpawn()
    {
        if(IsOwner){
            networkPlayerName.Value = GameObject.Find("MenuManager").GetComponent<LobbyManager>().username.text;
            //networkPlayerName.Value = GameObject.Find("GameManager").GetComponent<HostGame>().username.text;
            Debug.LogWarning(networkPlayerName.Value);
        }
        playerName.text = networkPlayerName.Value.ToString();
        networkPlayerName.OnValueChanged += networkPlayerName_OnValueChanged;
    }

    void networkPlayerName_OnValueChanged(FixedString32Bytes previousValue, FixedString32Bytes newValue){
        playerName.text = newValue.Value;
    }

    
}
