using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
public class WorldManager : MonoBehaviour
{
    [SerializeField]
    ShipPlayer _shipPlayer;
    [SerializeField]
    WeaponAimSystem _weaponAimSystem;

    public static WorldManager Instance { get; private set; }
    public static ShipPlayer Player => Instance._shipPlayer;
    public static WeaponAimSystem WeaponAimSystem => Instance._weaponAimSystem;
    public enum MissionProgressState { Playing, Success, Fail }
    public MissionProgressState MissionProgress
    {
        get => _missionProgress;
        set
        {
            _missionProgress = value;
            if (_missionProgress != MissionProgressState.Playing)
            {
                GameUI.Instance.OnStageFinish(_missionProgress == MissionProgressState.Success);
            }
        }
    }
    MissionProgressState _missionProgress;

    private void Awake()
    {
        Instance = this;
        MissionProgress = MissionProgressState.Playing;
    }
    private void Update()
    {
        if (MissionProgress == MissionProgressState.Playing)
        {
            if (GetTargetableEnemies(Player.Camp).Count == 0)
            {
                MissionProgress = MissionProgressState.Success;
            }
            else if (Player.OperatingShip == null)
            {
                MissionProgress = MissionProgressState.Fail;
            }
        }
        else
        {
            if (Input.GetKey(KeyCode.R))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
        if (Input.GetKey(KeyCode.Escape))
            Application.Quit();
    }
    public List<Unit> GetTargetableUnits(Predicate<Unit> predicate = null)
    {
        return GameObjectExtension.FindComponentsWithTag<Unit>(Unit.Tag, unit => (predicate?.Invoke(unit) ?? true) && unit.gameObject.activeInHierarchy);
    }
    public List<Unit> GetTargetableEnemies(Camp myCamp)
    {
        return GetTargetableUnits(unit => unit.IsEnemy(myCamp));
    }
}
