using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class CopyOnLine : MonoBehaviour
{
    public GameObject prefab;
    public int count = 10;
    public float padding = 1;

    private void Start()
    {
        Generate();
    }
    [HideInInspector]
    public List<GameObject> GeneratedObjects;
    private void Update()
    {
        for(int i = 0; i < GeneratedObjects.Count; i++)
        {
            GeneratedObjects[i].transform.position = transform.position + transform.forward * padding * i;
        }
    }

    public void Generate()
    {
        transform.DestroyAllChildren();
        for(int i = 0; i < count; i++)
        {
            GeneratedObjects.Add(Instantiate(prefab, transform));
        }
    }
}
