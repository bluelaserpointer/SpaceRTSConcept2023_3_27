using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Billboarder : MonoBehaviour
{
    public Camera referencingCamera;
    public Vector3 upVector = Vector3.up;
    void LateUpdate()
    {
        if (referencingCamera == null)
            return;
        transform.LookAt(referencingCamera.transform, upVector);
        transform.Rotate(0, 180, 0);
    }
}
