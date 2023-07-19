using IzumiTools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitMarker : ReusableAttachment<Unit>
{
    [Header("Preference")]
    [SerializeField]
    Vector2 _distanceDisplayRange = new Vector2(1000, 5000);

    [Header("Reference")]
    [SerializeField]
    GameObject visibleRoot;
    [SerializeField]
    Text unitNameText;
    [SerializeField]
    Text distanceText;

    public override Unit MarkTarget
    {
        get => _markTarget;
        set
        {
            _markTarget = value;
            unitNameText.text = value.displayName;
        }
    }
    Unit _markTarget;

    private void LateUpdate()
    {
        if (_markTarget == null)
        {
            gameObject.SetActive(false);
            return;
        }
        WorldPositionMarker.Mark(visibleRoot, WorldManager.Player.Camera, MarkTarget.transform.position);
        if (WorldManager.Player.OperatingShip.IsDead)
        {
            distanceText.enabled = false;
        }
        else
        {
            float distance = Vector3.Distance(WorldManager.Player.OperatingShip.transform.position, MarkTarget.transform.position);
            if (distance > _distanceDisplayRange.x && distance < _distanceDisplayRange.y)
            {
                distanceText.enabled = true;
                distanceText.text = string.Format("{0:F1}", distance / 1000) + "km";
            }
            else
            {
                distanceText.enabled = false;
            }
        }
    }
}
