using IzumiTools;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ShipUnit : Unit
{
    public ShipBrain shipBrain;
    public float testRadius;
    public Visibility visibility;
    [SerializeField]
    public float health;
    [SerializeField]
    ShipMobility _mobility;
    [SerializeField]
    GameObject _destructedPrefab;

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
            movementInput = movementInput.normalized * CurrentForse(useMainEngine) * powerRatio;
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
            _relativeMovementInput = value;
            brakeMode = false;
        }
    }
    public Vector3 GlobalMovementInput
    {
        get => transform.TransformDirection(_relativeMovementInput);
        set
        {
            _relativeMovementInput = transform.InverseTransformDirection(value);
            brakeMode = false;
        }
    }
    public float EnginePowerRatioInput
    {
        get => _enginePowerRatioInput;
        set
        {
            _enginePowerRatioInput = Mathf.Clamp(value, 0, 1);
            brakeMode = false;
        }
    }
    public Vector3 RotationInput
    {
        get => _rotationInput;
        set
        {
            _rotationInput = value;
            _faceRotationMode = false;
        }
    }
    [HideInInspector]
    public bool brakeMode;
    public bool FaceDirectionMode => _faceRotationMode;

    Vector3 _relativeMovementInput;
    float _enginePowerRatioInput = 1;
    Vector3 _rotationInput;
    bool _faceRotationMode;
    Quaternion _targetRotation;

    private void Awake()
    {
        Rigidbody = GetComponent<Rigidbody>();
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
    {;
        if (brakeMode)
        {
            Rigidbody.velocity = Vector3.MoveTowards(Rigidbody.velocity, Vector3.zero, EngineTopAccel * Time.fixedDeltaTime);
        }
        else
        {
            Rigidbody.AddForce(transform.TransformVector(_mobility.Output(RelativeMovementInput, useMainEngine, EnginePowerRatioInput)), ForceMode.Acceleration);
        }
        if (_faceRotationMode)
        {
            Rigidbody.angularVelocity = ToRotaionByOptimalAccel.OptimalAngularVelocityTowardsRotation(Rigidbody, _targetRotation, _mobility.rotateForce / Rigidbody.mass);
        }
        else
        {
            Vector3 xyRotation = Vector3.Cross(Vector3.up, transform.forward) * RotationInput.x + Vector3.up * RotationInput.y;
            Rigidbody.AddTorque(_mobility.rotateForce * xyRotation.normalized, ForceMode.Acceleration);
        }
        //transform.Rotate(_mobility.rotateSpeed * Vector3.forward * rotationInput.z);
    }
    public void MoveTowards(Vector3 targetPosition, float brakeDistance)
    {
        Vector3 dstDelta = targetPosition - transform.position;
        Vector3 dstDirection = dstDelta.normalized;
        float distance = Vector3.Distance(targetPosition, transform.position);
        if (distance > brakeDistance)
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
                //

            }
        }
        else
        {
            brakeMode = true;
        }
    }
    public void RotateTowards(Quaternion targetRotation)
    {
        _faceRotationMode = true;
        _targetRotation = targetRotation;
    }
    public override UnitEffectFeedback Damage(Damage damage)
    {
        UnitEffectFeedback feedback = new UnitEffectFeedback();
        health -= damage.damage;
        feedback.damage = damage.damage;
        if (health < 0)
        {
            Death();
            feedback.kill = true;
        }
        return feedback;
    }
    public override void Death()
    {
        base.Death();
        if(_destructedPrefab != null)
            Instantiate(_destructedPrefab).transform.SetPositionAndRotation(transform.position, transform.rotation);
        Destroy(gameObject);
    }
    public bool PrepareDock(DockPort port)
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
