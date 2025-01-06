using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bubble : MonoBehaviour
{
    //Add animations for player when stuck and for when bubble pop
    public float stopDuration = 3f;
    private bool playerTrapped = false;

    private void OnTriggerEnter(Collider collider){
        if(collider.gameObject.CompareTag("Player") && !playerTrapped){

            Rigidbody player = collider.gameObject.GetComponent<Rigidbody>();
            ThirdPersonController playerMove = collider.GetComponent<ThirdPersonController>();
            if(playerMove != null){
                StartCoroutine(StopPlayer(player, playerMove,  collider.transform)); 
            }
        }
    }

    private IEnumerator StopPlayer(Rigidbody rbPlayer, ThirdPersonController player,  Transform playerPosition)
    {
        playerTrapped = true;

        // Stop the player's velocity
        rbPlayer.velocity *= 0f;
        player.DisableMovement();
    
        playerPosition.position = transform.position;
        
        // Wait for the bubble to pop
        yield return new WaitForSeconds(stopDuration);

        // Return the player's velocity to the original speed
        player.EnableMovement();
        playerTrapped = false;
        
    }
}
