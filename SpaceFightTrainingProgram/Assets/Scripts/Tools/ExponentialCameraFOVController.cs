using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Camera))]
public class ExponentialCameraFOVController : MonoBehaviour
{
    [SerializeField] Cinemachine.CinemachineFreeLook _freeLook;
    public float baseFOV = 60;
    public float pow = 2;
    public ClampedInt level = new ClampedInt(0, 5);
    public Camera Camera { get; private set; }
    public float AppliedChangeRatio => Mathf.Pow(2, -level.Value);
    private void Awake()
    {
        Camera = GetComponent<Camera>();
    }
    void Update()
    {
        float wheelInput = Input.GetAxis("Mouse ScrollWheel");
        int fovChange = 0;
        if (wheelInput > 0) {
            fovChange = 1;
        }
        else if (wheelInput < 0)
        {
            fovChange = -1;
        }
        if (wheelInput != 0)
        {
            level.Value += fovChange;
            SetFieldOfView(baseFOV* AppliedChangeRatio);
        }
    }
    private void SetFieldOfView(float value)
    {
        if (_freeLook != null)
        {
            _freeLook.m_Lens.FieldOfView = value;
        }
        else
        {
            Camera.fieldOfView = value;
        }
    }
}
