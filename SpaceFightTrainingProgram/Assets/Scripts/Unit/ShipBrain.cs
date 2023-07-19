using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ShipBrain : UnitBrain
{
    public override Unit OperatingUnit => OperatingShip;
    public ShipUnit OperatingShip {
        get => _operatingShip;
        set
        {
            if(_operatingShip != value)
            {
                _operatingShip = value;
                OnControlShipChange(_operatingShip, value);
            }
        }
    }

    ShipUnit _operatingShip;

    public virtual void OnControlShipChange(ShipUnit oldShip, ShipUnit newShip) {
    }
}
