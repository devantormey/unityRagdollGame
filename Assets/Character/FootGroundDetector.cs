using UnityEngine;

public class FootGroundDetector : MonoBehaviour
{
    public bool isGrounded = false;

    void OnTriggerEnter(Collider other)
    {
        if (!other.isTrigger)
            isGrounded = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.isTrigger)
            isGrounded = false;
    }
}
