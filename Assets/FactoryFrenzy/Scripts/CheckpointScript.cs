using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointScript : MonoBehaviour
{
    public Renderer targetRenderer;
    public Color colorOnActivated = Color.green;
    public Color colorOnDeactivated = Color.red;

    Deathzone deathzone;
    KillPlayer killPlayer;

    private void Start()
    {
        deathzone = GameObject.Find("Deathzone").GetComponent<Deathzone>();
        //killPlayer = GameObject.Find("GameManager").GetComponent<KillPlayer>();
        targetRenderer.material.EnableKeyword("_EMISSION");

        targetRenderer.material.SetColor("_BaseColor", colorOnDeactivated);
        targetRenderer.material.SetColor("_EmissionColor", colorOnDeactivated);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.transform.gameObject.tag == "Player" && targetRenderer != null)
        {
            deathzone.respawnPositions = gameObject.transform.position;
            killPlayer.lastCheckpoint = gameObject.transform.position;

            targetRenderer.material.SetColor("_BaseColor", colorOnActivated);
            targetRenderer.material.SetColor("_EmissionColor", colorOnActivated);
        }
    }
}
