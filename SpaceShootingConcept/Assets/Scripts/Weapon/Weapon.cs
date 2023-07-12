using System.Collections;
using System.Collections.Generic;
using UnityEditor.Compilation;
using UnityEngine;

public abstract class Weapon : UnitModule
{
    public abstract Damage ExpectedDamage { get; }
    public abstract Transform LaunchAnchor { get; }
    public abstract float ProjectileAvgVelocity { get; }
    public abstract bool Coaxial { get; }
    public abstract void Init();
    public abstract GameObject GenerateCrosshair();
    public abstract void Trigger();
    public abstract void Fire();
    public abstract void AimPosition(Vector3 position);
    public bool DeflectAim(Unit targetUnit)
    {
        if (IzumiTools.ExtendedMath.TryDeflectShoot(
                Unit.Rigidbody,
                Unit.transform.position,
                ProjectileAvgVelocity,
                targetUnit.Rigidbody,
                targetUnit.transform.position,
                out Vector3 aimPosition))
        {
            AimPosition(aimPosition);
            return true;
        }
        return false;
    }
}
