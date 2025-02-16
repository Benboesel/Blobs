using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Boid/Separation")]
public class SeparationBehavior : FlockBehavior
{
    [Tooltip("How close before we start applying the separation force.")]
    public float avoidanceRadius = 1.5f;

    [Tooltip("Softness factor for the inverse-square function. '1' is typical per the paper.")]
    public float softness = 1f;

    public override Vector3 CalculateMove(Unit agent, List<Transform> context, FlockManager flock)
    {
        // If there are no neighbors, no need to adjust movement
        if (context.Count == 0)
            return Vector3.zero;

        Vector3 separationMove = Vector3.zero;
        int nAvoid = 0;

        foreach (Transform neighbor in context)
        {
            Vector3 offset = agent.transform.position - neighbor.position;
            float distSqr = offset.sqrMagnitude;

            // If the neighbor is within the avoidanceRadius, apply separation
            if (distSqr < avoidanceRadius * avoidanceRadius)
            {
                nAvoid++;

                // Inverse-square with softness factor => 1 / (dist^2 + softness^2)
                // If softness = 1, this is 1 / (dist^2 + 1).
                // offset.normalized is direction away from neighbor.
                float denom = distSqr + (softness * softness);
                // Add a small epsilon if you're worried about dividing by zero.
                
                separationMove += (offset / denom); 
            }
        }

        // Average out the total repulsion
        if (nAvoid > 0)
        {
            separationMove /= nAvoid;
        }

        return separationMove;
    }
}
