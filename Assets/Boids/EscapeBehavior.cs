using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Boid/Escape")]
public class EscapeBehavior : FlockBehavior
{
    [Tooltip("How close before we start applying the separation force.")]
    public float avoidanceRadius = 1.5f;
    public float Strength;
    [Tooltip("Tuning constant for the exponential ramp (e.g., 4 or 5).")]
    public float rampExponent = 4f;

    public override Vector3 CalculateMove(ChozosAI agent, List<Transform> neighbhors, List<Transform> enemies, FlockManager flock)
    {

        // If there are no neighbors, no need to adjust movement
        if (enemies.Count == 0)
            return Vector3.zero;

        Vector3 separationMove = Vector3.zero;
        int nAvoid = 0;

        foreach (Transform neighbor in enemies)
        {
            Vector3 offset = agent.transform.position - neighbor.position;
            float distance = offset.magnitude;

            // Only consider neighbors within the avoidance radius.
            if (distance < avoidanceRadius)
            {
                // Compute closeness factor (t=0 when at edge, t=1 when touching).
                float t = Mathf.Clamp01(1 - (distance / avoidanceRadius));

                // Compute an exponential ramp-up. When t=0, force=0; when t=1, force = exp(k) - 1.
                float forceMagnitude = Mathf.Exp(t * rampExponent) - 1f;

                // Add the contribution from this neighbor.
                separationMove += offset.normalized * forceMagnitude * Strength;
                nAvoid++;
            }
        }
        // Average out the total repulsion
        if (nAvoid > 0)
        {
            separationMove /= nAvoid;
        }

        return separationMove;


        // // Get the list of nearby predators from the flock manager
        // if (enemies.Count == 0)
        //     return Vector3.zero;

        // Vector3 escapeForce = Vector3.zero;
        // int predatorCount = 0;

        // foreach (Transform predatorTransform in enemies)
        // {
        //     Vector3 offset = agent.transform.position - predatorTransform.position;
        //     float distSqr = offset.sqrMagnitude;

        //     // Only consider predators within the defined range.
        //     if (distSqr > predatorRange * predatorRange)
        //         continue;

        //     // Avoid division by zero.
        //     if (distSqr < 0.0001f)
        //         distSqr = 0.0001f;

        //     // Calculate the inverse-square repulsion force (with softness).
        //     float forceMag = Fear / (distSqr + softness * softness);

        //     escapeForce += offset.normalized * forceMag;
        //     predatorCount++;
        // }

        // if (predatorCount > 0)
        //     escapeForce /= predatorCount;
        // escapeForce.y = 0;
        // return escapeForce;
    }
}
