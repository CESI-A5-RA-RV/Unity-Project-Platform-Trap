using UnityEngine;

public class PressTrap : MonoBehaviour
{
    public Transform lastCheckpoint;
    private bool isPlayerDead = false;
    private GameObject player;
    public Transform trapBottom;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isPlayerDead)
        {
            if (other.transform.position.y < trapBottom.position.y){
                isPlayerDead = true;
                // get player that entered the zone
                player = other.gameObject;
                Invoke(nameof(RespawnPlayer), 2f);
            }
            
        }
    }

    private void RespawnPlayer()
    {
        player.transform.position = lastCheckpoint.position;
        isPlayerDead = false;
    }
}
