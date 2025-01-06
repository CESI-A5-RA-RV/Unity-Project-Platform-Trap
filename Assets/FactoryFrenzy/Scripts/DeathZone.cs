/*using UnityEngine;

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
}*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deathzone : MonoBehaviour
{

    public Vector3 respawnPositions;

    void OnTriggerEnter(Collider other)
    {
        if (other.transform.gameObject.tag == "Player")
        {
            other.transform.position = respawnPositions;
        }
    }
}
