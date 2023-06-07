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
    new Camera camera;
    Unit _targetUnit;
    public void Init(Camera camera, Unit userUnit)
    {
        this.camera = camera;
        UserUnit = userUnit;
        effectiveAreaRect.rectTransform.sizeDelta = Vector2.one * effectiveAreaPx;
    }
    public void Prepare()
    {
        _targetMarkerNest.InactivateAll();
        if (WorldManager.Instance.TryGetNearstEnemy(UserUnit, out Unit enemy))
        {
            _targetUnit = enemy;
        }
    }
    public void HelpTarget(Weapon weapon)
    {
        if (_targetUnit == null)
            return;
        weapon.DeflectAim(_targetUnit);
        Transform markerTf = _targetMarkerNest.Get();
        markerTf.transform.position = camera.WorldToScreenPoint(_targetUnit.transform.position);
    }
    public void HelpTarget(List<Weapon> weapons)
    {
        weapons.ForEach(weapon => HelpTarget(weapon));
    }
}
