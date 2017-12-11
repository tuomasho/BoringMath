using UnityEngine;
using System.Collections;

using LockingPolicy = Thalmic.Myo.LockingPolicy;
using Pose = Thalmic.Myo.Pose;
using UnlockType = Thalmic.Myo.UnlockType;

namespace R1
{
    public class ArmMovement : MonoBehaviour
    {
        public static ArmMovement instance = null;
        public Quaternion _antiYaw = Quaternion.identity;
        private float _referenceRoll = 0.0f;
        private Pose _lastPose = Pose.Unknown;
        private GameObject gm;
        private GameManager gmScript;

        void Start()
        {
            gm = GameObject.Find("GameManager");
            gmScript = gm.GetComponent<GameManager>();
            gmScript.thalmicMyo = gmScript.myo.GetComponent<ThalmicMyo>();
        }
        void Update()
        {
            // Update references when the pose becomes fingers spread or the r key is pressed.
            bool updateReference = false;
            if (gmScript.thalmicMyo.pose != _lastPose)
            {
                _lastPose = gmScript.thalmicMyo.pose;

                if (gmScript.thalmicMyo.pose == Pose.FingersSpread)
                {
                    updateReference = true;

                    gmScript.ExtendUnlockAndNotifyUserAction(gmScript.thalmicMyo);
                }
            }
            if (Input.GetKeyDown("r"))
            {
                updateReference = true;
            }

            // Update references. This anchors the joint on-screen such that it faces forward away
            // from the viewer when the Myo armband is oriented the way it is when these references are taken.
            if (updateReference)
            {
                _antiYaw = Quaternion.FromToRotation(
                    new Vector3(gmScript.myo.transform.forward.x, 0, gmScript.myo.transform.forward.z),
                    new Vector3(0, 0, 1)
                );
            }

            // Current zero roll vector and roll value.
            Vector3 zeroRoll = computeZeroRollVector(gmScript.myo.transform.forward);
            float roll = rollFromZero(zeroRoll, gmScript.myo.transform.forward, gmScript.myo.transform.up);

            // The relative roll is simply how much the current roll has changed relative to the reference roll.
            // adjustAngle simply keeps the resultant value within -180 to 180 degrees.
            float relativeRoll = normalizeAngle(roll - _referenceRoll);

            // antiRoll represents a rotation about the myo Armband's forward axis adjusting for reference roll.
            Quaternion antiRoll = Quaternion.AngleAxis(relativeRoll, gmScript.myo.transform.forward);

            // Here the anti-roll and yaw rotations are applied to the myo Armband's forward direction to yield
            // the orientation of the joint.
            transform.rotation = _antiYaw * antiRoll * Quaternion.LookRotation(gmScript.myo.transform.forward);

            // The above calculations were done assuming the Myo armbands's +x direction, in its own coordinate system,
            // was facing toward the wearer's elbow. If the Myo armband is worn with its +x direction facing the other way,
            // the rotation needs to be updated to compensate.
            if (gmScript.thalmicMyo.xDirection != Thalmic.Myo.XDirection.TowardWrist)
            {
                // Mirror the rotation around the XZ plane in Unity's coordinate system (XY plane in Myo's coordinate
                // system). This makes the rotation reflect the arm's orientation, rather than that of the Myo armband.
                transform.rotation = new Quaternion(transform.localRotation.x,
                                                    -transform.localRotation.y,
                                                    transform.localRotation.z,
                                                    -transform.localRotation.w);
            }
        }

        // Compute the angle of rotation clockwise about the forward axis relative to the provided zero roll direction.
        // As the armband is rotated about the forward axis this value will change, regardless of which way the
        // forward vector of the Myo is pointing. The returned value will be between -180 and 180 degrees.
        float rollFromZero(Vector3 zeroRoll, Vector3 forward, Vector3 up)
        {
            // The cosine of the angle between the up vector and the zero roll vector. Since both are
            // orthogonal to the forward vector, this tells us how far the Myo has been turned around the
            // forward axis relative to the zero roll vector, but we need to determine separately whether the
            // Myo has been rolled clockwise or counterclockwise.
            float cosine = Vector3.Dot(up, zeroRoll);

            // To determine the sign of the roll, we take the cross product of the up vector and the zero
            // roll vector. This cross product will either be the same or opposite direction as the forward
            // vector depending on whether up is clockwise or counter-clockwise from zero roll.
            // Thus the sign of the dot product of forward and it yields the sign of our roll value.
            Vector3 cp = Vector3.Cross(up, zeroRoll);
            float directionCosine = Vector3.Dot(forward, cp);
            float sign = directionCosine < 0.0f ? 1.0f : -1.0f;

            // Return the angle of roll (in degrees) from the cosine and the sign.
            return sign * Mathf.Rad2Deg * Mathf.Acos(cosine);
        }

        // Compute a vector that points perpendicular to the forward direction,
        // minimizing angular distance from world up (positive Y axis).
        // This represents the direction of no rotation about its forward axis.
        Vector3 computeZeroRollVector(Vector3 forward)
        {
            Vector3 antigravity = Vector3.up;
            Vector3 m = Vector3.Cross(gmScript.myo.transform.forward, antigravity);
            Vector3 roll = Vector3.Cross(m, gmScript.myo.transform.forward);

            return roll.normalized;
        }

        // Adjust the provided angle to be within a -180 to 180.
        float normalizeAngle(float angle)
        {
            if (angle > 180.0f)
            {
                return angle - 360.0f;
            }
            if (angle < -180.0f)
            {
                return angle + 360.0f;
            }
            return angle;
        }
    }
}