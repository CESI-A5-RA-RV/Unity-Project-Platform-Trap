using UnityEngine;

public class PressTrap : MonoBehaviour
{
    
    public Transform trapBottom;


    private KillPlayer killPlayer;

    private void Start(){
        killPlayer = FindObjectOfType<KillPlayer>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            killPlayer.Kill(other);
        }
    }

}
