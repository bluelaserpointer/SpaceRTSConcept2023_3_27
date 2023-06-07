using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class WeaponAimSystem : MonoBehaviour
{
    [Header("Basic")]
    public new Camera camera;
    public List<Rigidbody> guideDSBodies;
    public List<Weapon> weapons;

    [Header("Reference")]
    [SerializeField]
    Transform _crosshairNest;
    [SerializeField]
    IzumiTools.ReuseNest<DeflectionShootingGuide> _dsGuideNest;
    [SerializeField]
    DeflectShootingShadowMaker _dsShadowMaker;
    [SerializeField]
    Collider _baseAimSurface;
    [SerializeField]
    BulletLineDrawer _bulletLineDrawer;

    public Unit Unit { get; private set; }
    public Collider BaseAimSurface => _baseAimSurface;

    readonly Dictionary<Weapon, GameObject> _weaponToCrosshair = new Dictionary<Weapon, GameObject>();
    Vector3 estimateMainTargetPosition;
    private void Update()
    {
        //crosshair update
        List<Transform> usedCrosshairTfs = new List<Transform>();
        foreach (Weapon weapon in WorldManager.Player.ActiveWeapons)
        {
            GameObject crosshairObj;
            if (!_weaponToCrosshair.TryGetValue(weapon, out crosshairObj))
            {
                _weaponToCrosshair.Add(weapon, crosshairObj = weapon.GenerateCrosshair());
                crosshairObj.transform.SetParent(_crosshairNest, false);
            }
            crosshairObj.SetActive(true);
            usedCrosshairTfs.Add(crosshairObj.transform);
        }
        foreach (Transform crosshairTf in _crosshairNest)
        {
            if (!usedCrosshairTfs.Contains(crosshairTf))
            {
                crosshairTf.gameObject.SetActive(false);
            }
        }
        //ds guide update
        _dsGuideNest.InactivateAll();
        foreach (Rigidbody targetRigidbody in guideDSBodies)
        {
            DeflectionShootingGuide guide = _dsGuideNest.Get(out bool _);
            guide.Init(camera, Unit.Rigidbody, targetRigidbody, weapons[0].LaunchAnchor, weapons[0].ProjectileAvgVelocity);
            if(_dsShadowMaker._targetObject == targetRigidbody.gameObject)
            {
                estimateMainTargetPosition = guide.EstimateTargetPosition;
                _dsShadowMaker.SetEstimateTargetPosition(guide.EstimateTargetPosition);
            }
        }
    }
    private void LateUpdate()
    {
        
    }
    public void SetUnit(Unit unit)
    {
        Unit = unit;
    }
    public void SetWeapons(List<Weapon> weapons)
    {
        this.weapons = weapons;
        if (weapons[0].Coaxial)
        {
            _bulletLineDrawer.LineRenderer.enabled = true;
            _bulletLineDrawer.Init(Unit.Rigidbody, weapons[0].LaunchAnchor, weapons[0].ProjectileAvgVelocity);
        }
        else
        {
            _bulletLineDrawer.LineRenderer.enabled = false;
        }
    }
    public Vector3 UpdateAim()
    {
        if (weapons.Count == 0)
        {
            print("<!>weapon list empty...");
            return Vector3.zero;
        }
        Vector3 aimPosition = Vector3.zero;
        _baseAimSurface.transform.localScale = Vector3.one * Vector3.Distance(estimateMainTargetPosition, Unit.transform.position) / 2;
        Ray cameraRay = new Ray(camera.transform.position, camera.transform.forward);
        bool hitAnything = false;
        bool freeAimMode = false;
        if (freeAimMode)
        {
            foreach (RaycastHit hitInfo in Physics.RaycastAll(cameraRay, 1000))
            {
                GameObject hitObject = hitInfo.collider.gameObject;
                UnitDamageCollider hitParts = hitObject.GetComponent<UnitDamageCollider>();
                if (hitParts != null && hitParts.Unit != Unit)
                {
                    aimPosition = hitInfo.point;
                    hitAnything = true;
                    break;
                }
            }
        }
        if (!hitAnything)
        {
            _baseAimSurface.transform.position = Unit.transform.position;
            float sphereRadiusDouble = _baseAimSurface.bounds.max.magnitude * 2;
            Ray invertCameraRay = new Ray(cameraRay.origin + cameraRay.direction * sphereRadiusDouble, -cameraRay.direction);
            //ray origin already in collider doesent hit, so we need invert the ray.
            if (_baseAimSurface.Raycast(invertCameraRay, out RaycastHit hitInfo, sphereRadiusDouble))
            {
                aimPosition = hitInfo.point;
            }
        }
        foreach (Weapon weapon in weapons)
        {
            weapon.AimPosition(aimPosition);
        }
        return aimPosition;
    }
}
