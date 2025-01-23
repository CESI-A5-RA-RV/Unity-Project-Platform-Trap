using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerData : NetworkBehaviour
{   
    public TextMeshPro playerName;
    public Vector3 playerCheckpoint;
    public NetworkVariable<FixedString32Bytes> networkPlayerName = new NetworkVariable<FixedString32Bytes>("Player", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<Vector3> networkPlayerCheckpoint = new NetworkVariable<Vector3>(default, NetworkVariableReadPermission.Owner, NetworkVariableWritePermission.Owner);

    public override void OnNetworkSpawn()
    {
        if(IsOwner){
            networkPlayerName.Value = GameObject.Find("MenuManager").GetComponent<LobbyManager>().username.text;
            //networkPlayerName.Value = GameObject.Find("GameManager").GetComponent<HostGame>().username.text;
            networkPlayerCheckpoint.Value = gameObject.transform.position;
        }
        playerName.text = networkPlayerName.Value.ToString();
        networkPlayerName.OnValueChanged += networkPlayerName_OnValueChanged;
        playerCheckpoint = networkPlayerCheckpoint.Value;
        networkPlayerCheckpoint.OnValueChanged += networkPlayerCheckpoint_OnValueChanged;
    }

    void networkPlayerName_OnValueChanged(FixedString32Bytes previousValue, FixedString32Bytes newValue){
        playerName.text = newValue.Value;
    }

    void networkPlayerCheckpoint_OnValueChanged(Vector3 previousValue, Vector3 newValue){
        playerCheckpoint = newValue;
    }

    
}
