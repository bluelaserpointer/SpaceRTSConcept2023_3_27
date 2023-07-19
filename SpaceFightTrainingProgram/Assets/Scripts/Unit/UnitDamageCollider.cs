using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider))]
public class UnitDamageCollider : MonoBehaviour
{
    public float Health { get; private set; }
    public Unit Unit { get; set; }
    public void Init(Unit unit)
    {
        Unit = unit;
    }
    public virtual UnitEffectFeedback Damage(Damage damage)
    {
        return new UnitEffectFeedback();
    }
    public virtual BulletHitFeedback BulletHit(Bullet bullet, Damage damage)
    {
        BulletHitFeedback feedback = new BulletHitFeedback();
        feedback.bullet = bullet;
        feedback.damageCollier = this;
        if (bullet.LaunchUnit == Unit)
        {
            feedback.isHit = false;
        }
        else
        {
            feedback.isHit = true;
            feedback.effect = Unit.Damage(damage);
        }
        return feedback;
    }
}
