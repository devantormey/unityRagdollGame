// Refactored ragdollController into modular structure
// This script is now intended to be called by a top-level Player script

using UnityEngine;
using System.Collections.Generic;

public class RagdollController : MonoBehaviour
{
    [Header("References")]
    public Transform animatedRoot;
    public Transform ragdollRoot;
    public GameObject animatedObj;

    [Header("Physics Parameters")]
    public float rotationStiffness = 2000f;
    public float rotationDamping = 50f;
    public float desiredSpineHeight = 1.95f;
    public float spinePinStiffness = 1000f;
    public float spinePinDamping = 100f;
    public float uprightStiffness = 2000f;
    public float uprightDamping = 100f;
    public float maxSpineForce = 500f;
    public float moveForwardForce = 100f;
    public float spine_moveForce = 100f;
    public float bobbingSpeed = 10f;
    public float bobbingAmplitude = 50f;
    public float turnStrength = 5000f;
    public float jumpForce = 500f;

    [Header("Foot Control")]
    public bool groundedBool = true;
    public FootGroundDetector leftFootDetector;
    public FootGroundDetector rightFootDetector;

    [Header("Grabbing")]
    public ConfigurableJoint leftGrabJoint;
    public ConfigurableJoint rightGrabJoint;
    public HandTrigger leftHandTrigger;
    public HandTrigger rightHandTrigger;

    private Dictionary<Transform, Rigidbody> boneMap = new();
    private Rigidbody spineRb;
    private Rigidbody rootRb;
    private Rigidbody rightArmRb;

    private bool ragdollMode;
    public bool isJumping;

    public void Initialize()
    {
        boneMap.Clear();
        foreach (Rigidbody rb in ragdollRoot.GetComponentsInChildren<Rigidbody>())
        {
            Transform match = FindChildByName(animatedRoot, rb.gameObject.name);
            if (match != null) boneMap[match] = rb;
        }
        spineRb = FindBoneRigidbodyByName("Spine1");
        rootRb = FindBoneRigidbodyByName("Root");
        rightArmRb = FindBoneRigidbodyByName("LowerArm.R");
    }

    public void ToggleRagdoll(bool enabled)
    {
        ragdollMode = enabled;
    }

    public void ApplyJumpImpulse()
    {
        if (isJumping || rootRb == null) return;
        isJumping = true;
        // Debug.Log("Jumping Code Ran: " + isJumping);
        rootRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        foreach (var pair in boneMap)
        {
            if (pair.Value != rootRb)
                pair.Value.AddForce(Vector3.up * (jumpForce * 0.5f), ForceMode.Impulse);
        }
    }


    public void ApplyUprightTorque()
    {
        if (rootRb == null || ragdollMode) return;

        // Current up direction of the spine
        Vector3 currentUp = rootRb.transform.up;
        
        // Calculate axis and angle to rotate from current up to world up
        Vector3 torqueAxis = Vector3.Cross(currentUp, Vector3.up);
        float angle = Vector3.Angle(currentUp, Vector3.up);

        // Convert angle to radians and apply stiffness
        Vector3 correctiveTorque = torqueAxis.normalized * angle * Mathf.Deg2Rad * uprightStiffness;

        // Apply damping based on angular velocity
        Vector3 dampingTorque = -rootRb.angularVelocity * uprightDamping;

        // Total torque
        Vector3 totalTorque = correctiveTorque + dampingTorque;

        rootRb.AddTorque(totalTorque, ForceMode.Acceleration);
    }

    public void UpdateSpineSpring()
    {
        if (ragdollMode || isJumping || spineRb == null || IsGrounded()  == false) return;

        Vector3 target = new(spineRb.position.x, desiredSpineHeight, spineRb.position.z);
        Vector3 springForce = (target - spineRb.position) * spinePinStiffness - spineRb.linearVelocity * spinePinDamping;
        springForce = Vector3.ClampMagnitude(springForce, maxSpineForce);
        spineRb.AddForce(springForce, ForceMode.Acceleration);
    }

    public void ApplyMovement(bool forward)
    {
        if (!forward || spineRb == null || ragdollMode || IsGrounded() == false ) return;
        Vector3 forwardVec = ragdollRoot.forward;
        rootRb.AddForce(forwardVec * moveForwardForce, ForceMode.Acceleration);
        spineRb.AddForce(forwardVec * spine_moveForce, ForceMode.Acceleration);
    }

    public void ApplyTurning(float direction)
    {
        if (spineRb == null || ragdollMode) return;
        spineRb.AddTorque(Vector3.up * turnStrength * direction, ForceMode.Acceleration);
    }


    public void GrabObject(Rigidbody targetRb, bool isLeft)
    {
        var joint = isLeft ? leftGrabJoint : rightGrabJoint;
        joint.connectedBody = targetRb;
        joint.xMotion = ConfigurableJointMotion.Limited;
        joint.yMotion = ConfigurableJointMotion.Limited;
        joint.zMotion = ConfigurableJointMotion.Limited;
    }

