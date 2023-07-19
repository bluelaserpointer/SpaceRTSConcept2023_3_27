using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(LineRenderer))]
public class BulletLineDrawer : MonoBehaviour
{
    public float bulletSpeed;
    public Rigidbody launcherRigidbody;
    public Transform launcherTransform;
    public float lineLength = 100;
    public LineRenderer LineRenderer { get; private set; }
    public Ray LineRay { get; private set; }

    private void Awake()
    {
        LineRenderer = GetComponent<LineRenderer>();
        LineRenderer.positionCount = 2;
    }
    public void Init(Rigidbody launcherRigidbody, Transform launcherTransform, float bulletSpeed)
    {
        this.launcherRigidbody = launcherRigidbody;
        this.launcherTransform = launcherTransform;
        this.bulletSpeed = bulletSpeed;
    }
    private void Update()
    {
        if (launcherTransform == null)
            return;
        Vector3 origin = launcherTransform.position;
        Vector3 direction = launcherTransform.forward * bulletSpeed;
        if (launcherRigidbody != null)
            direction += launcherRigidbody.velocity;
        direction.Normalize();
        LineRay = new Ray(origin, direction);
        LineRenderer.SetPositions(new Vector3[] {origin, origin + direction * lineLength});
    }
}
