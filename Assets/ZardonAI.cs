using UnityEngine;

public class ZardonAI : UnitAI
{
    [Header("Sensing & Patrol")]
    public float SensorRadius = 10f;
    public float PatrolRadius = 15f;  // How far from the main base to patrol

    [Header("Combat")]
    public float AttackRange = 8f;

    // Internal state machine.
    public enum AIState { Patrol, Chase, Attack }
    public AIState currentState = AIState.Patrol;

    private IAttackBehavior attackBehavior;
    private IMoveBehavior moveBehavior;
    private Unit currentTarget;
    private Vector3 currentPatrolDestination;
    public LayerMask AttackLayer;

    void Awake()
    {
        attackBehavior = GetComponent<IAttackBehavior>();
        moveBehavior = GetComponent<IMoveBehavior>();
        PickNewPatrolDestination();
    }

    void Update()
    {
        switch (currentState)
        {
            case AIState.Patrol:
                Patrol();
                break;
            case AIState.Chase:
                Chase();
                break;
            case AIState.Attack:
                Attack();
                break;
        }
    }

    /// <summary>
    /// Patrol mode: Look for enemies and wander around the main base.
    /// </summary>
    void Patrol()
    {
        Debug.Log("PATROLL");
        LookForCloserTarget();
        if (currentTarget != null)
        {
            currentState = AIState.Chase;
            return;
        }

        // Continue patrolling if no enemy is found.
        float distanceToDestination = Vector3.Distance(transform.position, currentPatrolDestination);
        if (distanceToDestination < 1f)
        {
            PickNewPatrolDestination();
        }
        else
        {
            moveBehavior.Move(currentPatrolDestination);
        }
    }

    public void LookForCloserTarget()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, SensorRadius, AttackLayer);
        foreach (Collider col in hits)
        {
            Unit potentialEnemy = col.GetComponent<Unit>();
            if (potentialEnemy != null)
            {
                currentTarget = potentialEnemy;
            }
        }
    }

    public void Chase()
    {
        Debug.Log("CHAASEE");

        LookForCloserTarget();
        if (currentTarget == null)
        {
            currentState = AIState.Patrol;
            PickNewPatrolDestination();
            return;
        }
        float distanceToTarget = Vector3.Distance(transform.position, currentTarget.transform.position);
        if (distanceToTarget > AttackRange)
        {
            moveBehavior.Move(currentTarget.transform.position);
        }
        else
        {
            currentState = AIState.Attack;
            Attack();
        }
    }

    /// <summary>
    /// Attack mode: Move toward the enemy until in attack range and then fire projectiles.
    /// </summary>
    void Attack()
    {
        Debug.Log("ATTACk");
        // If target is lost, return to patrol.
        if (currentTarget == null)
        {
            moveBehavior.Reset();
            // moveBehavior.Move(transform.position);
            currentState = AIState.Patrol;
            PickNewPatrolDestination();
            return;
        }
        // In range: perform a ranged attack.
        attackBehavior.Attack(currentTarget);
    }

    /// <summary>
    /// Picks a new random destination within patrolRadius around the main base.
    /// </summary>
    void PickNewPatrolDestination()
    {
        Vector2 randomPoint = Random.insideUnitCircle * PatrolRadius;

        currentPatrolDestination = new Vector3(
                  randomPoint.x,
                  transform.position.y,
                  randomPoint.y);
    }
}
