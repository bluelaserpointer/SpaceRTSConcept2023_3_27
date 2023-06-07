using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Billboarder : MonoBehaviour
{
    public Camera referencingCamera;
    void LateUpdate()
    {
        if (referencingCamera == null)
            return;
        transform.LookAt(referencingCamera.transform);
        transform.Rotate(0, 180, 0);
    }
}
