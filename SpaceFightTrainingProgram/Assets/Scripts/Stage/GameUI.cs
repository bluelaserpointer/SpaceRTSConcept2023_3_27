using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class GameUI : MonoBehaviour
{
    public static GameUI Instance { get; private set; }
    [SerializeField]
    GameObject _missionResultGroup;
    [SerializeField]
    GameObject _missionSuccessGroup;
    [SerializeField]
    GameObject _missionFailGroup;

    private void Awake()
    {
        Instance = this;
        HideMissionResultGroup();
    }
    public void HideMissionResultGroup()
    {
        _missionResultGroup.SetActive(false);
    }
    public void OnStageFinish(bool success)
    {
        _missionResultGroup.SetActive(true);
        if (success)
        {
            _missionSuccessGroup.SetActive(true);
            _missionFailGroup.SetActive(false);
        }
        else
        {
            _missionSuccessGroup.SetActive(false);
            _missionFailGroup.SetActive(true);
        }
    }
}
