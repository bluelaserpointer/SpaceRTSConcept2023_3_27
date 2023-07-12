using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShipPlayer : ShipBrain
{
    [Header("Test")]
    public Camp camp;
    [SerializeField]
    ShipUnit _testUnit;

    [Header("Reference")]
    [SerializeField]
    Camera _camera;
    [SerializeField]
    ExponentialCameraFOVController _fovController;
    [SerializeField]
    Transform _cameraRotationRoot;
    [SerializeField]
    Transform _rotationRoot;
    [SerializeField]
    Transform _xRotationRoot;
    [SerializeField]
    Transform _yRotationRoot;
    [SerializeField]
    ShipPlayUI _shipPlayUI;

    [Header("Control Guide")]
    [SerializeField]
    Transform _controlGuideRoot;
    [SerializeField]
    float _controlGuideSize;
    [SerializeField]
    float _controlGuideDisplayBaseDistance;
    [SerializeField]
    Transform _zRotationGuideRoot;
    [SerializeField]
    Transform _zRotationTextRoot;
    [SerializeField]
    TextMeshPro _upText, _downText, _leftText, _rightText;
    [SerializeField]
    RotateDirectionIndicator _xRotateDirectionIndicator, _yRotateDirectionIndicator, _zRotateDirectionIndicator;
    [SerializeField]
    Transform _xRotateGearIndicator, _yRotateGearIndicator, _zRotateGearIndicator;
    [SerializeField]
    List<GameObject> _keyRotationModeGuides, _keyMovementModeGuides;
    [SerializeField]
    Transform _modelForwardMarker, _cameraForwardMarker;
    [SerializeField]
    Sprite _keyRotationModeSprite, _mouseRotationModeSprite;
    [SerializeField]
    SpriteRenderer _xDirectionMarker, _yDirectionMarker;
    [SerializeField]
    GameObject _dockSelectGuide;

    public override Camp Camp => camp;
    public Camera Camera => _camera;
    public ShipPlayUI UI => _shipPlayUI;

    //internal
    [HideInInspector]
    public ClampedInt thrustGear;
    bool KeyMovementMode
    {
        get => _keyMovementMode;
        set
        {
            _keyMovementMode = value;
            _keyMovementModeGuides.ForEach(each => each.SetActive(value));
            _keyRotationModeGuides.ForEach(each => each.SetActive(!value));
            _xDirectionMarker.sprite = _yDirectionMarker.sprite = value ? _mouseRotationModeSprite : _keyRotationModeSprite;
        }
    }
    bool _keyMovementMode;
    Vector3Int _previousInputSign;
    public int ActiveWeaponIndex {
        get => _activeWeaponIndex;
        set
        {
            _activeWeaponIndex = value;
            WorldManager.WeaponAimSystem.SetWeapons(ActiveWeapons);
        }
    }
    int _activeWeaponIndex;
    [HideInInspector]
    public List<Weapon> ActiveWeapons => (ActiveWeaponIndex < keyBindWeapons.Count) ? keyBindWeapons[ActiveWeaponIndex] : new List<Weapon>();
    [HideInInspector]
    public readonly List<List<Weapon>> keyBindWeapons = new List<List<Weapon>>();
    public float AimDistance { get; private set; }
    DockPort _selectingDockPort;
    public bool IsDocked => OperatingShip.IsDocked;
    public Transform NavigateTarget { get; private set; }
    public bool Navigating => NavigateTarget != null;

    void Start()
    {
        OperatingShip = _testUnit;
        OperatingShip.shipBrain = this;
        WorldManager.WeaponAimSystem.SetUnit(OperatingShip);
        keyBindWeapons.Add(new List<Weapon>(new Weapon[] { OperatingShip.weapons[0] }));
        keyBindWeapons.Add(new List<Weapon>(new Weapon[] { OperatingShip.weapons[1] }));
        thrustGear = new ClampedInt(-1, 3);
        thrustGear.onValueChange.AddListener(newValue => UI.UpdateDisplay());
        KeyMovementMode = false;
        ActiveWeaponIndex = 0;
        AimDistance = 50;
    }
    void Update()
    {
        if (OperatingShip == null)
            return;
        //hide controlling unit when zoomed
        if (Camera.GetComponent<ExponentialCameraFOVController>().AppliedChangeRatio < 1)
            OperatingShip.visibility = ShipUnit.Visibility.Hide;
        else
            OperatingShip.visibility = ShipUnit.Visibility.Normal;
        //rotational components
        _rotationRoot.rotation = OperatingShip.Rigidbody.rotation;
        //camera repositioning & rotationing
        transform.position = OperatingShip.transform.position;
        _cameraRotationRoot.rotation = Camera.transform.rotation;
        Vector3 tmpVec;
        if((tmpVec = OperatingShip.transform.forward.Set(y: 0)).sqrMagnitude > 0.01F)
            _xRotationRoot.rotation = Quaternion.LookRotation(tmpVec);
        if ((tmpVec = Camera.transform.forward.Set(y: 0)).sqrMagnitude > 0.01F)
            _yRotationRoot.rotation = Quaternion.LookRotation(tmpVec);
        _zRotationGuideRoot.rotation = Quaternion.LookRotation(OperatingShip.Rigidbody.transform.forward);
        //control guide rotationing & resizing
        _controlGuideRoot.localScale = Vector3.one * OperatingShip.testRadius / _controlGuideSize * Vector3.Distance(Camera.transform.position, _controlGuideRoot.position) / _controlGuideDisplayBaseDistance * Camera.GetComponent<ExponentialCameraFOVController>().AppliedChangeRatio;
        Vector3 toCameraVec = Camera.transform.TransformDirection(Camera.transform.InverseTransformDirection(Camera.transform.position - transform.position).Set(z: 0));
        _controlGuideRoot.localPosition = toCameraVec * (1 - _fovController.AppliedChangeRatio);
        //input operations
        UpdateMoveOperation();
        UpdateWeaponOperation();
        UpdateDockOperation();
    }
    private void UpdateMoveOperation()
    {
        if (IsDocked)
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                OperatingShip.LeaveDock();
                thrustGear.Value = 1;
            }
            return;
        }
        if (Navigating)
        {
            OperatingShip.MoveTowards(NavigateTarget.position, 5);
            OperatingShip.RotateTowards(NavigateTarget.rotation);
            if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.B)
                || Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
            {
                NavigateTarget = null;
                if (OperatingShip.AssignedDockPort != null)
                {
                    OperatingShip.LeaveDock();
                }
            }
            else
            {
                return;
            }
        }
        //engine gear change
        if (Input.GetKeyDown(KeyCode.Z))
            --thrustGear.Value;
        else if (Input.GetKeyDown(KeyCode.C))
        {
            if (++thrustGear.Value == 0)
                KeyMovementMode = false;
        }
        else if (Input.GetKeyDown(KeyCode.B))
            thrustGear.Value = -1;
        if (Input.GetKeyDown(KeyCode.LeftAlt)) //key movement / rotation mode change
        {
            if (thrustGear.Value == -1)
            {
                thrustGear.Value = 0;
                KeyMovementMode = false;
            }
            else
                KeyMovementMode = !KeyMovementMode;
        }
        if (thrustGear.Value == -1)
            KeyMovementMode = true;
        //rotation
        float xInput = Input.GetAxisRaw("Horizontal");
        float yInput = Input.GetAxisRaw("Vertical");
        float rollInput = Input.GetAxisRaw("Roll");
        float elevationInput = Input.GetAxisRaw("Elevation");
        if (KeyMovementMode)
        {
            OperatingShip.RotateTowards(Camera.transform.rotation);
        }
        else //keyRotationMode
        {
            if (xInput == 0)
            {
                _previousInputSign.x = 0;
                _yRotateDirectionIndicator.gameObject.SetActive(false);
                _leftText.gameObject.SetActive(true);
                _rightText.gameObject.SetActive(true);
            }
            else
            {
                int inputSign = xInput > 0 ? 1 : -1;
                if (_previousInputSign.x != inputSign)
                {
                    _previousInputSign.x = inputSign;
                }
                _yRotateDirectionIndicator.gameObject.SetActive(true);
                _yRotateDirectionIndicator.SetRotateDirection(xInput > 0);
                _leftText.gameObject.SetActive(false);
                _rightText.gameObject.SetActive(false);
            }
            if (yInput == 0)
            {
                _previousInputSign.y = 0;
                _xRotateDirectionIndicator.gameObject.SetActive(false);
                _upText.gameObject.SetActive(true);
                _downText.gameObject.SetActive(true);
            }
            else
            {
                int inputSign = yInput > 0 ? 1 : -1;
                if (_previousInputSign.y != inputSign)
                {
                    _previousInputSign.y = inputSign;
                }
                _xRotateDirectionIndicator.gameObject.SetActive(true);
                _xRotateDirectionIndicator.SetRotateDirection(yInput > 0);
                _upText.gameObject.SetActive(false);
                _downText.gameObject.SetActive(false);
            }
            if (rollInput == 0)
            {
                _zRotateDirectionIndicator.gameObject.SetActive(false);
                _zRotationTextRoot.gameObject.SetActive(true);
            }
            else
            {
                _zRotateDirectionIndicator.gameObject.SetActive(true);
                _zRotateDirectionIndicator.SetRotateDirection(rollInput > 0);
                _zRotationTextRoot.gameObject.SetActive(false);
            }
            OperatingShip.RotationInput = new Vector3(-yInput, xInput, -rollInput);
        }
        //mouse rotation indicator
        /*
        if (KeyMovementMode)
        {
            Quaternion midPointRotation = Quaternion.Lerp(ControllingShip.transform.rotation, Camera.transform.rotation, 0.5F);
            Vector3 midPoint = transform.position + midPointRotation * Vector3.forward * _modelForwardMarker.localPosition.z;
            Plane diskPlane = new Plane(_modelForwardMarker.position, midPoint, _cameraForwardMarker.position);
            _mouseRotationIndicator.transform.up = diskPlane.normal;
        }
        */
        //movement
        if (thrustGear.Value == -1) //Hover
        {
            if (xInput == 0 && yInput == 0 && elevationInput == 0)
                OperatingShip.brakeMode = true;
            else
            {
                OperatingShip.brakeMode = false;
                OperatingShip.RelativeMovementInput = new Vector3(xInput, elevationInput, yInput);
            }
        }
        else //Jet
        {
            OperatingShip.brakeMode = false;
            Vector3 relativeMovementInput = new Vector3(KeyMovementMode ? xInput : 0, elevationInput, (float)thrustGear.Value / (thrustGear.Value < 0 ? -thrustGear.min : thrustGear.max));
            //negate velocity of unwanted directions
            if (!OperatingShip.brakeMode)
            {
                Vector3 velocity = OperatingShip.Rigidbody.velocity;
                Vector3 UnwantedVelocity = velocity - Vector3.Project(velocity, OperatingShip.transform.forward);
                Vector3 CorrectionInput = -UnwantedVelocity / OperatingShip.EngineTopAccel;
                if (CorrectionInput.sqrMagnitude > 1)
                {
                    OperatingShip.GlobalMovementInput = CorrectionInput.normalized;
                    OperatingShip.EnginePowerRatioInput = 1;
                }
                else
                {
                    OperatingShip.GlobalMovementInput = CorrectionInput + OperatingShip.transform.TransformDirection(relativeMovementInput) * Mathf.Sqrt(1 - CorrectionInput.sqrMagnitude);
                    OperatingShip.EnginePowerRatioInput = 1;
                }
            }
        }
    }
    private void UpdateWeaponOperation()
    {
        //weapon select
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ActiveWeaponIndex = 0;
            UI.UpdateDisplay();
            KeyMovementMode = ActiveWeapons[0].Coaxial;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ActiveWeaponIndex = 1;
            UI.UpdateDisplay();
            KeyMovementMode = ActiveWeapons[0].Coaxial;
        }
        if (IsDocked)
            return;
        //weapon aim
        WorldManager.WeaponAimSystem.UpdateAim();
        //weapon trigger
        if (Input.GetMouseButton(0))
        {
            foreach (Weapon weapon in ActiveWeapons)
            {
                weapon.Trigger();
            }
        }
    }
    private void UpdateDockOperation()
    {
        if (IsDocked)
        {
            _selectingDockPort = null;
        }
        else
        {
            if (OperatingShip.AssignedDockPort != null && Vector3.Distance(OperatingShip.transform.position, OperatingShip.AssignedDockPort.transform.position) < 1)
            {
                OperatingShip.Dock();
            }
            else
            {
                _selectingDockPort = SelectDockPortByCameraRay();
            }
        }
        if (_selectingDockPort == null)
        {
            _dockSelectGuide.gameObject.SetActive(false);
        }
        else
        {
            _dockSelectGuide.gameObject.SetActive(true);
            _dockSelectGuide.transform.SetPositionAndRotation(_selectingDockPort.transform.position, _selectingDockPort.transform.rotation);
            if (Input.GetKeyDown(KeyCode.E))
            {
                if(OperatingShip.PrepareDock(_selectingDockPort))
                {
                    thrustGear.Value = thrustGear.min;
                    NavigateTarget = _selectingDockPort.transform;
                }
            }
        }
    }
    private DockPort SelectDockPortByCameraRay()
    {
        Ray cameraRay = new Ray(Camera.transform.position, Camera.transform.forward);
        float minDistance = float.MaxValue;
        Dock closestDock = null;
        foreach (var hitInfo in Physics.RaycastAll(cameraRay, 1000, LayerMask.GetMask("Dock")))
        {
            if(hitInfo.distance < minDistance)
            {
                minDistance = hitInfo.distance;
                closestDock = hitInfo.collider.GetComponent<Dock>();
            }
        }
        return closestDock?.SuggestPortByRaySelect(cameraRay, 1000);
    }
    public override void OnControlShipChange(ShipUnit oldShip, ShipUnit newShip)
    {
        thrustGear.Value = 0;
        UI.UpdateDisplay();
    }
}
