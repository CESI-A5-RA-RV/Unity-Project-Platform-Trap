using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.Netcode;
using UnityEngine;

public class PlayerCamera : NetworkBehaviour
{
    public CinemachineFreeLook virtualCamera;
    // Start is called before the first frame update
    void Start()
    {
        if(IsOwner){
            if(virtualCamera != null){
                virtualCamera.Follow = transform;
                virtualCamera.LookAt = transform;
            }
        }
        else{
            if(virtualCamera != null){
                virtualCamera.gameObject.SetActive(false);
            }
        }
    }

}
