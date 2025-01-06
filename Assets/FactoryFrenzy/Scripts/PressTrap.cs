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
            Vector3 directionToPlayer = other.transform.position - trapBottom.position;
            Debug.Log("trapBottom: "+trapBottom.position.y);
            Debug.Log("Player: "+other.transform.position.y);
            if (other.transform.position.y < trapBottom.position.y){
                isPlayerDead = true;
                // get player that entered the zone
                player = other.gameObject;
                player.SetActive(false);
                Invoke(nameof(RespawnPlayer), 2f);
            }
            
        }
    }

    private void RespawnPlayer()
    {
        player.transform.position = lastCheckpoint.position;
        player.SetActive(true);
        isPlayerDead = false;
    }
}
