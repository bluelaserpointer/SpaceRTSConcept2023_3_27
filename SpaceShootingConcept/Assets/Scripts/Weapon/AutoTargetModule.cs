using IzumiTools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class AutoTargetModule : MonoBehaviour
{
    [Header("Stats")]
    public float effectiveAreaPx = 200;

    [Header("Reference")]
    [SerializeField]
    ReuseNest<Transform> _targetMarkerNest;
    [SerializeField]
    Image effectiveAreaRect;

    public Unit UserUnit { get; private set; }
    public bool Avaliable => _targetUnit != null;
    new Camera camera;
    Unit _targetUnit;
    private void Start()
    {
        Init(WorldManager.Player.Camera, WorldManager.Player.OperatingUnit);
    }
    private void Update()
    {
        if (UserUnit == null)
            return;
        UpdateTarget();
    }
    public void Init(Camera camera, Unit userUnit)
    {
        this.camera = camera;
        UserUnit = userUnit;
        effectiveAreaRect.rectTransform.sizeDelta = Vector2.one * effectiveAreaPx;
    }
    public void UpdateTarget()
    {
        _targetMarkerNest.InactivateAll();
        List<Unit> targetableUnits = WorldManager.Instance.GetTargetableUnits(unit =>
        {
            if (!unit.IsEnemy(UserUnit))
            {
                return false;
            }
            Vector2 unitScreenPoint = camera.WorldToScreenPoint(unit.transform.position);
            if (effectiveAreaRect.rectTransform.ContainsPoint(unitScreenPoint))
            {
                return true;
            }
            return false;
        });
        if(targetableUnits.Count == 0)
        {
            _targetUnit = null;
            return;
        }
        float minDistanceScore = float.MaxValue;
        Vector2 screenCentre = new Vector2(Screen.width / 2, Screen.height / 2);
        Unit targetUnit = null;
        foreach(Unit targetableUnit in targetableUnits)
        {
            float distance3d = Vector3.Distance(UserUnit.transform.position, targetableUnit.transform.position);
            float distance2d = Vector2.Distance(screenCentre, camera.WorldToScreenPoint(targetableUnit.transform.position));
            float distanceScore = distance2d * 100 + distance3d;
            if (distanceScore < minDistanceScore)
            {
                minDistanceScore = distanceScore;
                targetUnit = targetableUnit;
            }
        }
        _targetUnit = targetUnit;
        Transform markerTf = _targetMarkerNest.Get();
        markerTf.transform.position = camera.WorldToScreenPoint(_targetUnit.transform.position).Set(z: 0);
    }
    public void HelpTarget(Weapon weapon)
    {
        if (_targetUnit == null)
            return;
        weapon.DeflectAim(_targetUnit);
    }
    public void HelpTarget(List<Weapon> weapons)
    {
        weapons.ForEach(weapon => HelpTarget(weapon));
    }
}
