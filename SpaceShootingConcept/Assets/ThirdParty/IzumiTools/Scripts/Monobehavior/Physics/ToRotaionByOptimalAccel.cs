using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace IzumiTools
{
    /// <summary>
    /// Head towards rotation by optimal accel, without overswing in most cases.<br/>
    /// Top accel is limitable, while it's change speed is not limitable. <br/>
    /// If you lower accel limit during its operation, its could still overswing.
    /// </summary>
    [DisallowMultipleComponent]
    public class ToRotaionByOptimalAccel : MonoBehaviour
    {
        public new Rigidbody rigidbody;
        public ArticulationBody articulationBody;
        //inspector
        public Quaternion targetRotation;
        public float maxVelocityChange = 5;
        public float safeStability = 0.9F;

        private void FixedUpdate()
        {
            Quaternion deltaRotation;
            Vector3 deltaEularAngles;
            Vector3 optimalAngVelocity;
            Vector3 actualAngVelocity;
            if (rigidbody != null)
            {
                deltaRotation = targetRotation * Quaternion.Inverse(rigidbody.rotation);
                deltaEularAngles = (deltaRotation.w > 0 ? 1 : -1) * new Vector3(deltaRotation.x, deltaRotation.y, deltaRotation.z);
                optimalAngVelocity = new Vector3(OptimalVelocityStopAt(deltaEularAngles.x), OptimalVelocityStopAt(deltaEularAngles.y), OptimalVelocityStopAt(deltaEularAngles.z));
                actualAngVelocity = Vector3.MoveTowards(rigidbody.angularVelocity, optimalAngVelocity, maxVelocityChange * Time.fixedDeltaTime);
                rigidbody.angularVelocity = actualAngVelocity;
            }
            if (articulationBody != null)
            {
                deltaRotation = targetRotation * Quaternion.Inverse(articulationBody.transform.rotation);
                deltaEularAngles = (deltaRotation.w > 0 ? 1 : -1) * new Vector3(deltaRotation.x, deltaRotation.y, deltaRotation.z);
                optimalAngVelocity = new Vector3(OptimalVelocityStopAt(deltaEularAngles.x), OptimalVelocityStopAt(deltaEularAngles.y), OptimalVelocityStopAt(deltaEularAngles.z));
                actualAngVelocity = Vector3.MoveTowards(articulationBody.angularVelocity, optimalAngVelocity, maxVelocityChange * Time.fixedDeltaTime);
                articulationBody.angularVelocity = actualAngVelocity;
            }
        }
        private float OptimalVelocityStopAt(float distance)
        {
            return ExtendedMath.OptimalVelocityStopAt(distance, maxVelocityChange) * safeStability;
        }
        public static Vector3 OptimalAngularVelocityTowardsRotation(Rigidbody rigidbody, Quaternion targetRotation, float maxAccel, float safeStability = 0.9F)
        {
            Quaternion deltaRotation = targetRotation * Quaternion.Inverse(rigidbody.rotation);
            Vector3 deltaEularAngles = (deltaRotation.w > 0 ? 1 : -1) * new Vector3(deltaRotation.x, deltaRotation.y, deltaRotation.z);
            Vector3 optimalAngVelocity = new Vector3(
                ExtendedMath.OptimalVelocityStopAt(deltaEularAngles.x, maxAccel) * safeStability,
                ExtendedMath.OptimalVelocityStopAt(deltaEularAngles.y, maxAccel) * safeStability,
                ExtendedMath.OptimalVelocityStopAt(deltaEularAngles.z, maxAccel) * safeStability);
            Vector3 actualAngVelocity = Vector3.MoveTowards(rigidbody.angularVelocity, optimalAngVelocity, maxAccel * Time.fixedDeltaTime);
            return actualAngVelocity;
        }
    }
}