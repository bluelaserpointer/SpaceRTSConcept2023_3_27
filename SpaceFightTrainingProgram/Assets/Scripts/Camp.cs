using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Camp")]
public class Camp : ScriptableObject
{
    [SerializeField]
    string _name;
    [SerializeField]
    Color _baseColor;

    public string Name => _name;
    public Color BaseColor => _baseColor;
}
