using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelLoader : MonoBehaviour
{
    [SerializeField] public int selectedLevelId;
    [SerializeField] private List<LevelElement> elementPrefabs;
    private Dictionary<string, LevelElement> prefabDictionary;

    private void Start()
    {
        prefabDictionary = new Dictionary<string, LevelElement>();
        foreach (var prefab in elementPrefabs)
        {
            prefabDictionary[prefab.name] = prefab;
        }

        string homePath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile);
        string appDataPath = System.IO.Path.Combine(homePath, "AppData", "LocalLow");
        string jsonFilePath = System.IO.Path.Combine(appDataPath, "DefaultCompany", "Platform-Trap-Level-Editor", "levels.json");

        string json = System.IO.File.ReadAllText(jsonFilePath);

        MultiLevelData multiLevelData = JsonUtility.FromJson<MultiLevelData>(json);

        LevelData selectedLevel = multiLevelData.levels.Find(level => level.id == selectedLevelId);

        if (selectedLevel != null)
        {
            Debug.Log($"Loading Level: {selectedLevel.levelName}");
            LoadLevel(selectedLevel);
        }
        else
        {
            Debug.LogWarning("Level not found!");
        }
    }

    private void LoadLevel(LevelData levelData)
    {
        foreach (var elementData in levelData.elements)
        {
            Debug.Log(elementData.elementType);
            if (prefabDictionary.TryGetValue(elementData.elementType, out LevelElement prefab))
            {
                LevelElement element = Instantiate(prefab, Vector3.zero, Quaternion.identity);
                element.Initialize(elementData);
            }
            else
            {
                Debug.LogWarning($"Prefab for element type {elementData.elementType} not found!");
            }
        }
    }
}
