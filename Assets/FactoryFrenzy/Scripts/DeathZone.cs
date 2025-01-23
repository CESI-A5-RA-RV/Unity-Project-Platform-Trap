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
    AudioSource audioSource;
    void OnTriggerEnter(Collider other)
    {
        if (other.transform.gameObject.tag == "Player")
        {   audioSource = gameObject.GetComponent<AudioSource>();
            PlayerData playerData = other.GetComponent<PlayerData>();
            audioSource.Play();
            other.transform.position = playerData.playerCheckpoint;
        }
    }
}
