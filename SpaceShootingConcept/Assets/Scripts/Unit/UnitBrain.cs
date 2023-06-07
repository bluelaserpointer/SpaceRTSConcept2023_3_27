using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

[DisallowMultipleComponent]
public abstract class UnitBrain : MonoBehaviour
{
    public abstract Camp Camp { get; }
    public abstract Unit OperatingUnit { get; }
    public void Stop()
    {
    }
    public bool IsEnemy(Unit unit)
    {
        return Camp != unit.Camp;
    }
}
