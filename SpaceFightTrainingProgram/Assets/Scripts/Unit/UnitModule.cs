using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class UnitModule : MonoBehaviour
{
    public Unit Unit { get; private set; }

    public void Init(Unit unit)
    {
        Unit = unit;
    }
}
