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
        Collider collider = WorldManager.WeaponAimSystem.BaseAimSurface;
        float raycastDistance = collider.bounds.max.magnitude * 2;
        Ray invertRay = new Ray(launchAnchor.position + launchAnchor.forward * raycastDistance, -launchAnchor.forward);
        if (collider.Raycast(invertRay, out RaycastHit hitInfo, raycastDistance))
        {
            _displayRoot.SetActive(true);
            transform.position = WorldManager.Player.Camera.WorldToScreenPoint(hitInfo.point);
        }
        else
        {
            _displayRoot.SetActive(false);
        }
    }
}
