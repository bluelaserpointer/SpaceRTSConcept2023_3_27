using IzumiTools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[DisallowMultipleComponent]
public class TestCarrierAI : ShipBrain
{
    [Header("Ability")]
    [SerializeField]
    Transform _shipStockRoot;
    [SerializeField]
    int _initialStock;
    [SerializeField]
    ShipUnit _defaultStockShipPrefab;
    [SerializeField]
    int _maxStock;
    [SerializeField]
    Dock dock;
    [SerializeField]
    float _shipReleaseSpeed;


    [Header("Tactics")]
    [SerializeField]
    float _attackDistance;
    [SerializeField]
    float _safeStanbyDistance;
    public PlanState Plan
    {
        get => _plan;
        set
        {
            switch(_plan = value)
            {
                case PlanState.Idle:
                    BroadcastToBelongingShips(new UnitRequest.DockRequest(dock));
                    break;
                case PlanState.Attack:
                    BroadcastToBelongingShips(new UnitRequest.LeaveDockRequest());
                    break;
            }
        }
    }
    PlanState _plan;
    public enum PlanState { Idle, Aleart, Attack }
    public readonly List<ShipUnit> belongingShips = new List<ShipUnit>();
    public readonly List<ShipUnit> stockedShips = new List<ShipUnit>();
    public readonly List<ShipUnit> releasingShips = new List<ShipUnit>();

    private void Awake()
    {
        Plan = PlanState.Idle;
    }
    private void Start()
    {
        for (int i = 0; i < _initialStock; ++i)
        {
            ShipUnit ship = Instantiate(_defaultStockShipPrefab, _shipStockRoot);
            ship.Brain.Camp = Camp;
            StockShip(ship);
        }
    }
    private void Update()
    {
        foreach (ShipUnit ship in belongingShips.ToArray())
        {
            if (ship == null)
            {
                belongingShips.Remove(ship);
            }
        }
        UpdatePlanOperation();
        UpdateDockOperation();
    }
    private void UpdateDockOperation()
    {
        foreach (ShipUnit ship in releasingShips.ToArray())
        {
            if (ship == null)
            {
                releasingShips.Remove(ship);
                continue;
            }
            ship.transform.position = Vector3.MoveTowards(ship.transform.position, ship.AssignedDockPort.transform.position, _shipReleaseSpeed * Time.deltaTime);
            if (Vector3.Distance(ship.transform.position, ship.AssignedDockPort.transform.position) < 0.1F)
            {
                ship.Dock();
                releasingShips.Remove(ship);
                ship.IsInert = false;   
                ship.transform.SetParent(WorldManager.Instance.transform, true);
            }
        }
    }
    private void UpdatePlanOperation()
    {
        List<Unit> unitsInAttackRange = WorldManager.Instance.GetTargetableUnits(unit => unit.IsEnemy(this));
        Unit closestEnemy = unitsInAttackRange.FindMin(unit => Vector3.Distance(unit.transform.position, OperatingShip.transform.position), out float closestEnemyDistance);
        if (Plan == PlanState.Idle)
        {
            if (unitsInAttackRange.Count > 0)
            {
                Plan = PlanState.Attack;
                return;
            }
        }
        else if (Plan == PlanState.Attack)
        {
            if (unitsInAttackRange.Count == 0)
            {
                Plan = PlanState.Idle;
                return;
            }
            StartReleaseAnyShip();
        }
    }
    public bool StockShip(ShipUnit ship)
    {
        if (stockedShips.Count >= _maxStock)
        {
            return false;
        }
        stockedShips.Add(ship);
        ship.IsInert = true;
        ship.gameObject.SetActive(false);
        ship.transform.SetParent(_shipStockRoot);
        ship.transform.localPosition = Vector3.zero;
        ship.transform.localRotation = Quaternion.identity;
        return true;
    }
    public ShipUnit StartReleaseAnyShip()
    {
        if (stockedShips.Count == 0)
        {
            return null;
        }
        DockPort port = dock.SuggestPort();
        if (port == null)
        {
            return null;
        }
        ShipUnit ship = stockedShips[0];
        ship.gameObject.SetActive(true);
        ship.AssignDockPort(port);
        stockedShips.Remove(ship);
        releasingShips.Add(ship);
        return ship;
    }
    public void BroadcastToBelongingShips(UnitRequest request)
    {
        belongingShips.ForEach(ship => ship.Request(request));
    }
    public override void Request(UnitRequest request)
    {
    }
}
