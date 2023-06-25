using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class DeflectionShootingGuide : MonoBehaviour
{
    public new Camera camera;
    public Rigidbody launcherRigidbody;
    public Rigidbody targetRigidbody;
    public Transform launchAnchor;
    public float projectileVelocity;
    public Transform targetMeshRoot;

    [Header("Reference")]
    public Image colliderImage;
    public Image originImage;
    public Image targetImage;
    public LineRenderer lineRenderer;
    [SerializeField]
    IzumiTools.ReuseNest<MeshFilter> _shadowMeshFilterNest;

    public Vector3 EstimateTargetPosition { get; private set; }
    public Vector3 EstimateDelta => EstimateTargetPosition - targetRigidbody.position;

    public void Init(Camera camera, Rigidbody launcherRigidbody, Transform launchAnchor, float projectileVelocity, Rigidbody targetRigidbody, Transform targetMeshRoot)
    {
        this.camera = camera;
        this.launcherRigidbody = launcherRigidbody;
        this.launchAnchor = launchAnchor;
        this.projectileVelocity = projectileVelocity;
        this.targetRigidbody = targetRigidbody;
        this.targetMeshRoot = targetMeshRoot;
    }
    private void Awake()
    {
        lineRenderer.positionCount = 2;
    }
    private void LateUpdate()
    {
        EstimatePosition();
        UpdateShadow();
        UpdateGuideAndLine();
    }
    private void EstimatePosition()
    {
        float hitTimeEstimate = IzumiTools.ExtendedMath.EstimateBulletFlightTimeOnDeflectionShooting(
            launchAnchor.position,
            projectileVelocity,
            targetRigidbody.position,
            targetRigidbody.velocity - launcherRigidbody.velocity);
        EstimateTargetPosition = targetRigidbody.position + (targetRigidbody.velocity - launcherRigidbody.velocity) * hitTimeEstimate;
    }
    private void UpdateShadow()
    {
        _shadowMeshFilterNest.InactivateAll();
        foreach (var meshFilter in targetMeshRoot.GetComponentsInChildren<MeshFilter>())
        {
            var shadow = _shadowMeshFilterNest.Get();
            shadow.mesh = meshFilter.mesh;
            shadow.transform.SetPositionAndRotation(meshFilter.transform.position + EstimateDelta, meshFilter.transform.rotation);
            shadow.transform.localScale = meshFilter.transform.lossyScale;
        }
    }
    private void UpdateGuideAndLine()
    {
        lineRenderer.SetPositions(new Vector3[] {targetRigidbody.position, EstimateTargetPosition});
        float ymaxBound = camera.WorldToScreenPoint(targetRigidbody.ClosestPointOnBounds(targetRigidbody.position + camera.transform.up * 100)).y;
        float yminBound = camera.WorldToScreenPoint(targetRigidbody.ClosestPointOnBounds(targetRigidbody.position - camera.transform.up * 100)).y;
        float xmaxBound = camera.WorldToScreenPoint(targetRigidbody.ClosestPointOnBounds(targetRigidbody.position + camera.transform.right * 100)).x;
        float xminBound = camera.WorldToScreenPoint(targetRigidbody.ClosestPointOnBounds(targetRigidbody.position - camera.transform.right * 100)).x;
        Vector2 boundSize = new Vector2(Mathf.Abs(xmaxBound - xminBound), Mathf.Abs(ymaxBound - yminBound));
        colliderImage.transform.position = new Vector2((xmaxBound + xminBound) / 2, (ymaxBound + yminBound) / 2);
        colliderImage.rectTransform.sizeDelta = boundSize;
        originImage.transform.position = camera.WorldToScreenPoint(targetRigidbody.position);
        targetImage.transform.position = camera.WorldToScreenPoint(EstimateTargetPosition);
    }
}
