using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.UI.CanvasScaler;

[DisallowMultipleComponent]
public abstract class Unit : MonoBehaviour
{
    public string displayName;
    public static string Tag = "Unit";
    public abstract UnitBrain Brain { get; }
    public Camp Camp => Brain?.Camp;
    public readonly List<UnitModule> modules = new List<UnitModule>();
    public readonly List<Weapon> weapons = new List<Weapon>();
    public Rigidbody Rigidbody { get; protected set; }
    public abstract bool IsDead { get; }
    public virtual bool IsInert
    {
        get => _isInert;
        set
        {
            _isInert = value;
            if(value)
            {
                Brain.enabled = false;
                foreach (var childCollider in transform.GetComponentsInChildren<Collider>())
                {
                    childCollider.gameObject.layer = LayerMask.NameToLayer("Inert");
                }
            }
            else
            {
                Brain.enabled = true;
                foreach (var childCollider in transform.GetComponentsInChildren<Collider>())
                {
                    childCollider.gameObject.layer = LayerMask.NameToLayer("Default");
                }
            }
        }
    }
    bool _isInert;

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
    public abstract UnitEffectFeedback Damage(Damage damage);
    public virtual void Death() {
    }
    public bool IsEnemy(Camp camp)
    {
        return Brain != null ? Brain.IsEnemy(camp) : false;
    }
    public bool IsEnemy(Unit unit)
    {
        return IsEnemy(unit.Camp);
    }
    public bool IsEnemy(UnitBrain ai)
    {
        return ai.OperatingUnit != null ? IsEnemy(ai.OperatingUnit) : false;
    }
    public void Request(UnitRequest request)
    {

    }
}
