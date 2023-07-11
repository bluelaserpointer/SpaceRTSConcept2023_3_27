using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FormationAI : ShipBrain
{
    public Camp camp;
    public override Camp Camp => camp;

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
        Vector3 dstPos = Formation.transform.position + formationDeltaPosition;
        Vector3 dstDelta = dstPos - OperatingShip.transform.position;
        Vector3 dstDirection = dstDelta.normalized;
        float distance = Vector3.Distance(dstPos, OperatingShip.transform.position);
        if(distance > 5)
        {
            OperatingShip.brakeMode = false;
            Vector3 velocity = OperatingShip.Rigidbody.velocity;
            Vector3 UnwantedVelocity = velocity - Vector3.Project(velocity, dstDirection);
            Vector3 CorrectionInput = -UnwantedVelocity / OperatingShip.EngineTopAccel;
            if(CorrectionInput.sqrMagnitude > 1)
            {
                //print("correct " + name + ", " + CorrectionInput.sqrMagnitude);
                OperatingShip.GlobalMovementInput = CorrectionInput.normalized;
            }
            else
            {
                //print("push " + name + ", " + CorrectionInput.sqrMagnitude);
                OperatingShip.GlobalMovementInput = CorrectionInput + (dstPos - OperatingShip.transform.position).normalized * Mathf.Sqrt(1 - CorrectionInput.sqrMagnitude);
            }
        }
        else
        {
            OperatingShip.brakeMode = true;
        }
        OperatingShip.FaceRotation(formationRotation);

        if(Formation.Fire)
        {
            foreach (Weapon weapon in OperatingShip.weapons)
            {
                weapon.AimPosition(Formation._targetEnemy.transform.position);
                weapon.Trigger();
            }
        }
    }
}
