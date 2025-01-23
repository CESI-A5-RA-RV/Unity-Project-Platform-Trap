using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillPlayer : MonoBehaviour
{
    private GameObject player;
    private Rigidbody rbPlayer;

    AudioSource audioSource;
    // Start is called before the first frame update
    public void Kill(Collider other)
    {
                // get player that entered the zone
                player = other.gameObject;
                rbPlayer = player.GetComponent<Rigidbody>();
                PlayerData playerData = player.GetComponent<PlayerData>();
                Animator playerAnimator = other.GetComponent<Animator>();
                audioSource = other.GetComponent<AudioSource>();
                if(playerAnimator != null && rbPlayer != null){
                    rbPlayer.velocity = Vector3.zero;
                    rbPlayer.isKinematic = true;
                    playerAnimator.SetTrigger("Death");
                    audioSource.Play();
                }
                StartCoroutine(RespawnPlayer(playerData));
    }

    private IEnumerator RespawnPlayer(PlayerData playerData)
    {   yield return new WaitForSeconds(1.5f);
        player.transform.position = playerData.playerCheckpoint;
        rbPlayer.isKinematic = false;
    }
}
