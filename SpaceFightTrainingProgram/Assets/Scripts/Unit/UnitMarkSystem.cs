using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class UnitMarkSystem : MonoBehaviour
{
    [SerializeField]
    IzumiTools.ReuseNest<UnitMarker> _markerNest;

    readonly Dictionary<Unit, UnitMarker> _markRecord = new Dictionary<Unit, UnitMarker>();
    Camp _myCamp;
    private void Start()
    {
        _myCamp = WorldManager.Player.OperatingShip.Camp;
    }
    private void Update()
    {
        foreach (Unit unit in WorldManager.Instance.GetTargetableEnemyUnits(_myCamp))
        {
            if (_markRecord.ContainsKey(unit))
                continue;
            UnitMarker marker = _markerNest.Get();
            _markRecord.Add(unit, marker);
            marker.MarkTarget = unit;
        }
    }
}
