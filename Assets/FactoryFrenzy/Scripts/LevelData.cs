using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelData
{
    public int id;
    public string levelName;            // Name of the level
    public string backgroundName;       // Background to use
    public List<LevelElementSO> elements; // List of elements in the level
}

[System.Serializable]
public class LevelElementSO
{
    public int id; 
    public string name;                 
    public ElementType elementType;     
    public Vector3 position;           
    public Vector3 size;
    public Vector3 rotation;
    public Dictionary<string, string> parameters; // Key-value pairs for parameters
}
