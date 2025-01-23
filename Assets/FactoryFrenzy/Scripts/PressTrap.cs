using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PressTrap : MonoBehaviour
{
    
    public Transform trapBottom;
    public Transform soundBox;

    AudioSource audioSource;
    private KillPlayer killPlayer;

    private void Start(){
        killPlayer = FindObjectOfType<KillPlayer>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Update(){
        if(transform.position.y > soundBox.transform.position.y){
            audioSource.PlayDelayed(0.25f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Rigidbody rbPLayer = other.gameObject.GetComponent<Rigidbody>();
            if(rbPLayer.transform.position.y < trapBottom.transform.position.y){
                killPlayer.Kill(other);
            }
            
        }
    }


}
