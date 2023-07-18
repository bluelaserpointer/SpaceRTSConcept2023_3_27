using IzumiTools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitMarker : ReusableAttachment<Unit>
{
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
            unitNameText.text = value.name;
        }
    }
    Unit _markTarget;

    private void Update()
    {
        WorldPositionMarker.Mark(visibleRoot, WorldManager.Player.Camera, MarkTarget.transform.position);
        float distance = Vector3.Distance(WorldManager.Player.OperatingShip.transform.position, MarkTarget.transform.position);
        distanceText.text = distance + "m";
    }
}
