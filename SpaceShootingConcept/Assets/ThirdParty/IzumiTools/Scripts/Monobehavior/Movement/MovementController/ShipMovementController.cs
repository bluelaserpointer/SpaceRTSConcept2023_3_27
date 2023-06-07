using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IzumiTools
{
    [DisallowMultipleComponent]
    public class ShipMovementController : MonoBehaviour
    {
        //inspector
        [SerializeField]
        Rigidbody _positionRigidbody;
        [SerializeField]
        Rigidbody _rotationRigidbody;
        [SerializeField]
        ShipMobility _mobility;

        [System.Serializable]
        struct ShipMobility
        {
            public float foward;
            public float backward;
            public float rotateSpeed;
            public float elevation;
            public Vector3 Output(Vector3 movementInput)
            {
                movementInput.z *= (movementInput.z > 0 ? foward : backward);
                movementInput.y *= elevation;
                return movementInput;
            }
        }

        public Rigidbody PositionRigidbody => _positionRigidbody;
        public Rigidbody RotationRigidbody => _rotationRigidbody;
        [HideInInspector]
        public Vector3 movementInput;
        [HideInInspector]
        public Vector3 rotationInput;

        void Update()
        {
            movementInput = new Vector3(0, 0, Input.GetAxisRaw("Vertical"));
            rotationInput = new Vector3(-Input.GetAxisRaw("Elevation"), Input.GetAxisRaw("Horizontal"), -Input.GetAxisRaw("Roll"));
        }
        private void FixedUpdate()
        {
            PositionRigidbody.AddForce(RotationRigidbody.transform.TransformVector(_mobility.Output(movementInput)) * Time.fixedDeltaTime, ForceMode.Force);
            Vector3 worldRotation = new Vector3(0, 0, 0);
            Vector3 shipRotation = RotationRigidbody.transform.TransformVector(new Vector3(rotationInput.x, rotationInput.y, rotationInput.z)); 
            RotationRigidbody.AddTorque(_mobility.rotateSpeed * (worldRotation + shipRotation) * Time.fixedDeltaTime, ForceMode.Force);
        }
    }
}