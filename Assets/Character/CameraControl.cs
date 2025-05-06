using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(-22, 12, 0);
    public float followSpeed = 5f;

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 currentPosition = transform.position;

        // Lock Z to initial value, only update X and Y based on the target
        Vector3 desiredPosition = new Vector3(
            target.position.x + offset.x,
            offset.y ,// Keep current y
            target.position.z + offset.z
        );

        transform.position = Vector3.Lerp(currentPosition, desiredPosition, followSpeed * Time.deltaTime);

        // Lock rotation entirely (no sea sickness)
        transform.rotation = Quaternion.identity;
    }
}
