using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Bumper : MonoBehaviour
{
    [SerializeField] float bounceForce;
    private Animator animator;

    private void Start(){
        animator = GetComponentInParent<Animator>();
    }

    private void OnTriggerEnter(Collider collider){
        if(collider.gameObject.CompareTag("Player")){
            Vector3 bounceDirection = transform.forward;
            bounceDirection.Normalize();

            Rigidbody player = collider.gameObject.GetComponent<Rigidbody>();

            if(player != null){
                animator.SetTrigger("ActivateBumper");
                player.AddForce(bounceDirection * bounceForce, ForceMode.Impulse);
            }
        }
    }

    private void OnTriggerStay(Collider collider){
        if(collider.gameObject.CompareTag("Player")){
            Vector3 bounceDirection = transform.forward;
            bounceDirection.Normalize();

            Rigidbody player = collider.gameObject.GetComponent<Rigidbody>();

            if(player != null){
                player.AddForce(bounceDirection * bounceForce, ForceMode.Impulse);
            }
        }
    }
}
