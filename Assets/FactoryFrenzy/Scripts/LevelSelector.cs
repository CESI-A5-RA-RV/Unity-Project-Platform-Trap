using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class LevelSelector : MonoBehaviour
{
    // Start is called before the first frame update
    public TMP_Dropdown selectedLevelId;
    public int levelId;
    void Start()
    {
        string homePath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile);
        string appDataPath = System.IO.Path.Combine(homePath, "AppData", "LocalLow");
        string jsonFilePath = System.IO.Path.Combine(appDataPath, "DefaultCompany", "Platform-Trap-Level-Editor", "levels.json");

        string json = System.IO.File.ReadAllText(jsonFilePath);

        MultiLevelData multiLevelData = JsonUtility.FromJson<MultiLevelData>(json);

        List<string> options = new List<string>();
        foreach(var option in multiLevelData.levels){
            options.Add(option.id.ToString());
        }
        selectedLevelId.ClearOptions();
        selectedLevelId.AddOptions(options);
        selectedLevelId.onValueChanged.AddListener(delegate{
            OnDropdownValueChanged();
        });
        levelId = selectedLevelId.value;

    }

    private void OnDropdownValueChanged(){
        Debug.Log(selectedLevelId.value);
        levelId = selectedLevelId.value;
    }

    
}
