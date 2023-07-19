using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IzumiTools
{
    /// <summary>
    /// Controls cinemachine virtual camera sensitivity and fieldOfView by mouse, and manages cursor lock.
    /// </summary>
    [DisallowMultipleComponent]
    public class MouseControlFreeLookVirtualCamera : MonoBehaviour
    {
        //inspector
        [Header("Sensitivity")]
        [SerializeField]
        float sensitivityMouse = 1f;
        [SerializeField]
        ExponentialCameraFOVController _fovController;
        [SerializeField]
        float _fovAffectSensitivityRatio = 1.0f;

        [Header("Option")]
        [SerializeField]
        bool lockCursorOnAwake = true;

        [Header("Reference")]
        [SerializeField]
        CinemachineFreeLook freelook;
        //
        float baseXSpeed, baseYSpeed;
        private void Awake()
        {
            if (lockCursorOnAwake)
                LockCursor(true);
            baseXSpeed = freelook.m_XAxis.m_MaxSpeed;
            baseYSpeed = freelook.m_YAxis.m_MaxSpeed;
        }
        void LateUpdate()
        {
            //sensitivity
            float sensitivity = sensitivityMouse;
            if (_fovController != null)
                sensitivity *= Mathf.Pow(_fovController.AppliedChangeRatio, _fovAffectSensitivityRatio);
            freelook.m_XAxis.m_MaxSpeed = baseXSpeed * sensitivity;
            freelook.m_YAxis.m_MaxSpeed = baseYSpeed * sensitivity;
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