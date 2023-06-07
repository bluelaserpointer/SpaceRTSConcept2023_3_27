using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAround : MonoBehaviour
{
    public float speed;
    public float radius = 1;

    // Update is called once per frame
    void Update()
    {
        float angle = Time.timeSinceLevelLoad * speed;
        transform.localPosition = new Vector3(radius * Mathf.Cos(angle), 0, radius * Mathf.Sin(angle));
    }
}
