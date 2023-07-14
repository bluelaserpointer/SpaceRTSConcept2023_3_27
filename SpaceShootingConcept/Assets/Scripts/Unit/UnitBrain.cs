using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

[DisallowMultipleComponent]
public abstract class UnitBrain : MonoBehaviour
{
    [SerializeField]
    Camp _camp;
    public Camp Camp
    {
        get => _camp;
        set => _camp = value;
    }
    public abstract Unit OperatingUnit { get; }
    public bool IsEnemy(Camp camp)
    {
        return Camp != camp;
    }
    public bool IsEnemy(Unit unit)
    {
        return IsEnemy(unit.Camp);
    }
    public abstract void Request(UnitRequest request);
}
