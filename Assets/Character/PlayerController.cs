using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private RagdollController ragdollController;
    private AnimationController animationController;
    private IInputProvider inputProvider;
    private InputState inputState;

    public Health health;
    public float strength = 50f;

    private bool wasGrabbingLeft = false;
    private bool wasGrabbingRight = false;

    private bool wasAttacking = false;

    void Start()
    {
        ragdollController = GetComponentInChildren<RagdollController>();
        animationController = GetComponentInChildren<AnimationController>();
        inputProvider = GetComponent<IInputProvider>(); // This can be Keyboard or Gamepad

        if (ragdollController == null)
            Debug.LogError("RagdollController not found!");

        if (animationController == null)
            Debug.LogError("AnimationController not found!");

        if (inputProvider == null)
            Debug.LogError("No input provider found!");

        ragdollController.Initialize();

        animationController.OnPunchImpulse += () =>
        {
            ragdollController.ApplyPunchImpulse(strength);
        };
    }

    void Update()
    {
        if (inputProvider != null){ inputState = inputProvider.GetInputState(); }

        // Movement
        animationController.SetWalking(inputState.MoveDirection.magnitude > 0.1f);

        // Rotate the visual to face movement
        animationController.FaceInputDirection(inputState.MoveDirection);

        // Punching
        if (inputState.IsPunchingLeft) animationController.PunchLeft();
        if (inputState.IsPunchingRight) animationController.PunchRight();

        // Attacking
        if (inputState.IsAttacking && !wasAttacking)
        {
            animationController.StartAttack();
        }
        
        if (inputState.IsAttacking)
        {
            animationController.UpdateAttackProgress(inputState.AttackProgress);
            // ragdollController.TryAttack();
        }

        if (!inputState.IsAttacking && wasAttacking)
        {
            animationController.StopAttack();
            // ragdollController.StopAttack();
        }

        // Grabbing
        if (inputState.IsGrabbingLeft && !wasGrabbingLeft)
        {
            animationController.StartGrab(true);
        }
        if (inputState.IsGrabbingRight && !wasGrabbingRight)
        {
            animationController.StartGrab(false);
        }

        if (inputState.IsGrabbingLeft)
        {
            animationController.UpdateGrabHeight(inputState.GrabHeight);
            ragdollController.TryGrab(true);
        }
        if (inputState.IsGrabbingRight)
        {
            animationController.UpdateGrabHeight(inputState.GrabHeight);
            ragdollController.TryGrab(false);
        }

        if (!inputState.IsGrabbingLeft && wasGrabbingLeft)
        {
            animationController.StopGrab(true);
            ragdollController.StopGrab(true);
        }

        if (!inputState.IsGrabbingRight && wasGrabbingRight)
        {
            animationController.StopGrab(false);
            ragdollController.StopGrab(false);
        }

        // Cache for next frame
        wasGrabbingLeft = inputState.IsGrabbingLeft;
        wasGrabbingRight = inputState.IsGrabbingRight;
        wasAttacking = inputState.IsAttacking;

        // Jump
        if (inputState.IsJumping){
            ragdollController.TryJump();
            // Debug.Log("Starting Jump");
            animationController.StartJump();
        }  


        if (!ragdollController.CheckJumping()){
                // Debug.Log("Stopping Jump");
                animationController.StopJump();
            }
        // Optional: toggle ragdoll mode here
    }

    void FixedUpdate()
    {
        ragdollController.TickPhysics(inputState.MoveDirection);
    }
}
