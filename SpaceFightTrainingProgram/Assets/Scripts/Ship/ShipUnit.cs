using IzumiTools;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
public class ShipUnit : Unit
{
    public ShipBrain shipBrain;
    public float testRadius;
    public Visibility visibility;
    [SerializeField]
    public CappedValue health;
    [SerializeField]
    ShipMobility _mobility;
    [SerializeField]
    [Range(0f, 1f)]
    float _velocityRotateRatio;
    [SerializeField]
    GameObject _destructedPrefab;
    public UnityEvent<UnitEffectFeedback> OnEffect = new UnityEvent<UnitEffectFeedback>();

    public override bool IsDead => health.IsEmpty;
    public DockPort AssignedDockPort { get; private set; }
    public bool IsDocked { get; private set; }

    [System.Serializable]
    public struct ShipMobility
    {
        public float main;
        public float sub;
        public float rotateForce;
        public Vector3 Output(Vector3 movementInput, bool useMainEngine, float powerRatio)
        {
            movementInput = movementInput * CurrentForse(useMainEngine) * powerRatio;
            return movementInput;
        }
        public float CurrentForse(bool useMainEngine)
        {
            return useMainEngine ? main : sub;
        }
    }
    public enum Visibility { Normal, Hide }

    public override UnitBrain Brain => shipBrain;
    public readonly List<UnitDamageCollider> parts = new List<UnitDamageCollider>();

    public ShipMobility Mobility => _mobility;
    public float EngineForce => _mobility.CurrentForse(useMainEngine);
    public float EngineTopAccel => EngineForce / Rigidbody.mass;

    [HideInInspector]
    public bool useMainEngine;

    public Vector3 RelativeMovementInput
    {
        get => _relativeMovementInput;
        set
        {
            _relativeMovementInput = value.normalized;
            brakeMode = false;
            _autoMoveMode = false;
        }
    }
    public Vector3 GlobalMovementInput
    {
        get => transform.TransformDirection(_relativeMovementInput);
        set => RelativeMovementInput = transform.InverseTransformDirection(value);
    }
    public float EnginePowerRatioInput
    {
        get => _enginePowerRatioInput;
        set
        {
            _enginePowerRatioInput = Mathf.Clamp(value, 0, 1);
            brakeMode = false;
            _autoMoveMode = false;
        }
    }
    public Vector3 RotationInput
    {
        get => _rotationInput;
        set
        {
            _rotationInput = value.normalized;
            _autoRotationMode = false;
        }
    }
    [HideInInspector]
    public bool brakeMode;
    public bool FaceDirectionMode => _autoRotationMode;

    Vector3 _relativeMovementInput;
    float _enginePowerRatioInput = 1;
    Vector3 _rotationInput;
    bool _autoMoveMode;
    Vector3 _autoMoveTarget;
    float _autoMoveBrakeDistance;
    bool _autoRotationMode;
    Quaternion _autoRotateTarget;

