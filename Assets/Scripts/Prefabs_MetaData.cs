using UnityEngine;

public class Prefabs_MetaData : MonoBehaviour
{
    [HideInInspector] public string PrefabName;

    public void Initialize(string prefabName)
    {
        PrefabName = prefabName.Replace("(Clone)", "").Trim();;
    }
}

[System.Serializable]
public struct Prefab_SaveData
{
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;
}
