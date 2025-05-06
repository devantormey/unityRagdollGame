using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private  RagdollController ragdollController;
    private AnimationController animationController;

    public bool isGrabbingLeft = false;
    public bool isGrabbingRight = false;

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
        // Movement input
        bool moveForward = Input.GetKey(KeyCode.W);
        bool turnLeft = Input.GetKey(KeyCode.A);
        bool turnRight = Input.GetKey(KeyCode.D);

        animationController.SetWalking(moveForward);
        if (turnLeft) animationController.RotateLeft();
        if (turnRight) animationController.RotateRight();

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
                // ragdollController.StartGrab(true);
            }
            if (Input.GetMouseButtonDown(1))
            {
                isGrabbingRight = true;
                animationController.StartGrab(false, Input.mousePosition.y);
                // ragdollController.StartGrab(false);
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
        ragdollController.TickPhysics();
    }
}
