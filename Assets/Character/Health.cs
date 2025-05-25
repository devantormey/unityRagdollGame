using UnityEngine;
public class Health : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;

    private RagdollController ragdollController;

    void Start()
    {
        currentHealth = maxHealth;
        ragdollController = GetComponentInChildren<RagdollController>();
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log($"{name} died!");
        if (ragdollController != null)
        {
            ragdollController.ToggleRagdoll(true);
        }
        // Trigger ragdoll death, respawn, or remove
    }
}
