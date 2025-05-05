using UnityEngine;

public class HandTrigger : MonoBehaviour
{
    public Rigidbody targetRigidbody;

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collided with something, " + other.name);
        if (other.attachedRigidbody != null && !other.isTrigger)
        {
            targetRigidbody = other.attachedRigidbody;
            Debug.Log("Found A body to attach to");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.attachedRigidbody == targetRigidbody)
        {
            targetRigidbody = null;
        }
    }
}
