using UnityEngine;

public class EnemyContactDamage : MonoBehaviour
{
    [SerializeField] private PlayerHealth player;
    [SerializeField] private float attackDistance = 1.5f;
    [SerializeField] private float damage = 10f;
    [SerializeField] private float attackCooldown = 1f;

    private float nextAttackTime;

    private void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.transform.position);

        if (distance <= attackDistance && Time.time >= nextAttackTime)
        {
            float multiplier = DifficultySettings.Instance != null
                ? DifficultySettings.Instance.EnemyDamageMultiplier
                : 1f;

            player.TakeDamage(damage * multiplier);
            nextAttackTime = Time.time + attackCooldown;
        }
    }
}