using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[DisallowMultipleComponent]
public abstract class Unit : MonoBehaviour
{
    public abstract UnitBrain Brain { get; }
    public Camp Camp => Brain?.Camp;
    public readonly List<UnitModule> modules = new List<UnitModule>();
    public readonly List<Weapon> weapons = new List<Weapon>();
    public Rigidbody Rigidbody { get; protected set; }

    public void LinkAllModules()
    {
        modules.Clear();
        weapons.Clear();
        foreach (UnitModule module in GetComponentsInChildren<UnitModule>())
        {
            module.Init(this);
            modules.Add(module);
            if (typeof(Weapon).IsAssignableFrom(module))
            {
                weapons.Add(module as Weapon);
            }
        }
    }
    public virtual void Damage(Damage damage)
    {
        //Health -= damage.damage;
    }
    public bool IsEnemy(Unit unit)
    {
        return Brain != null ? Brain.IsEnemy(unit) : false;
    }
}