    private void Awake()
    {
        Rigidbody = GetComponent<Rigidbody>();
        health.Maximize();
        LinkAllModules();
        foreach (var part in GetComponentsInChildren<UnitDamageCollider>())
        {
            part.Init(this);
            parts.Add(part);
        }
        if(shipBrain != null)
        {
            shipBrain.OperatingShip = this;
        }
    }
    private void LateUpdate()
    {
        foreach (var renderer in GetComponentsInChildren<Renderer>()) {
            renderer.enabled = visibility != Visibility.Hide;
        }
    }
    private void FixedUpdate()
    {
        if (_autoMoveMode)
        {
            Vector3 dstDelta = _autoMoveTarget - transform.position;
            Vector3 dstDirection = dstDelta.normalized;
            float distance = Vector3.Distance(_autoMoveTarget, transform.position);
            if (distance > _autoMoveBrakeDistance)
            {
                brakeMode = false;
                Vector3 velocity = Rigidbody.velocity;
                Vector3 orientedVelocity = Vector3.Project(velocity, dstDirection);
                Vector3 disorientedVelocity = velocity - orientedVelocity;
                Vector3 correctionInput = -disorientedVelocity / EngineTopAccel;
                if (correctionInput.sqrMagnitude > 1)
                {
                    //print("correct " + name + ", " + CorrectionInput.sqrMagnitude);
                    GlobalMovementInput = correctionInput.normalized;
                    EnginePowerRatioInput = 1;
                }
                else
                {
                    //print("push " + name + ", " + CorrectionInput.sqrMagnitude);
                    float moveTowardsInput = (ExtendedMath.OptimalVelocityStopAt(distance, EngineTopAccel) - orientedVelocity.magnitude) / EngineTopAccel;
                    if (moveTowardsInput > 1)
                    {
                        GlobalMovementInput = correctionInput + dstDirection * Mathf.Sqrt(1 - correctionInput.sqrMagnitude);
                    }
                    else
                    {
                        Vector3 combinedInput = correctionInput + moveTowardsInput * dstDirection;
                        float combinedInputMagnitude = combinedInput.magnitude;
                        GlobalMovementInput = combinedInput.normalized;
                        EnginePowerRatioInput = combinedInputMagnitude / 1;
                    }
                }
            }
            else
            {
                brakeMode = true;
            }
        }
        if (brakeMode)
        {
            Rigidbody.velocity = Vector3.MoveTowards(Rigidbody.velocity, Vector3.zero, EngineTopAccel * Time.fixedDeltaTime);
        }
        else
        {
            Rigidbody.AddForce(transform.TransformVector(_mobility.Output(RelativeMovementInput, useMainEngine, EnginePowerRatioInput)), ForceMode.Acceleration);
        }
        //Vector3 originalVelocity = Rigidbody.velocity;
        //Vector3.rot originalVelocity.
        float rotateForce = _mobility.rotateForce * (1 + (1 - EnginePowerRatioInput));
        if (_autoRotationMode)
        {
            if (Quaternion.Angle(Rigidbody.rotation, _autoRotateTarget) < 0.1F)
                Rigidbody.angularVelocity = Vector3.MoveTowards(Rigidbody.angularVelocity, Vector3.zero, rotateForce / Rigidbody.mass * Time.fixedDeltaTime);
            else
                Rigidbody.angularVelocity = ToRotaionByOptimalAccel.OptimalAngularVelocityTowardsRotation(Rigidbody, _autoRotateTarget, rotateForce / Rigidbody.mass);
        }
        else
        {
            Rigidbody.AddTorque(rotateForce * RotationInput, ForceMode.Acceleration);
        }
        Quaternion oldRotation = Rigidbody.rotation;
        Quaternion newRotation = oldRotation * Quaternion.Euler(Rigidbody.angularVelocity * Time.fixedDeltaTime);
        Vector3 removeVelocity = Rigidbody.velocity * _velocityRotateRatio;
        Vector3 applyVelocity = removeVelocity.magnitude * (newRotation * Vector3.forward);
        Rigidbody.velocity += -removeVelocity + applyVelocity;
    }
    public void MoveTowards(Vector3 targetPosition, float brakeDistance)
    {
        _autoMoveMode = true;
        _autoMoveTarget = targetPosition;
        _autoMoveBrakeDistance = brakeDistance;
    }
    public void RotateTowards(Quaternion targetRotation)
    {
        _autoRotationMode = true;
        _autoRotateTarget = targetRotation;
    }
    public override UnitEffectFeedback Damage(Damage damage)
    {
        UnitEffectFeedback feedback = new UnitEffectFeedback();
        health.Value -= damage.damage;
        feedback.damage = damage.damage;
        if (IsDead)
        {
            Death();
            feedback.kill = true;
        }
        OnEffect.Invoke(feedback);
        return feedback;
    }
    public override void Death()
    {
        base.Death();
        health.Value = 0;
        if(_destructedPrefab != null)
        {
            GameObject destructedObj = Instantiate(_destructedPrefab);
            destructedObj.transform.SetPositionAndRotation(transform.position, transform.rotation);
            destructedObj.AddComponent<ConstantSpeed>().move = Rigidbody.velocity;
        }
        if (AssignedDockPort != null)
            AssignedDockPort.OnLeaving(this);
        Destroy(gameObject);
    }
    public bool AssignDockPort(DockPort port)
    {
        if (AssignedDockPort != null)
        {
            LeaveDock();
        }
        if(port.Assign(this))
        {
            AssignedDockPort = port;
            return true;
        }
        return false;
    }
    public void Dock()
    {
        IsDocked = true;
        Rigidbody.constraints = RigidbodyConstraints.FreezeAll;
        AssignedDockPort.OnDock(this);
    }
    public void LeaveDock()
    {
        IsDocked = false;
        Rigidbody.constraints = RigidbodyConstraints.None;
        AssignedDockPort.OnLeaving(this);
        AssignedDockPort = null;
    }
}
