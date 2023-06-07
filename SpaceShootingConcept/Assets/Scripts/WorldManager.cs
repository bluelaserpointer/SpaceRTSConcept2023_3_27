using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class WorldManager : MonoBehaviour
{
    [SerializeField]
    Transform _stageContentsRoot;
    [SerializeField]
    ShipPlayer _shipPlayer;
    [SerializeField]
    WeaponAimSystem _weaponAimSystem;

    public static WorldManager Instance { get; private set; }
    public static Transform StageContentsRoot => Instance._stageContentsRoot;
    public static ShipPlayer Player => Instance._shipPlayer;
    public static WeaponAimSystem WeaponAimSystem => Instance._weaponAimSystem;

    private void Awake()
    {
        Instance = this;
    }
    private void Update()
    {
        if(Input.GetKey(KeyCode.Escape))
        {
            Time.timeScale = Time.timeScale == 0 ? 1 : 0;
        }
    }
    public List<Unit> GetAllUnits()
    {
        List<Unit> units = new List<Unit>();
        foreach(Transform t in transform)
        {
            if(t.TryGetComponent(out Unit unit))
            {
                units.Add(unit);
            }
        }
        return units;
    }
    public bool TryGetNearstEnemy(Unit myUnit, out Unit enemy)
    {
        foreach (Transform t in transform)
        {
            if (t.TryGetComponent(out Unit unit))
            {
                if(myUnit.IsEnemy(unit))
                {
                    enemy = unit;
                    return true;
                }
            }
        }
        enemy = null;
        return false;
    }
}
