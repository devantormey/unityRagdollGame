using UnityEngine;

public class RagdollHitbox : MonoBehaviour
{
    public Health ownerHealth;
    public float damageMultiplier = 1f;
    public float minDamageImpulse = 5f;

    void OnCollisionEnter(Collision collision)
    {
        float impactForce = collision.impulse.magnitude;

        if (impactForce >= minDamageImpulse)
        {
            float damage = impactForce * damageMultiplier;
            ownerHealth?.TakeDamage(damage);

            // Optional: Visualize or debug
            Debug.Log($"{gameObject.name} took {damage} damage (impulse: {impactForce})");
        }
    }
}

