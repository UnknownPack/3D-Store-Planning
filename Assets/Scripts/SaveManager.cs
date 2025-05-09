using System;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;
    public List<GameObject> prefabDictionary = new List<GameObject>();
    private Dictionary<string, Prefab_SaveData> _saveData;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        _saveData = new Dictionary<string, Prefab_SaveData>();
    }
    //For future implementation

    void SaveWork()
    {
        List<Prefabs_MetaData> saveableGameObjects = new List<Prefabs_MetaData>(FindObjectsByType<Prefabs_MetaData>(FindObjectsSortMode.None));
        _saveData.Clear();
        foreach (var prefab in saveableGameObjects)
        {
            Prefab_SaveData prefabSaveData;
            Transform prefabTransform = prefab.transform;
            prefabSaveData.position = prefabTransform.position;
            prefabSaveData.rotation = prefabTransform.rotation;
            prefabSaveData.scale = prefabTransform.localScale;
            string objectName = prefab.PrefabName.Replace("(Clone)", "");
            _saveData.Add(objectName, prefabSaveData);
        }
        
        // Save on drive/ text-file or other method
    }

    void SpawnSavedWork()
    {
        if (_saveData.Count <= 0)
            return;

        foreach (var obj in _saveData)
        {
            GameObject result = prefabDictionary.Find(go => go.name == obj.Key);
            if(result != null)
            {
                var data = obj.Value;
                GameObject instance = Instantiate(result, data.position, data.rotation);
                instance.transform.localScale = data.scale;
            }
        }
    }
}


