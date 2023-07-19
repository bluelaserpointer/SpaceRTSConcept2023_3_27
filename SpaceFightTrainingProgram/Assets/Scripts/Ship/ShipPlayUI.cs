using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class ShipPlayUI : MonoBehaviour
{
    [Header("Test")]
    [SerializeField]
    ShipPlayer _shipPlayer;

    [Header("Reference")]
    [SerializeField]
    TextMeshProUGUI _speedText;
    [SerializeField]
    Transform _gearDisplayRoot;
    [SerializeField]
    List<Color> _gearButtonLitColors;

    public void UpdateDisplay()
    {
        if (_shipPlayer.OperatingUnit == null)
            return;
        //engine gear
        for (int childIndex = 0; childIndex < _gearDisplayRoot.childCount; ++childIndex)
        {
            bool lit = childIndex == _shipPlayer.thrustGear.ValueIndex;
            Transform gearIconTf = _gearDisplayRoot.GetChild(childIndex);
            gearIconTf.GetComponentInChildren<Image>().color = lit ? _gearButtonLitColors[childIndex] : Color.white;
            gearIconTf.GetComponentInChildren<TextMeshProUGUI>().color = lit ? Color.black : Color.white;
        }
    }
    private void Update()
    {
        if (_shipPlayer.OperatingUnit == null)
            return;
        _speedText.text = string.Format("{0:F1}", _shipPlayer.OperatingUnit.Rigidbody.velocity.magnitude) + "\r\nkm/s";
    }
}
