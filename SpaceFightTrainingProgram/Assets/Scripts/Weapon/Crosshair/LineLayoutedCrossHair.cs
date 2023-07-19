using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(LookTransform))]
public class LineLayoutedCrossHair : MonoBehaviour
{
    LookTransform _lookTransform;
    void Awake()
    {
        _lookTransform = GetComponent<LookTransform>();
        _lookTransform.target = GameObject.Find("ShipPlayer").transform;
    }
}
