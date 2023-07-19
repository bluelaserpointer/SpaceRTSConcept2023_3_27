using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class BulletRemainUI : MonoBehaviour
{
    [SerializeField]
    WeaponAimSystem weaponAimSystem;
    [SerializeField]
    GameObject shellObj;
    [SerializeField]
    float shellAmount;
    [SerializeField]
    Transform shellRound;

    float angleGap;

    private void Awake()
    {
        angleGap = 360 / shellAmount;
        for (int i = 1; i < shellAmount; i++)
        {
            GameObject shell = Instantiate(shellObj, shellObj.transform.parent);
            shell.transform.eulerAngles = Vector3.forward * angleGap * i;
        }
    }
    private void Update()
    {
        shellRound.transform.eulerAngles = Vector3.forward * ((ShipGun)weaponAimSystem.weapons[0]).FireCD.Ratio * angleGap;
    }
}
