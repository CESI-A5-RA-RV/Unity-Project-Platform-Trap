using UnityEngine;

public class DeadZone : MonoBehaviour
{
    public Transform lastCheckpoint;
    private bool isPlayerDead = false;
    private GameObject player;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isPlayerDead)
        {
            isPlayerDead = true;
            // get player that entered the zone
            player = other.gameObject;
            player.SetActive(false);
            Invoke(nameof(RespawnPlayer), 2f);
        }
    }

    private void RespawnPlayer()
    {
        player.transform.position = lastCheckpoint.position;
        player.SetActive(true);
        isPlayerDead = false;
    }
}
