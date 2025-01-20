using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyMenu : MonoBehaviour
{
    
    void Awake()
    {
        GameObject menu = GameObject.FindGameObjectWithTag("Menu");
        DontDestroyOnLoad(menu);
    }

    
}
