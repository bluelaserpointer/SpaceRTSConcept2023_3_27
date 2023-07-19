using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class AutoDestructOnChildless : MonoBehaviour
{
    private void Update()
    {
        if(transform.childCount == 0)
            Destroy(gameObject);
    }
}
