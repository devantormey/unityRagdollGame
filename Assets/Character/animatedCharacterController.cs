using UnityEngine;

public class AnimationController : MonoBehaviour
{
    private Animator animator;
    private float rotationSpeed = 150f;

    private bool grabLeft = false;
    private bool grabRight = false;

    private float grabHeight = 0.5f;

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

    public void FaceInputDirection(Vector3 inputDirection)
    {
        if (inputDirection.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(inputDirection, Vector3.up);
            Vector3 euler = targetRotation.eulerAngles;
            euler.x = 0f;
            euler.z = 0f;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(euler), 10f * Time.deltaTime);
        }
    }

    public void PunchLeft()
    {
        animator.SetTrigger("PunchLeft");
    }

    public void PunchRight()
    {
        animator.SetTrigger("PunchRight");
    }

    public void StartGrab(bool isLeft)
    {
        animator.SetBool("Grab", true);
        if (isLeft) grabLeft = true;
        else grabRight = true;
    }

    public void UpdateGrabHeight(float normalizedHeight)
    {
        grabHeight = Mathf.Clamp01(normalizedHeight);  // Ensure it's between 0–1
        animator.SetFloat("GrabHeight", grabHeight);
    }

    public void StopGrab(bool isLeft)
    {
        if (isLeft) grabLeft = false;
        else grabRight = false;

        if (!grabLeft && !grabRight)
        {
            animator.SetBool("Grab", false);
        }
    }
}
