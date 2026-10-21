using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private float health = 50f;
    [SerializeField] private EnemyAI enemyAI;

    private void Awake()
    {
        if (enemyAI == null)
            enemyAI = GetComponent<EnemyAI>();
    }

    public void TakeDamage(float damage)
    {
        health -= damage;

        if (enemyAI != null)
            enemyAI.ReceiveDamage();

        if (health <= 0f)
            Destroy(gameObject);
    }
}