using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CheckpointScript : NetworkBehaviour
{
    public Renderer targetRenderer;
    public Color colorOnActivated = Color.green;
    public Color colorOnDeactivated = Color.red;

    private void Start()
    {

        targetRenderer.material.EnableKeyword("_EMISSION");

        targetRenderer.material.SetColor("_BaseColor", colorOnDeactivated);
        targetRenderer.material.SetColor("_EmissionColor", colorOnDeactivated);
    }

    void OnTriggerEnter(Collider other)
    {
        if(!IsServer) return;
        if (other.transform.gameObject.tag == "Player")
        {
            var playerData = other.GetComponent<PlayerData>();
            playerData.networkPlayerCheckpoint.Value = gameObject.transform.position;

            var player = other.GetComponent<NetworkObject>();
            if(player != null){
                ulong playerId = player.OwnerClientId;
                UpdataCheckpointStateClientRpc(playerId, true);

                foreach(ulong otherPlayerId in NetworkManager.Singleton.ConnectedClientsIds){
                    if(otherPlayerId != playerId){
                        UpdataCheckpointStateClientRpc(otherPlayerId, false);
                    }
                }
            }

            
        }
    }

    [ClientRpc]
    private void UpdataCheckpointStateClientRpc(ulong playerId, bool isActive, ClientRpcParams clientRpcParams = default){
        if(NetworkManager.Singleton.LocalClientId == playerId){

            targetRenderer.material.SetColor("_BaseColor", isActive ? colorOnActivated : colorOnDeactivated);
            targetRenderer.material.SetColor("_EmissionColor", isActive ? colorOnActivated : colorOnDeactivated);
        }
    }
}
