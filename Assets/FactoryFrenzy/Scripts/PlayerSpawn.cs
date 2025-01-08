using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawn : MonoBehaviour
{

    private GameObject player;
    private GameObject startPlatform;
    void Awake(){
        player = GameObject.FindWithTag("Player");
        startPlatform = GameObject.FindWithTag("Player");
        player.transform.position = startPlatform.transform.position;
    }
}
