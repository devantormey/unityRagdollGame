using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Transform target;  // Your player
    public Vector3 offset = new Vector3(0, 3, -6);
    public float followSpeed = 5f;

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + target.rotation * offset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);

        // Optional: look at player
        transform.LookAt(target.position + Vector3.up * 1.5f);
    }
}
