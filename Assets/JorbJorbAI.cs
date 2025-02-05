using UnityEngine;

public class JorbJorbAI : UnitAI
{
    [Header("Sensing & Patrol")]
    public float SensorRadius = 10f;
    public float PatrolRadius = 15f;  // How far from the main base to patrol

    [Header("Combat")]
    public float AttackRange = 8f;

    // Internal state machine.
    private enum AIState { Patrol, Attack }
    private AIState currentState = AIState.Patrol;

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
        // Look for enemy units (assuming enemy units have the tag "Enemy")
        Collider[] hits = Physics.OverlapSphere(transform.position, SensorRadius, AttackLayer);
        foreach (Collider col in hits)
        {
            Unit potentialEnemy = col.GetComponent<Unit>();
            if (potentialEnemy != null)
            {
                currentTarget = potentialEnemy;
                currentState = AIState.Attack;
                break;
            }
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

    /// <summary>
    /// Attack mode: Move toward the enemy until in attack range and then fire projectiles.
    /// </summary>
    void Attack()
    {
        // If target is lost, return to patrol.
        if (currentTarget == null)
        {
            currentState = AIState.Patrol;
            PickNewPatrolDestination();
            return;
        }

        float distanceToTarget = Vector3.Distance(transform.position, currentTarget.transform.position);
        // if (distanceToTarget > SensorRadius)
        // {
        //     // Lost the enemy—return to patrolling.
        //     currentTarget = null;
        //     currentState = AIState.Patrol;
        //     PickNewPatrolDestination();
        //     return;
        // }

        // Move closer if not yet in attack range.
        if (distanceToTarget > AttackRange)
        {
            moveBehavior.Move(currentTarget.transform.position);
        }
        else
        {
            moveBehavior.Move(transform.position);
            // In range: perform a ranged attack.
            attackBehavior.Attack(currentTarget);
        }
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

    /// <summary>
    /// (Optional) Can be called by other units when a friendly unit is attacked.
    /// </summary>
    public void OnFriendAttacked(Unit friend, Unit attacker)
    {
        if (Vector3.Distance(transform.position, attacker.transform.position) <= SensorRadius)
        {
            currentTarget = attacker;
            currentState = AIState.Attack;
        }
    }
}
