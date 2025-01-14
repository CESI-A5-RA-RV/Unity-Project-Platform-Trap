using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class TeleportToStart : MonoBehaviour
{
    public GameObject start;
    private void OnTriggerEnter(Collider other){
        if(other.gameObject.CompareTag("Player")){
            Rigidbody player = other.gameObject.GetComponent<Rigidbody>();
            GameObject cinemachineFree = GameObject.FindWithTag("MainCamera");
            CinemachineFreeLook cinemachineFreeLook = cinemachineFree.GetComponent<CinemachineFreeLook>();
            player.velocity = Vector3.zero;
            cinemachineFreeLook.m_XAxis.Value = player.transform.eulerAngles.y;
            other.transform.position = start.transform.position;
            other.transform.rotation = start.transform.rotation;
        }
    }
}
