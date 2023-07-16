using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class TestBullet : Bullet
{
    [SerializeField]
    LineRenderer debugLineRenderer;

    float enemyDistance;
    public void SetDebugLine(float enemyDistance)
    {
        this.enemyDistance = enemyDistance;
        debugLineRenderer.gameObject.SetActive(true);
    }
    protected override void Update()
    {
        base.Update();
        if (debugLineRenderer.gameObject.activeSelf)
        {
            debugLineRenderer.SetPositions(new Vector3[] { transform.position, transform.position + Rigidbody.velocity.normalized * enemyDistance});
        }
    }
}
