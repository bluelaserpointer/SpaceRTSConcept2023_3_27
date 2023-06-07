using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestShipAI : ShipBrain
{
    public enum State { Idle, Attacking }
    public State state;
    public Camp camp;
    public override Camp Camp => camp;
    public ShipUnit targetEnemy;


    List<Weapon> coaxialWeapons = new List<Weapon>();
    List<Weapon> rotatableWeapons = new List<Weapon>();
    public override void OnControlShipChange(ShipUnit oldShip, ShipUnit newShip)
    {
        base.OnControlShipChange(oldShip, newShip);
        coaxialWeapons.Clear();
        rotatableWeapons.Clear();
        foreach (var weapon in OperatingUnit.weapons)
        {
            (weapon.Coaxial ? coaxialWeapons : rotatableWeapons).Add(weapon);
        }
    }
    private void Update()
    {
        if (OperatingShip == null)
            return;
        if (state == State.Idle)
        {
            TargetNearstEnemy();
        }
        else if (state == State.Attacking)
        {
            if (targetEnemy == null)
            {
                state = State.Idle;
                Update();
                return;
            }
            float enemyDistance = Vector3.Distance(OperatingShip.transform.position, targetEnemy.transform.position);
            if(coaxialWeapons.Count > 0)
            {
                float estimateHitTime = IzumiTools.ExtendedMath.EstimateBulletFlightTimeOnDeflectionShooting(
                    coaxialWeapons[0].LaunchAnchor.position,
                    coaxialWeapons[0].ProjectileAvgVelocity,
                    targetEnemy.Rigidbody.position,
                    targetEnemy.Rigidbody.velocity - OperatingShip.Rigidbody.velocity);
                Vector3 estimateTargetPosition = targetEnemy.Rigidbody.position + (targetEnemy.Rigidbody.velocity - OperatingShip.Rigidbody.velocity) * estimateHitTime;
                Quaternion targetRotation = Quaternion.LookRotation(estimateTargetPosition - OperatingShip.Rigidbody.position, OperatingShip.transform.up);
                OperatingShip.FaceRotation(targetRotation);
                OperatingShip.MovementInput = Vector3.forward * 1;
                if (Quaternion.Angle(OperatingShip.Rigidbody.rotation, targetRotation) < 5)
                {
                    coaxialWeapons.ForEach(each => each.Trigger());
                }

                foreach (Weapon weapon in OperatingShip.weapons)
                {
                    weapon.AimPosition(estimateTargetPosition);
                }
            }
            else
            {
                OperatingShip.FaceRotation(Quaternion.LookRotation(targetEnemy.transform.position - OperatingShip.transform.position, OperatingShip.transform.up));
            }
        }
    }
    public void TargetNearstEnemy()
    {
        bool foundEnemy = false;
        foreach(Transform tf in WorldManager.Instance.transform)
        {
            if (!tf.TryGetComponent<ShipUnit>(out var unit) || !IsEnemy(unit))
                continue;
            targetEnemy = unit;
            state = State.Attacking;
            foundEnemy = true;
            break;
        }
        if(!foundEnemy)
            state = State.Idle;
    }
}
