using UnityEngine;
using System.Collections.Generic;

public class ragdollController : MonoBehaviour
{
    public Transform animatedRoot;  // Root of animated skeleton
    public Transform ragdollRoot;   // Root of ragdoll skeleton

    // Animated Game Object params
    public GameObject animatedObj;
    public float rotationSpeed = 5f;

    private Dictionary<Transform, Rigidbody> boneMap = new Dictionary<Transform, Rigidbody>();

    public float rotationStiffness = 2000f;
    public float rotationDamping = 50f;

    private Rigidbody pelvisRb;


    private Rigidbody spineRb;
    private Rigidbody rootRb;

    // Spine Upright Control
    public float desiredSpineHeight = 1.95f; // How high spine should float above ground
    public float spinePinStiffness = 1000f;
    public float spinePinDamping = 100f;
    public float maxSpineForce = 500f;

    // Forward Movement control
    public float moveForwardForce = 300f;     // How hard to push forward when moving
    public float spine_moveForce = 100f;
    public float bobbingSpeed = 10f;           // Speed of bobbing oscillation
    public float bobbingAmplitude = 50f;       // Strength of up/down bobbing


    // Rotation Control
    public float turnStrength = 5000f; 
    float maxAllowedAngle = 90f; // Degrees


    // Procedural Walking Targets
    public Transform leftFootTarget;
    public Transform rightFootTarget;

    // Procedural Walking Params
    public float stepLength = 0.6f;
    public float stepHeight = 0.4f;
    public float stepDuration = 0.8f;

    private bool leftFootStepping = false;
    private bool rightFootStepping = false;
    private float leftFootStepStartTime;
    private float rightFootStepStartTime;

    private Vector3 leftFootStepStartPos;
    private Vector3 leftFootStepEndPos;

    private bool ragdollMode = false;
    bool jumpKeyInput = false;
    bool isJumping = false;

    //Grabbing Mechanic
    public GameObject poseSampler; // Reference to PoseSampler GameObject
    public AnimationClip grabMidClip;
    public float grabBlendWeight = 1.0f; // 0 = no effect, 1 = fully apply
    public bool isGrabbingLeft = false;
    public bool isGrabbingRight = false;
    private Dictionary<string, Quaternion> grabMidRotations;
    private Animation grabAnimation;

    private Vector3 rightFootStepStartPos;
    private Vector3 rightFootStepEndPos;

    public float jumpForce = 500f;


