using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelLoader : MonoBehaviour
{
    [SerializeField] private List<LevelElement> elementPrefabs;
    private Dictionary<string, LevelElement> prefabDictionary;

    private void Start()
    {
        prefabDictionary = new Dictionary<string, LevelElement>();
        foreach (var prefab in elementPrefabs)
        {
            prefabDictionary[prefab.name] = prefab;
        }

        string homeDirectory = System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile);
        string desktopPath = System.IO.Path.Combine(homeDirectory, "Desktop", "levels.json");

        string json = System.IO.File.ReadAllText(desktopPath);

        LevelData levelData = JsonUtility.FromJson<LevelData>(json);
        LoadLevel(levelData);   
    }

    private void LoadLevel(LevelData levelData)
    {
        foreach (var elementData in levelData.elements)
        {
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
