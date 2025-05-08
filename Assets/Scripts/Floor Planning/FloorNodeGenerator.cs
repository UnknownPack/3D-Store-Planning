using System.Collections.Generic;
using UnityEngine;

public class FloorNodeGenerator : MonoBehaviour
{
    public GameObject nodePlaceHolder;
    public float offset = 5f;
    private List<GameObject> nodes;

    void Start()
    { 
        int width = Mathf.RoundToInt(transform.localScale.x); 
        int length = Mathf.RoundToInt(transform.localScale.z);
        nodes = new List<GameObject>();
        GameObject parent = new GameObject("FloorNodes");
        for (int i = -width; i < width; i++)
        {
            for (int k = -length; k < length; k++)
            {
                GameObject instance = Instantiate(nodePlaceHolder, new Vector3(i * offset, 0f, k * offset), Quaternion.identity);
                nodes.Add(instance); 
                instance.transform.SetParent(parent.transform);
            }
        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}

public class FloorNode
{
    private Vector3 _position; 
    public GameObject Prefab { get; set; } 

    public FloorNode(Vector3 position, GameObject visual)
    {
        this._position = position;
        this.Prefab = visual;
    }
    
}
