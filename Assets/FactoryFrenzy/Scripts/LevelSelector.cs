using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelSelector : MonoBehaviour
{
    // Start is called before the first frame update
    public TMP_Dropdown selectedLevelId;
    void Start()
    {
        string homePath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile);
        string appDataPath = System.IO.Path.Combine(homePath, "AppData", "LocalLow");
        string jsonFilePath = System.IO.Path.Combine(appDataPath, "DefaultCompany", "Platform-Trap-Level-Editor", "levels.json");

        string json = System.IO.File.ReadAllText(jsonFilePath);

        MultiLevelData multiLevelData = JsonUtility.FromJson<MultiLevelData>(json);

        //LevelData selectedLevel = multiLevelData.levels.Find(level => level.id == selectedLevelId);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
