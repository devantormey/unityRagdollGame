using UnityEngine;
using System; 

public class AnimationController : MonoBehaviour
{
    private Animator animator;
    // private float rotationSpeed = 150f;

    private bool grabLeft = false;
    private bool grabRight = false;

    private float grabHeight = 0.5f;
    private float attackProgress = 0.0f;
    private bool isAttacking = false;
    private bool hitMidpoint = false;

    public Action OnPunchImpulse;
    //movement
    public float rotationSpeed = 3f; // Exposed in the inspector for tuning

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
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(euler), rotationSpeed * Time.deltaTime);
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

    //This should consume an enumaration based on equipment and trigger the corresponding animation
    public void StartAttack()
    {
        animator.SetBool("isAttacking", true);
        isAttacking = true;
    }

    public void StartJump(){
        animator.SetBool("isJumping",true);
    }
    public void StopJump(){
        animator.SetBool("isJumping",false);
    }

    public void UpdateGrabHeight(float normalizedHeight)
    {
        grabHeight = Mathf.Clamp01(normalizedHeight);  // Ensure it's between 0–1
        animator.SetFloat("GrabHeight", grabHeight);
    }
    public void UpdateAttackProgress(float normalizedHeight)
    {
        attackProgress = Mathf.Clamp01(normalizedHeight);  // Ensure it's between 0–1
        animator.SetFloat("AttackProgress", attackProgress);
        if (isAttacking && attackProgress <= 0.1){
            hitMidpoint = true;
        }
        if (hitMidpoint && isAttacking && attackProgress >= 0.5){
            //apply force in punch direction proportional to strength
            OnPunchImpulse?.Invoke();  // Call the action if assigned
            //set midpoint false so this only happens once
            hitMidpoint = false;
        }
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
    public void StopAttack()
    {
        animator.SetBool("isAttacking", false);
        isAttacking = false;
        hitMidpoint = false;
    }
}
