using UnityEngine;
using System.Collections.Generic;

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

    // Static collection to track which enemies are already being chased.
    private static HashSet<Unit> targetedEnemies = new HashSet<Unit>();

    void Start()
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
        LookForCloserTarget();
        if (currentTarget != null)
        {
            currentState = AIState.Chase;
            return;
        }

        // Continue patrolling if no enemy is found.
        float distanceToDestination = Vector3.Distance(transform.position, currentPatrolDestination);
        if (distanceToDestination < 0.5f)
        {
            PickNewPatrolDestination();
        }
        else
        {
            moveBehavior.Move(currentPatrolDestination);
        }
    }

    /// <summary>
    /// Searches for the closest enemy that is not already targeted by another Zardon.
    /// </summary>
    public void LookForCloserTarget()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, SensorRadius, AttackLayer);
        Unit closestEnemy = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider col in hits)
        {
            Unit potentialEnemy = col.GetComponent<Unit>();
            if (potentialEnemy != null && potentialEnemy.IsAlive())
            {
                // Skip enemies that are already claimed by another Zardon (unless it's our current target)
                if (targetedEnemies.Contains(potentialEnemy) && potentialEnemy != currentTarget)
                {
                    continue;
                }
                float distance = Vector3.Distance(transform.position, potentialEnemy.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestEnemy = potentialEnemy;
                }
            }
        }

        if (closestEnemy != null)
        {
            // If switching targets, remove the previous one from the claimed set.
            if (currentTarget != null && currentTarget != closestEnemy)
            {
                targetedEnemies.Remove(currentTarget);
            }
            currentTarget = closestEnemy;
            targetedEnemies.Add(currentTarget);
        }
    }

    public void Chase()
    {
        LookForCloserTarget();
        if (currentTarget == null || !currentTarget.IsAlive())
        {
            // Clean up: remove the target from the claimed set if it's dead or lost.
            if (currentTarget != null)
            {
                targetedEnemies.Remove(currentTarget);
            }
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
        // If target is lost, clean up and return to patrol.
        if (currentTarget == null || !currentTarget.IsAlive())
        {
            if (currentTarget != null)
            {
                targetedEnemies.Remove(currentTarget);
            }
            currentState = AIState.Patrol;
            PickNewPatrolDestination();
            return;
        }
        // Attack the target.
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
