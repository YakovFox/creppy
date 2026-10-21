using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public enum State
    {
        Idle,
        Walking,
        Attacking,
        Hurt
    }

    [Header("Target and components")]
    [SerializeField] private Transform target;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Animator animator;
    [SerializeField] private PlayerHealth playerHealth;

    [Header("Distance")]
    [SerializeField] private float detectionDistance = 10f;
    [SerializeField] private float attackDistance = 1.8f;

    [Header("Attck")]
    [SerializeField] private float damage = 10f;
    [SerializeField] private float attackCooldown = 1.5f;

    [Header("Damage reaction")]
    [SerializeField] private float hurtDuration = 0.4f;

    public State CurrentState { get; private set; } = State.Idle;

    private float nextAttackTime;
    private float hurtTimer;

    private void Awake()
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        bool canSeeTarget = CanSeeTarget();

        if (IsSeeingPlayer != canSeeTarget)
        {
            IsSeeingPlayer = canSeeTarget;
            CombatMusicManager.SetEnemyAlert(this, canSeeTarget);
        }

        if (!canSeeTarget)
        {
            SetState(State.Idle);
            agent.isStopped = true;
        }
        else
        {
            float distance = Vector3.Distance(transform.position, target.position);

            if (distance <= attackDistance)
            {
                agent.isStopped = true;
                FaceTarget();

                if (Time.time >= nextAttackTime)
                    Attack();
                else
                    SetState(State.Attacking);
            }
            else
            {
                SetState(State.Walking);
                agent.isStopped = false;
                agent.SetDestination(target.position);
            }
        }


        UpdateAnimator();
    }

    public bool IsSeeingPlayer { get; private set; }

    private bool CanSeeTarget()
    {
        Vector3 origin = transform.position + Vector3.up;
        Vector3 destination = target.position + Vector3.up;

        if (Vector3.Distance(origin, destination) > detectionDistance)
            return false;

        Vector3 direction = destination - origin;

        if (Physics.Raycast(origin, direction.normalized, out RaycastHit hit, detectionDistance))
            return hit.transform == target || hit.transform.IsChildOf(target);

        return false;
    }

    private void OnDisable()
    {
        CombatMusicManager.SetEnemyAlert(this, false);
    }

    private void UpdateStateByDistance()
    {
        if (target == null) return;

        float distance = Vector3.Distance(transform.position, target.position);

        if (distance <= attackDistance)
            SetState(State.Attacking);
        else if (distance <= detectionDistance)
            SetState(State.Walking);
        else
            SetState(State.Idle);
    }

    private void Attack()
    {
        SetState(State.Attacking);
        nextAttackTime = Time.time + attackCooldown;

        if (playerHealth != null)
        {
            float multiplier = DifficultySettings.Instance != null
                ? DifficultySettings.Instance.EnemyDamageMultiplier
                : 1f;

            playerHealth.TakeDamage(damage * multiplier);
        }

        if (animator != null)
            animator.SetTrigger("Attack");
    }

    public void ReceiveDamage()
    {
        SetState(State.Hurt);
        hurtTimer = hurtDuration;
        agent.isStopped = true;

        if (animator != null)
            animator.SetTrigger("Hurt");
    }

    private void FaceTarget()
    {
        Vector3 direction = target.position - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude > 0.01f)
        {
            Quaternion rotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                rotation,
                10f * Time.deltaTime
            );
        }
    }

    private void SetState(State newState)
    {
        if (CurrentState == newState) return;

        CurrentState = newState;

        if (animator != null)
        {
            animator.SetBool("Walking", newState == State.Walking);
            animator.SetBool("Attacking", newState == State.Attacking);
        }
    }

    private void UpdateAnimator()
    {
    }
}