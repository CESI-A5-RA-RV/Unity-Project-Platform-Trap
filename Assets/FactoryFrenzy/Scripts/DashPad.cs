using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashPad : MonoBehaviour
{
    public PhysicMaterial noFrictionMaterial;  // Physics Material with no friction (or low friction)
    public PhysicMaterial normalMaterial; 
   [SerializeField] float directionForce;
   public float speedBoostMultiplier = 2f;  // Speed multiplier during the boost
    public float speedBoostDuration = 2f;  // Duration of the speed boost in seconds
    public float dashDuration = 0.2f;

   private void OnTriggerEnter(Collider collider){
        if(collider.gameObject.CompareTag("Player")){
            Vector3 dashDirection = transform.forward;
           

            Rigidbody player = collider.gameObject.GetComponent<Rigidbody>();

            if(player != null){
                player.velocity = Vector3.zero;
                player.AddForce(dashDirection * directionForce, ForceMode.Acceleration);
                player.velocity = dashDirection * directionForce;
                StartCoroutine(SpeedBoost(player));
                StartCoroutine(OverrideFriction(player));
            }
        }
    }

    private void OnTriggerStay(Collider collider){
        if(collider.gameObject.CompareTag("Player")){
            Vector3 dashDirection = transform.forward;
            

            Rigidbody player = collider.gameObject.GetComponent<Rigidbody>();

            if(player != null){
                player.AddForce(dashDirection * directionForce, ForceMode.Acceleration);
                player.velocity = dashDirection * directionForce;
                StartCoroutine(SpeedBoost(player));
                StartCoroutine(OverrideFriction(player));
            }
        }
    }

     private IEnumerator SpeedBoost(Rigidbody playerRigidbody)
    {
        // Save the original velocity for later use
        float originalSpeed = playerRigidbody.velocity.magnitude;

        // Increase the player's speed
        playerRigidbody.velocity *= speedBoostMultiplier;

        // Wait for the speed boost duration to expire
        yield return new WaitForSeconds(speedBoostDuration);

        // Return the player's velocity to the original speed
        playerRigidbody.velocity = playerRigidbody.velocity.normalized * originalSpeed;
    }

    private IEnumerator OverrideFriction(Rigidbody playerRigidbody)
    {
        // Temporarily reduce drag to simulate frictionless movement
        float originalDrag = playerRigidbody.drag;
        playerRigidbody.drag = 0f;  // Disable drag

        // Wait for a short duration to simulate the momentum boost
        yield return new WaitForSeconds(dashDuration);

        // Restore the original drag after the dash period
        playerRigidbody.drag = originalDrag;
    }
}
