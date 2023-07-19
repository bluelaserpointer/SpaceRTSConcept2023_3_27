using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DustGenerator : MonoBehaviour
{
    public Camera referencingCamera;
    [SerializeField]
    Billboarder _dustPrefab;
    [SerializeField]
    float _dustCount = 1000;
    [SerializeField]
    float _generateRadius = 50;
    [SerializeField]
    Vector2 _dustScaleRange = new Vector2(0.1F, 1.0F);

    void Start()
    {
        for(int i = 0; i < _dustCount; i++)
        {
            Billboarder dust = Instantiate(_dustPrefab, transform);
            _dustPrefab.GetComponent<Billboarder>().referencingCamera = referencingCamera;
            dust.transform.position = referencingCamera.transform.position + Random.insideUnitSphere * _generateRadius;
            dust.transform.localScale = Vector3.one * Random.Range(_dustScaleRange.x, _dustScaleRange.y);
        }
    }

    void Update()
    {
        float sqrRaidus = _generateRadius * _generateRadius;
        foreach(Transform child in transform)
        {
            Vector3 difference = child.position - referencingCamera.transform.position;
            if(difference.sqrMagnitude > sqrRaidus)
            {
                child.position = referencingCamera.transform.position + Random.onUnitSphere * _generateRadius;
            }
        }
    }
}
