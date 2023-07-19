using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UnitRequest
{
    public abstract void DefaultOperation(Unit unit);
    public class DockRequest : UnitRequest
    {
        public Dock Dock { get; private set; }
        public DockRequest(Dock dock)
        {
            Dock = dock;
        }
        public override void DefaultOperation(Unit unit)
        {
            if(unit.GetType() == typeof(ShipUnit))
            {
                ShipUnit ship = (ShipUnit)unit;
                ship.AssignDockPort(Dock.SuggestPort()); //TODO: wait until dock has empty port
            }
        }
    }
    public class LeaveDockRequest : UnitRequest
    {
        public LeaveDockRequest()
        {
        }
        public override void DefaultOperation(Unit unit)
        {
            if (unit.GetType() == typeof(ShipUnit))
            {
                ShipUnit ship = (ShipUnit)unit;
                ship.LeaveDock();
            }
        }
    }
    public class AttackRequest : UnitRequest
    {
        public Unit Unit { get; private set; }
        public AttackRequest(Unit unit)
        {
            Unit = unit;
        }
        public override void DefaultOperation(Unit unit)
        {
        }
    }
}
