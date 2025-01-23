using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : LevelElement
{
    public override void Initialize(ElementData data)
    {
        base.Initialize(data);

        // Search for the Launcher component in child objects
        MovingWall movingWall = GetComponentInChildren<MovingWall>();

        if (data.parameters != null)
        {
            foreach (var param in data.parameters)
            {
                Debug.Log("Param key: " + param.key + " ; Param value: " + param.value );
                if (param.key == "Speed")
                {
                    movingWall.speed = param.value;
                }
                else if (param.key == "Range")
                {
                    Debug.Log(movingWall.speed);
                    movingWall.range = param.value;
                }
            }
        }
        else
        {
            Debug.Log("No parameters found for this element.");
        }
    }
}
