using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ReceiveBulletHit : MonoBehaviour
{
    public abstract void Hit(Bullet bullet);
}
