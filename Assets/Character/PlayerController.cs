using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private RagdollController ragdollController;
    private AnimationController animationController;

    public bool isGrabbingLeft = false;
    public bool isGrabbingRight = false;

    private Vector3 inputDirection;

    void Start()
    {
        ragdollController = GetComponentInChildren<RagdollController>();
        animationController = GetComponentInChildren<AnimationController>();

        if (ragdollController == null)
            Debug.LogError("RagdollController not found!");

        if (animationController == null)
            Debug.LogError("animationController not found!");

        ragdollController.Initialize(); 
    }

    void Update()
    {
        // WASD movement direction
        float h = Input.GetKey(KeyCode.A) ? -1f : (Input.GetKey(KeyCode.D) ? 1f : 0f);
        float v = Input.GetKey(KeyCode.S) ? -1f : (Input.GetKey(KeyCode.W) ? 1f : 0f);
        inputDirection = new Vector3(h, 0f, v).normalized;

        // Punching
        bool isHoldingShift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        if (isHoldingShift)
        {
            if (Input.GetMouseButtonDown(0)) animationController.PunchLeft();
            if (Input.GetMouseButtonDown(1)) animationController.PunchRight();
        }

        // Grabbing (Start)
        if (!isHoldingShift)
        {
            if (Input.GetMouseButtonDown(0))
            {
                isGrabbingLeft = true;
                animationController.StartGrab(true, Input.mousePosition.y);
            }
            if (Input.GetMouseButtonDown(1))
            {
                isGrabbingRight = true;
                animationController.StartGrab(false, Input.mousePosition.y);
            }
        }

        if (isGrabbingLeft)
            ragdollController.TryGrab(true);
        if (isGrabbingRight)
            ragdollController.TryGrab(false);

        // Grabbing (Update)
        if (isGrabbingLeft || isGrabbingRight)
        {
            animationController.UpdateGrabHeight(Input.mousePosition.y);
        }

        // Grabbing (End)
        if (Input.GetMouseButtonUp(0) && isGrabbingLeft)
        {
            isGrabbingLeft = false;
            animationController.StopGrab(true);
            ragdollController.StopGrab(true);
        }

        if (Input.GetMouseButtonUp(1) && isGrabbingRight)
        {
            isGrabbingRight = false;
            animationController.StopGrab(false);
            ragdollController.StopGrab(false);
        }

        // Jump
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ragdollController.TryJump();
        }

        // Toggle Ragdoll Mode
        if (Input.GetKeyDown(KeyCode.R))
        {
            ragdollController.ToggleRagdollMode();
        }
    }

    void FixedUpdate()
    {
        Vector3 rootVelocity = ragdollController.GetRootVelocity();

        // Animate walking based on real movement
        animationController.SetWalking(rootVelocity.magnitude > 1.2f);

        animationController.FaceInputDirection(inputDirection);


        // Apply movement forces
        ragdollController.TickPhysics(inputDirection);
    }

}
