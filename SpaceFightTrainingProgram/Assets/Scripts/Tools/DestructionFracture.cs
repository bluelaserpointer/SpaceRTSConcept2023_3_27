using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[DisallowMultipleComponent]
public class DestructionFracture : MonoBehaviour
{
    [SerializeField]
    float _spreadForce = 50;
    [SerializeField]
    float _torqueForceScale = 5;
    [SerializeField]
    float _lifeTime = 5;

    float _spawnTime;
    private void Start()
    {
        foreach(Transform fracture in transform)
        {
            Rigidbody rb = fracture.AddComponent<Rigidbody>();
            rb.useGravity = false;
            rb.AddForce(fracture.localPosition.normalized * _spreadForce);
            rb.AddTorque(Random.insideUnitSphere * _torqueForceScale);
        }
        _spawnTime = Time.timeSinceLevelLoad;
    }
    private void Update()
    {
        if(Time.timeSinceLevelLoad - _spawnTime > _lifeTime)
        {
            Destroy(gameObject);
        }
    }
}
