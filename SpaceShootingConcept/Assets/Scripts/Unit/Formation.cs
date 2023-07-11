using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Formation : MonoBehaviour
{
    [Header("Debug")]
    public Unit _targetEnemy;

    public List<FormationAI> ais;

    public float layerRaidus = 5, layerSpan = 5;
    public int layerCapacity = 3;
    public bool Fire { get; private set; }

    private void Start()
    {
        ais.AddRange(transform.GetComponentsInChildren<FormationAI>());
        foreach (FormationAI ai in ais)
        {
            ai.Formation = this;
        }
        //Fire = true;
    }
    void Update()
    {
        Vector3 avgPos = Vector3.zero;
        for (int i = 0; i < ais.Count; i++)
        {
            int layer = 0;
            for (int capacity = LayerCapacity(0); capacity < i + 1;)
            {
                capacity += LayerCapacity(++layer);
            }
            float z = -layerSpan * layer;
            float radius = layerRaidus * layer;
            float angle = 2 * Mathf.PI * (i - LayerCapacity(layer - 1)) / LayerCapacity(layer);
            Vector3 deltaPosition = transform.TransformPoint(new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, z)) - transform.position;
            ais[i].formationDeltaPosition = deltaPosition;
            ais[i].formationRotation = transform.rotation;
            avgPos += ais[i].OperatingShip.transform.position;
        }
        avgPos /= ais.Count;
        //transform.position = _targetEnemy.transform.position;
        //transform.rotation = Quaternion.LookRotation(_targetEnemy.transform.position - avgPos, Vector3.up);
    }
    int LayerCapacity(int layer)
    {
        return 1 + layer * layerCapacity;
    }
}
