using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    public float CurrentHealth { get; private set; }

    private void Awake()
    {
        CurrentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        CurrentHealth = Mathf.Max(0f, CurrentHealth - damage);
        Debug.Log($"Player health: {CurrentHealth}");

        if (CurrentHealth <= 0f)
        {
            Debug.Log("You're dead");
        }
    }

    public void SetHealth(float health)
    {
        CurrentHealth = Mathf.Clamp(health, 0f, maxHealth);
    }
}