using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DockPort : MonoBehaviour
{
    [SerializeField]
    float _positionFixSpeed = 32, _rotationFixSpeed = 32;

    public ShipUnit DockingUnit { get; private set; }
    public bool Avaliable => !Assigned;
    public bool Assigned => DockingUnit != null;
    public bool Docked => DockingUnit?.IsDocked ?? false;
    Vector3 dockDeltaPosition;
    Quaternion dockDeltaRotation;

    public bool Assign(ShipUnit ship)
    {
        if (Assigned)
        {
            return false;
        }
        DockingUnit = ship;
        return true;
    }
    public void OnDock(ShipUnit ship)
    {
        if(DockingUnit != ship)
        {
            print("<!> DockPort.Dock called from unexpected unit");
        }
        DockingUnit = ship;
        dockDeltaPosition = ship.transform.position - transform.position;
        dockDeltaRotation = ship.transform.rotation * Quaternion.Inverse(transform.rotation);
    }
    public void OnLeaving(ShipUnit ship)
    {
        if(DockingUnit == ship)
        {
            DockingUnit = null;
        }
        else
        {
            print("<!> DockPort.Leaving called from unexpected unit");
        }
    }
    private void Update()
    {
        if (Docked)
        {
            DockingUnit.transform.SetPositionAndRotation(transform.position + dockDeltaPosition, transform.rotation * dockDeltaRotation);
            dockDeltaPosition = Vector3.MoveTowards(dockDeltaPosition, Vector3.zero, _positionFixSpeed * Time.deltaTime);
            dockDeltaRotation = Quaternion.RotateTowards(dockDeltaRotation, Quaternion.identity, _rotationFixSpeed * Time.deltaTime);
        }
    }
}