    void Start()
    {
        grabMidRotations = new Dictionary<string, Quaternion>();
        // Build bone map
        foreach (Rigidbody rb in ragdollRoot.GetComponentsInChildren<Rigidbody>())
        {
            Transform matchingBone = FindChildByName(animatedRoot, rb.gameObject.name);
            if (matchingBone != null)
            {
                boneMap.Add(matchingBone, rb);
                // Debug.LogWarning("Found Matching bone for: " + rb.gameObject.name + " and " + matchingBone.name);
            }
            else
            {
                Debug.LogWarning("No matching animated bone for: " + rb.gameObject.name);
            }
        }

        spineRb = FindBoneRigidbodyByName("Spine1"); // (or whatever your bone name is exactly)
        rootRb = FindBoneRigidbodyByName("Root");


        if (grabMidClip != null)
        {
            grabAnimation = animatedObj.GetComponent<Animation>();
            grabMidClip.SampleAnimation(animatedObj, 0f);  // Apply the pose to gooby_animated

            foreach (Transform t in animatedObj.GetComponentsInChildren<Transform>())
            {
                grabMidRotations[t.name] = t.localRotation;
            }

            Debug.Log("Grab mid pose loaded from gooby_animated");

        }

        foreach (var kvp in grabMidRotations)
        {
            Debug.Log("Grab pose bone key: " + kvp.Key);
        }


    }
    void Update(){
        bool toggleRagdollMode  = Input.GetKey(KeyCode.R);
         if(toggleRagdollMode){
            ragdollMode = !ragdollMode;
        }
        jumpKeyInput = Input.GetKeyDown(KeyCode.Space);
        // if(jumpKeyInput && !isJumping){
        //     isJumping = true;
        //     //apply jumpForce
            
        // }

        if (jumpKeyInput && !isJumping && IsGrounded())
        {
            isJumping = true;
            rootRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        if (IsGrounded()){
            isJumping = false;
        }
    }
    void FixedUpdate()
    {
        isGrabbingLeft = Input.GetMouseButton(0);  // Left click
        isGrabbingRight = Input.GetMouseButton(1); // Right click

        bool isPressingW = Input.GetKey(KeyCode.W);
        bool isPressingS = Input.GetKey(KeyCode.S);
        bool isTurningRight = Input.GetKey(KeyCode.D);
        bool isTurningLeft = Input.GetKey(KeyCode.A);
        
        

        if (!ragdollMode){
           foreach (var pair in boneMap)
            {

                ApplyBoneRotationControl(pair.Key, pair.Value);


                // // This is the alternative Grab mechanic
                // string boneName = pair.Key.name;

                // bool skipLeftArm = isGrabbingLeft && (boneName.Contains("UpperArm.L") || boneName.Contains("LowerArm.L"));
                // bool skipRightArm = isGrabbingRight && (boneName.Contains("UpperArm.R") || boneName.Contains("LowerArm.R"));

                // if (!skipLeftArm && !skipRightArm)
                // {
                //     ApplyBoneRotationControl(pair.Key, pair.Value);
                // }
                // }

                // if (isGrabbingLeft)
                // {
                //     ApplyPoseToBone("UpperArm.L", grabMidRotations);
                //     ApplyPoseToBone("LowerArm.L", grabMidRotations);
                // }

                // if (isGrabbingRight)
                // {
                //     ApplyPoseToBone("UpperArm.R", grabMidRotations);
                //     ApplyPoseToBone("LowerArm.R", grabMidRotations);
            }
        }




        if (spineRb != null && !ragdollMode && !isJumping)
        {
            Vector3 currentSpinePosition = spineRb.position;

            // Target is straight upward from ground at a fixed height
            Vector3 targetPosition = new Vector3(currentSpinePosition.x, desiredSpineHeight, currentSpinePosition.z);

            // Hooke's law spring toward target
            Vector3 springForce = (targetPosition - currentSpinePosition) * spinePinStiffness - spineRb.linearVelocity * spinePinDamping;

            // Clamp to prevent crazy impulses
            springForce = Vector3.ClampMagnitude(springForce, maxSpineForce);

            spineRb.AddForce(springForce, ForceMode.Acceleration);
        }

        if (isPressingW && spineRb != null)
            {
                // Push spine forward in ragdollRoot's forward direction
                Vector3 forward = ragdollRoot.forward;
                Vector3 moveForce = forward * moveForwardForce; // Define moveForwardForce at top
                rootRb.AddForce(moveForce, ForceMode.Acceleration);
                Vector3 moveForce2 = forward * spine_moveForce; // Define moveForwardForce at top
                spineRb.AddForce(moveForce2, ForceMode.Acceleration);
                // (Optional) Add bobbing by adding some slight upward force too
                float bobbingForce = Mathf.Sin(Time.time * bobbingSpeed) * bobbingAmplitude;
                // spineRb.AddForce(Vector3.up * bobbingForce, ForceMode.Acceleration);
            }

        // Only trigger a new step if not already stepping
        if (isPressingS)
            {
                if (!leftFootStepping && !rightFootStepping)
                {
                    StartLeftFootStep();
                }
                else if (leftFootStepping && !rightFootStepping && Time.time - leftFootStepStartTime > stepDuration / 2f)
                {
                    StartRightFootStep();
                }
            }
        
        if (leftFootStepping)
        {
            float t = (Time.time - leftFootStepStartTime) / stepDuration;
            if (t >= 1f)
            {
                t = 1f;
                leftFootStepping = false;
            }

            Vector3 horizontalPos = Vector3.Lerp(leftFootStepStartPos, leftFootStepEndPos, t);
            float verticalOffset = Mathf.Sin(t * Mathf.PI) * stepHeight;
            leftFootTarget.position = horizontalPos + Vector3.up * verticalOffset;
            ApplyFootSpring(leftFootTarget, "LowerLeg.L"); 
        }

        if (rightFootStepping)
        {
            float t = (Time.time - rightFootStepStartTime) / stepDuration;
            if (t >= 1f)
            {
                t = 1f;
                rightFootStepping = false;
            }

            Vector3 horizontalPos = Vector3.Lerp(rightFootStepStartPos, rightFootStepEndPos, t);
            float verticalOffset = Mathf.Sin(t * Mathf.PI) * stepHeight;
            rightFootTarget.position = horizontalPos + Vector3.up * verticalOffset;
            ApplyFootSpring(rightFootTarget, "LowerLeg.R");
        }

        if (isTurningLeft && spineRb != null)
        {
            if (animatedObj == null){
                    Debug.LogError("animatedObj is not assigned!");
            }

            Vector3 turningTorque = new Vector3(0f, -turnStrength, 0f); // Negative Y torque to turn left
            spineRb.AddTorque(turningTorque, ForceMode.Acceleration);
            // animatedRoot.transform.Rotate(Vector3.down * rotationSpeed * Time.deltaTime);
        }

        if (isTurningRight && spineRb != null)
        {

            if (animatedObj == null){
                    Debug.LogError("animatedObj is not assigned!");
            }

            Vector3 turningTorque = new Vector3(0f, turnStrength, 0f); // Positive Y torque to turn right
            spineRb.AddTorque(turningTorque, ForceMode.Acceleration);
            // animatedRoot.transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
        }




        

       

    }

    private Transform FindChildByName(Transform root, string name)
    {
        foreach (Transform t in root.GetComponentsInChildren<Transform>())
        {
            if (t.name == name)
                return t;
        }
        return null;
    }
    private Rigidbody FindPelvisRigidbody()
    {
        foreach (var pair in boneMap)
        {
            if (pair.Key.name.ToLower().Contains("Root") || pair.Key.name.ToLower().Contains("root"))
            {
                return pair.Value;
            }
        }
        Debug.LogWarning("Pelvis Rigidbody not found!");
        return null;
    }

    private Rigidbody FindBoneRigidbodyByName(string boneName)
        {
            foreach (var pair in boneMap)
            {
                if (pair.Key.name == boneName)
                {
                    return pair.Value;
                }
            }
            Debug.LogWarning($"Bone Rigidbody not found: {boneName}");
            return null;
        }

    void StartLeftFootStep()
    {
        leftFootStepping = true;
        leftFootStepStartTime = Time.time;
        leftFootStepStartPos = leftFootTarget.position;
        leftFootStepEndPos = leftFootStepStartPos - ragdollRoot.forward * stepLength;
    }

    void StartRightFootStep()
    {
        rightFootStepping = true;
        rightFootStepStartTime = Time.time;
        rightFootStepStartPos = rightFootTarget.position;
        rightFootStepEndPos = rightFootStepStartPos - ragdollRoot.forward * stepLength;
    }



    void ApplyFootSpring(Transform target, string boneName)
    {
        Rigidbody rb = FindBoneRigidbodyByName(boneName);
        if (rb != null)
        {
            Vector3 force = (target.position - rb.position) * 1000f - rb.linearVelocity * 100f;
            rb.AddForce(force, ForceMode.Acceleration);
        }
    }

    void ApplyBoneRotationControl(Transform animatedBone, Rigidbody ragdollBone)
    {
        // Old World-Space Version (your current working one)

        Quaternion targetRotation = animatedBone.rotation;
        Quaternion deltaRotation = targetRotation * Quaternion.Inverse(ragdollBone.rotation);
        deltaRotation.ToAngleAxis(out float angle, out Vector3 axis);
        if (angle > 180f) angle -= 360f;

        Vector3 torque = (axis * angle * Mathf.Deg2Rad * rotationStiffness) - ragdollBone.angularVelocity * rotationDamping;
        ragdollBone.AddTorque(torque, ForceMode.Acceleration);
    }

    bool IsGrounded()
    {
        Ray ray = new Ray(rootRb.position, Vector3.down);
        float rayLength = 1.5f; // Adjust based on how tall your character is
        return Physics.Raycast(ray, rayLength);
    }


    void ApplyBoneRotationControlRelative(Transform animatedBone, Rigidbody ragdollBone)
    {
        Transform ragdollBoneTransform = ragdollBone.transform;

        // Correct RELATIVE way: compare local rotations directly
        Quaternion targetLocalRotation = animatedBone.localRotation;
        Quaternion currentLocalRotation = ragdollBoneTransform.localRotation;

        Quaternion deltaRotation = targetLocalRotation * Quaternion.Inverse(currentLocalRotation);
        deltaRotation.ToAngleAxis(out float angle, out Vector3 axis);

        if (angle > 180f) angle -= 360f;

        if (float.IsNaN(axis.x) || float.IsNaN(axis.y) || float.IsNaN(axis.z))
        {
            return;
        }

        Vector3 torque = (axis * angle * Mathf.Deg2Rad * rotationStiffness) - ragdollBone.angularVelocity * rotationDamping;

        float maxTorqueForce = 1000f;
        torque = Vector3.ClampMagnitude(torque, maxTorqueForce);

        ragdollBone.AddTorque(torque, ForceMode.Acceleration);
    }

    void ApplyPoseToBone(string boneName, Dictionary<string, Quaternion> pose)
    {
        var rb = FindBoneRigidbodyByName(boneName);
        Debug.Log("Bone: " + rb);
        Debug.Log(boneName + "Bone Pose:" + pose[boneName]);
        if (rb != null && pose.ContainsKey(boneName))
        {
            Debug.Log(boneName + "Bone Pose:" + pose[boneName]);

            Quaternion target = pose[boneName];
            Quaternion currentLocal = rb.transform.localRotation;
            Quaternion delta = target * Quaternion.Inverse(currentLocal);
            delta.ToAngleAxis(out float angle, out Vector3 axis);
            Debug.Log("Angle: " + angle);
            if (angle > 180f) angle -= 360f;
            if (float.IsNaN(axis.x) || float.IsNaN(axis.y) || float.IsNaN(axis.z)) {
                Debug.Log("hit this naughty boy");
                return;
            }

            Vector3 torque = (axis * angle * Mathf.Deg2Rad * rotationStiffness) - rb.angularVelocity * rotationDamping;
            rb.AddTorque(torque, ForceMode.Acceleration);
        }
    }




}
