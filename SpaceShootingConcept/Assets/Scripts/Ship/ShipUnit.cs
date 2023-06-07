using IzumiTools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ShipUnit : Unit
{
    public ShipBrain shipBrain;
    public float testRadius;
    public Visibility visibility;
    [SerializeField]
    ShipMobility _mobility;

    [System.Serializable]
    struct ShipMobility
    {
        public float main;
        public float sub;
        public float rotateForce;
        public Vector3 Output(Vector3 movementInput, bool useMainEngine)
        {
            movementInput.Normalize();
            movementInput = movementInput.normalized * CurrentForse(useMainEngine);
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

    [HideInInspector]
    public bool useMainEngine;

    [HideInInspector]
    public Vector3 MovementInput
    {
        get => _movementInput;
        set
        {
            _movementInput = value;
            _brakeMode = false;
        }
    }
    [HideInInspector]
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
    public bool BrakeMode {
        get => _brakeMode;
        set
        {
            _brakeMode = value;
        }
    }
    public bool FaceDirectionMode => _faceRotationMode;
    [HideInInspector]
    public Vector3 aimPosition;

    Vector3 _movementInput;
    Vector3 _rotationInput;
    bool _brakeMode;
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
            shipBrain.OperatingShip = this;
    }
    private void LateUpdate()
    {
        foreach (var renderer in GetComponentsInChildren<Renderer>()) {
            renderer.enabled = visibility != Visibility.Hide;
        }
    }
    private void FixedUpdate()
    {
        if (_brakeMode)
        {
            Rigidbody.velocity = Vector3.MoveTowards(Rigidbody.velocity, Vector3.zero, _mobility.CurrentForse(useMainEngine) / Rigidbody.mass * Time.fixedDeltaTime);
        }
        else
        {
            Rigidbody.AddForce(transform.TransformVector(_mobility.Output(MovementInput, useMainEngine)), ForceMode.Acceleration);
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
    public void Brake()
    {
        BrakeMode = true;
    }
    public void FaceRotation(Quaternion targetRotation)
    {
        _faceRotationMode = true;
        _targetRotation = targetRotation;
    }
}
