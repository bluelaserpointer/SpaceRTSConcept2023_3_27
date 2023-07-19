using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FormationAI : ShipBrain
{
    List<Weapon> weapons = new List<Weapon>();
    public Formation Formation
    {
        get => _formation;
        set => _formation = value;
    }
    Formation _formation;

    public Vector3 formationDeltaPosition;
    public Quaternion formationRotation;

    public override void OnControlShipChange(ShipUnit oldShip, ShipUnit newShip)
    {
        base.OnControlShipChange(oldShip, newShip);
        weapons.Clear();
        foreach (var weapon in OperatingUnit.weapons)
        {
            weapons.Add(weapon);
        }
    }
    private void Update()
    {
        if (OperatingShip == null)
        {
            Destroy(gameObject);
            return;
        }
        OperatingShip.MoveTowards(Formation.transform.position + formationDeltaPosition, 5);
        OperatingShip.RotateTowards(formationRotation);

        if(Formation.Fire)
        {
            foreach (Weapon weapon in OperatingShip.weapons)
            {
                weapon.AimPosition(Formation._targetEnemy.transform.position);
                weapon.Trigger();
            }
        }
    }

    public override void Request(UnitRequest request)
    {
    }
}
