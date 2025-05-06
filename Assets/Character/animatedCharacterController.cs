using UnityEngine;

public class AnimationController : MonoBehaviour
{
    private Animator animator;
    private float rotationSpeed = 150f;

    private bool grabLeft = false;
    private bool grabRight = false;

    private float grabHeight = 0.5f; // 0 = low, 1 = high
    private float mouseStartY;
    private float mouseSensitivity = 0.003f;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void SetWalking(bool isWalking)
    {
        animator.SetBool("isWalking", isWalking);
    }

    public void RotateLeft()
    {
        transform.Rotate(Vector3.down * rotationSpeed * Time.deltaTime);
    }

    public void RotateRight()
    {
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }

    public void PunchLeft()
    {
        animator.SetTrigger("PunchLeft");
    }

    public void PunchRight()
    {
        animator.SetTrigger("PunchRight");
    }

    public void StartGrab(bool isLeft, float currentMouseY)
    {
        animator.SetBool("Grab", true);
        mouseStartY = currentMouseY;

        if (isLeft) grabLeft = true;
        else grabRight = true;
    }

    public void UpdateGrabHeight(float currentMouseY)
    {
        if (!grabLeft && !grabRight) return;

        float mouseDeltaY = currentMouseY - mouseStartY;
        grabHeight = Mathf.Clamp01(0.5f + mouseDeltaY * mouseSensitivity);
        animator.SetFloat("GrabHeight", grabHeight);
    }

    public void StopGrab(bool isLeft)
    {
        if (isLeft) grabLeft = false;
        else grabRight = false;

        // If neither grab is still active, clear animation state
        if (!grabLeft && !grabRight)
        {
            animator.SetBool("Grab", false);
        }
    }
}
