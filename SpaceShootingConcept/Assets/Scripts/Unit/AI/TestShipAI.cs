using IzumiTools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestShipAI : ShipBrain
{
    [SerializeField]
    Cooldown _targetUpdateInterval = new Cooldown(4);
    public enum State { Idle, Attacking }
    public State state;
    public Unit targetEnemy;

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
        if (OperatingShip.IsDocked)
        {
            OperatingShip.LeaveDock();
        }
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
            if (_targetUpdateInterval.AddDeltaTimeAndEat())
            {
                TargetNearstEnemy();
            }
            CoaxialWeaponOperation();
        }
    }
    public void CoaxialWeaponOperation()
    {
        if (coaxialWeapons.Count == 0)
        {
            return;
        }
        float enemyDistance = Vector3.Distance(OperatingShip.transform.position, targetEnemy.transform.position);
        OperatingShip.RotateTowards(Quaternion.LookRotation(targetEnemy.transform.position - OperatingShip.transform.position, OperatingShip.transform.up));
        bool hitable = ExtendedMath.TryDeflectShoot(
            coaxialWeapons[0].LaunchAnchor.position,
            targetEnemy.Rigidbody.position,
            targetEnemy.Rigidbody.velocity - OperatingShip.Rigidbody.velocity,
            coaxialWeapons[0].ProjectileAvgVelocity,
            out Vector3 estimateAimPosition);
        if (!hitable)
            estimateAimPosition = targetEnemy.Rigidbody.position;
        Quaternion targetRotation = Quaternion.LookRotation(estimateAimPosition - OperatingShip.Rigidbody.position, OperatingShip.transform.up);
        OperatingShip.RotateTowards(targetRotation);
        OperatingShip.RelativeMovementInput = Vector3.forward * 1;
        if (hitable && Quaternion.Angle(OperatingShip.Rigidbody.rotation, targetRotation) < 5)
        {
            coaxialWeapons.ForEach(each => each.Trigger());
        }
        foreach (Weapon weapon in coaxialWeapons)
        {
            weapon.AimPosition(estimateAimPosition);
        }
    }
    public void TargetNearstEnemy()
    {
        targetEnemy = WorldManager.Instance.GetTargetableEnemies(Camp).FindMin(enemy => Vector3.Distance(OperatingShip.transform.position, enemy.transform.position));
        if (targetEnemy != null)
        {
            state = State.Attacking;
        }
        else
        {
            state = State.Idle;
        }
    }

    public override void Request(UnitRequest request)
    {
    }
}
