using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiRotator : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    private Vector3 offset = new Vector3(0, 180, 0);

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(cameraTransform);
        transform.Rotate(offset);
    }
}
