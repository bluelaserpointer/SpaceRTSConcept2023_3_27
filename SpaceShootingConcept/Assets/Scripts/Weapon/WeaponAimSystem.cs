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
    public float baseAimDistance = 100;

    [Header("Reference")]
    [SerializeField]
    Transform _crosshairNest;
    [SerializeField]
    IzumiTools.ReuseNest<DeflectionShootingGuide> _dsGuideNest;
    [SerializeField]
    Collider _aimSurface;
    [SerializeField]
    AutoTargetModule _autoTargetModule;
    [SerializeField]
    BulletLineDrawer _bulletLineDrawer;

    public Unit Unit { get; private set; }
    public Collider AimSurface => _aimSurface;

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
            DeflectionShootingGuide guide = _dsGuideNest.Get();
            guide.Init(camera, Unit.Rigidbody, weapons[0].LaunchAnchor, weapons[0].ProjectileAvgVelocity, targetRigidbody, targetRigidbody.transform);
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
    public void UpdateAim()
    {
        if (weapons.Count == 0)
        {
            print("<!>weapon list empty...");
        }
        if (!_autoTargetModule.Avaliable)
        {
            AimOnCameraRay();
        }
        else
        {
            _autoTargetModule.HelpTarget(weapons);
        }
    }
    public void AimOnCameraRay()
    {
        Vector3 aimPosition = Vector3.zero;
        _aimSurface.transform.localScale = Vector3.one * baseAimDistance;
        Ray cameraRay = new Ray(camera.transform.position/* + camera.transform.forward * Vector3.Dot(Unit.transform.position - camera.transform.position, camera.transform.forward)*/, camera.transform.forward);
        bool hitAnything = false;
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
        if (!hitAnything)
        {
            _aimSurface.transform.position = Unit.transform.position;
            float sphereRadiusDouble = _aimSurface.bounds.max.magnitude * 2;
            Ray invertCameraRay = new Ray(cameraRay.origin + cameraRay.direction * sphereRadiusDouble, -cameraRay.direction);
            //ray origin in collider doesent hit, so we need invert the ray.
            if (_aimSurface.Raycast(invertCameraRay, out RaycastHit hitInfo, sphereRadiusDouble))
            {
                aimPosition = hitInfo.point;
            }
        }
        foreach (Weapon weapon in weapons)
        {
            weapon.AimPosition(aimPosition);
        }
    }
}
