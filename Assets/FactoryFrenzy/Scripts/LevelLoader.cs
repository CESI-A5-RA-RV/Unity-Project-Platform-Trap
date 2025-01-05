using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LevelLoader : MonoBehaviour
{
    public GameObject[] elementPrefabs;
    public GameObject backgroundPrefab;

    public string levelJsonPath = "";

    public void LoadLevel()
    {
        string json = File.ReadAllText(levelJsonPath);

        LevelData levelData = JsonUtility.FromJson<LevelData>(json);
        //SetBackground(levelData.backgroundName);
        foreach (LevelElementSO element in levelData.elements)
        {
            InstantiateElement(element);
        }
    }

    private void InstantiateElement(LevelElementSO element)
    {
        // Find the correct prefab based on the element type
        GameObject prefab = GetPrefabForElementType(element.elementType);
        if (prefab == null) return;

        // Instantiate the prefab
        GameObject instance = Instantiate(prefab, element.position, Quaternion.Euler(element.rotation));
        instance.transform.localScale = element.size;

        // Apply additional parameters if needed
        ApplyParameters(instance, element.parameters);
    }

    private GameObject GetPrefabForElementType(ElementType elementType)
    {
        switch (elementType)
        {
            case ElementType.StartPlatform:
                return elementPrefabs[0];
            case ElementType.ClassicPlatform:
                return elementPrefabs[1];
            case ElementType.MobilePlatform:
                return elementPrefabs[2];
            case ElementType.Bumper:
                return elementPrefabs[3];
            case ElementType.ProjectileLauncher:
                return elementPrefabs[4];
            case ElementType.Trampoline:
                return elementPrefabs[5];
            case ElementType.Fan:
                return elementPrefabs[6];
            case ElementType.Checkpoint:
                return elementPrefabs[7];
            default:
                return null;
        }
    }

    private void SetBackground(string backgroundName)
    {
        if (backgroundPrefab != null)
        {
            Instantiate(backgroundPrefab, Vector3.zero, Quaternion.identity);
        }
    }

    private void ApplyParameters(GameObject instance, Dictionary<string, string> parameters)
    {
        var componentTypes = instance.GetComponents<MonoBehaviour>();

        foreach (var component in componentTypes)
        {
            var fields = component.GetType().GetFields(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            foreach (var field in fields)
            {
                if (parameters.ContainsKey(field.Name))
                {
                    if (field.FieldType == typeof(float) && float.TryParse(parameters[field.Name], out float value))
                    {
                        field.SetValue(component, value);
                    }
                }
            }
        }
    }

    void Start()
    {
        LoadLevel();
    }
}
