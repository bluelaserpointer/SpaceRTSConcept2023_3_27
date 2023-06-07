using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class LookTransform : MonoBehaviour
{
    public Transform target;
    public Transform upwardOverride;

    void Update()
    {
        transform.LookAt(target, upwardOverride != null ? upwardOverride.position - transform.position : Vector3.up);
    }
}
