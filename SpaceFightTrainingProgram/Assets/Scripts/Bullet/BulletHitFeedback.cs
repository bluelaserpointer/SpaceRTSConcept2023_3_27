using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct BulletHitFeedback
{
    public Bullet bullet;
    public UnitDamageCollider damageCollier;
    public bool isHit;
    public UnitEffectFeedback effect;
}
