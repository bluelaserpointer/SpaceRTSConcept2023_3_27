using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IzumiTools
{
    /// <summary>
    /// Controls camera look direction and fieldOfView by mouse, and manages cursor lock.
    /// </summary>
    [DisallowMultipleComponent]
    public class MouseControllableCamera : MonoBehaviour
    {
        //inspector
        [Header("Test")]
        public Rigidbody chaseTarget;

        [Header("Sensitivity")]
        [SerializeField]
        float sensitivityMouse = 2f;
        [SerializeField]
        ExponentialCameraFOVController _fovController;
        [SerializeField]
        float _fovAffectSensitivityRatio = 1.0f;

        [Header("Option")]
        [SerializeField]
        bool limitXAngleBetweenPoles = true;
        [SerializeField]
        bool lockCursorOnAwake = true;

        [Header("Reference")]
        [SerializeField]
        new Camera camera;
        [SerializeField]
        Transform xAxis, yAxis;
        //
        public Camera Camera => camera;
        public Transform XAxis => xAxis;
        public Transform YAxis => yAxis;
        Vector3 lastChaseDeltaY;
        float lastChaseDeltaXAng;
        bool storedLastPos;
        private void Awake()
        {
            if (lockCursorOnAwake)
                LockCursor(true);
        }
        void LateUpdate()
        {
            //rotation
            float sensitivity = sensitivityMouse;
            if (_fovController != null)
                sensitivity *= _fovController.AppliedChangeRatio * _fovAffectSensitivityRatio;
            if (limitXAngleBetweenPoles)
            {
                Vector3 xAxisEularAngles = xAxis.localEulerAngles;
                xAxisEularAngles.x = ExtendedMath.RotateWithClamp(xAxisEularAngles.x, -Input.GetAxis("Mouse Y") * sensitivity, 270, 90);
                xAxis.localEulerAngles = xAxisEularAngles;
                yAxis.localEulerAngles += Vector3.up * Input.GetAxis("Mouse X") * sensitivity;
            }
            else
            {
                xAxis.Rotate(Vector3.right * -Input.GetAxis("Mouse Y") * sensitivity, Space.Self);
                yAxis.Rotate(Vector3.up * Input.GetAxis("Mouse X") * sensitivity, Space.Self);
            }
            //test: look at moving object
            if(chaseTarget != null)
            {
                Vector3 deltaX = chaseTarget.transform.position - xAxis.position;
                float newAngleX = Mathf.Rad2Deg * Mathf.Atan2(-deltaX.y, Mathf.Sqrt(deltaX.x * deltaX.x + deltaX.z * deltaX.z));
                if (!storedLastPos)
                {
                    storedLastPos = true;
                }
                else
                {
                    Vector3 oldPosVec = lastChaseDeltaY.Set(y: 0).normalized;
                    Vector3 newPosVec = (chaseTarget.transform.position - yAxis.position).Set(y: 0).normalized;
                    yAxis.Rotate(Vector3.up * Vector3.SignedAngle(oldPosVec, newPosVec, Vector3.up), Space.Self);

                    xAxis.Rotate(Vector3.right * (newAngleX - lastChaseDeltaXAng), Space.Self);
                }
                lastChaseDeltaXAng = newAngleX;
                lastChaseDeltaY = chaseTarget.transform.position - yAxis.position;
            }
            //cursor
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                LockCursor(false);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                LockCursor(true);
            }
        }
        public void LockCursor(bool cond)
        {
            if (cond)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
        private void OnDestroy()
        {
            LockCursor(false);
        }
    }
}