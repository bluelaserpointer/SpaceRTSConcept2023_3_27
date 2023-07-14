using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VecAngTester : MonoBehaviour
{
    [SerializeField]
    Transform o, t;

    Vector3 lastDelta;
    bool haveLastPos;

    // Update is called once per frame
    void Update()
    {
        if(haveLastPos)
        {
            o.Rotate(Vector3.up * Vector3.SignedAngle(lastDelta.Set(y: 0), (t.position - o.position).Set(y: 0), Vector3.up));
        }
        else
        {
            haveLastPos = true;
        }
        lastDelta = t.position - o.position;
        //print(Vector3.SignedAngle((t1.position - o.position).Set(y: 0), (t2.position - o.position).Set(y: 0), Vector3.up));
    }
}