    public void ReleaseObject(bool isLeft)
    {
        var joint = isLeft ? leftGrabJoint : rightGrabJoint;
        joint.connectedBody = null;
        joint.xMotion = ConfigurableJointMotion.Free;
        joint.yMotion = ConfigurableJointMotion.Free;
        joint.zMotion = ConfigurableJointMotion.Free;
    }


    private void ApplyBoneRotationControl(Transform animatedBone, Rigidbody ragdollBone)
    {
        Quaternion targetRotation = animatedBone.rotation;
        Quaternion deltaRotation = targetRotation * Quaternion.Inverse(ragdollBone.rotation);
        deltaRotation.ToAngleAxis(out float angle, out Vector3 axis);
        if (angle > 180f) angle -= 360f;
        Vector3 torque = (axis * angle * Mathf.Deg2Rad * rotationStiffness) - ragdollBone.angularVelocity * rotationDamping;
        ragdollBone.AddTorque(torque, ForceMode.Acceleration);
    }

    private Transform FindChildByName(Transform root, string name)
    {
        foreach (Transform t in root.GetComponentsInChildren<Transform>())
        {
            if (t.name == name) return t;
        }
        return null;
    }

    private Rigidbody FindBoneRigidbodyByName(string name)
    {
        foreach (var pair in boneMap)
        {
            if (pair.Key.name == name) return pair.Value;
        }
        return null;
    }

    public void ApplyBoneRotationControlAll()
    {
        if (ragdollMode) return;
        foreach (var pair in boneMap)
            ApplyBoneRotationControl(pair.Key, pair.Value);
    }
    public void ApplyLocalBoneRotationControlAll()
    {
        if (ragdollMode) return;
        foreach (var pair in boneMap)
        {
            ApplyLocalBoneRotationControl(pair.Key, pair.Value);
        }
    }

    private void ApplyLocalBoneRotationControl(Transform animatedBone, Rigidbody ragdollBone)
    {
        Quaternion targetLocalRotation = animatedBone.localRotation;
        Quaternion deltaRotation = targetLocalRotation * Quaternion.Inverse(ragdollBone.transform.localRotation);

        deltaRotation.ToAngleAxis(out float angle, out Vector3 axis);
        if (angle > 180f) angle -= 360f;

        Vector3 torque = (axis * angle * Mathf.Deg2Rad * rotationStiffness) - ragdollBone.angularVelocity * rotationDamping;
        ragdollBone.AddTorque(torque, ForceMode.Acceleration);
    }

    public bool IsGrounded()
    {
        // Check both foot detectors first
        if (leftFootDetector != null && rightFootDetector != null)
        {
            if (leftFootDetector.isGrounded || rightFootDetector.isGrounded)
                return true;
        }

        // Fallback: raycast from root if foot detectors aren't available
        Ray ray = new Ray(rootRb.position, Vector3.down);
        return Physics.Raycast(ray, desiredSpineHeight);
    }


    public void SetIsJumping(bool state)
    {
        Debug.Log("Jumping State: " + isJumping);
        isJumping = state;
    }

    public void TryGrab(bool isLeft)
    {
        var joint = isLeft ? leftGrabJoint : rightGrabJoint;
        var trigger = isLeft ? leftHandTrigger : rightHandTrigger;

        if (joint.connectedBody == null && trigger.targetRigidbody != null)
        {
            GrabObject(trigger.targetRigidbody, isLeft);
        }
    }


    public void StopGrab(bool isLeft)
    {
        ReleaseObject(isLeft);
    }

    public void TryJump()
    {
        if (!isJumping && IsGrounded())
            ApplyJumpImpulse();
    }
    public void ToggleRagdollMode()
    {
        ragdollMode = !ragdollMode;
    }

    public bool CheckJumping(){
        return isJumping;
    }

    public void ApplyPunchImpulse(float force)
    {
        if (spineRb == null) return;

        Vector3 forward = spineRb.transform.forward;
        rightArmRb.AddForce(forward * force, ForceMode.Impulse);
        // Debug.Log("applying punch force!");
    }

    public void TickPhysics(Vector3 moveDirection)
    {
        ApplyBoneRotationControlAll();
        // UpdateSpineSpring();
        // ApplyLocalBoneRotationControlAll();
        ApplyUprightTorque();

        if(IsGrounded() && isJumping)
        {
            isJumping = false;
        }
        if (moveDirection != Vector3.zero && IsGrounded())
        {
            // Move in world-space direction
            Vector3 worldDir = moveDirection.normalized;
            rootRb.AddForce(worldDir * moveForwardForce, ForceMode.Acceleration);
            // spineRb.AddForce(worldDir * spine_moveForce, ForceMode.Acceleration);
        }
    }

    public Vector3 GetRootVelocity()
    {
        return rootRb != null ? rootRb.linearVelocity : Vector3.zero;
    }


}
