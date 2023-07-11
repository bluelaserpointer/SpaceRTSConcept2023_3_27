using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGunCrosshair : MonoBehaviour
{
    [SerializeField]
    GameObject _displayRoot;

    public ShipGun Gun
    {
        get => _gun;
        set
        {
            Init(value);
        }
    }
    ShipGun _gun;

    public void Init(ShipGun gun)
    {
        _gun = gun;
    }
    private void Update()
    {
        if (Gun == null)
        {
            print("<!>gun null...(update)");
            return;
        }
        Transform launchAnchor = Gun.LaunchAnchor;
        Ray gunRay = new Ray(launchAnchor.position, launchAnchor.forward);
        bool hitAnything = false;
        Vector3 estimateHitPos = Vector3.zero;
        foreach (RaycastHit hitInfo in Physics.RaycastAll(gunRay, 1000))
        {
            GameObject hitObject = hitInfo.collider.gameObject;
            UnitDamageCollider hitParts = hitObject.GetComponent<UnitDamageCollider>();
            if (hitParts != null && hitParts.Unit != Gun.Unit)
            {
                estimateHitPos = hitInfo.point;
                hitAnything = true;
                break;
            }
        }
        if (!hitAnything)
        {
            Collider aimSurface = WorldManager.WeaponAimSystem.AimSurface;
            float raycastDistance = aimSurface.bounds.max.magnitude * 2;
            Ray invertGunRay = new Ray(launchAnchor.position + launchAnchor.forward * raycastDistance, -launchAnchor.forward);
            if (aimSurface.Raycast(invertGunRay, out RaycastHit hitInfo, raycastDistance))
            {
                estimateHitPos = hitInfo.point;
            }
            else
            {
                print("<!> invert gun raycast failed");
            }
        }
        transform.position = WorldManager.Player.Camera.WorldToScreenPoint(estimateHitPos).Set(z: 0);
    }
}
