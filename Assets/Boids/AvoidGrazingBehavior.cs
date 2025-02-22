using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Boid/AvoidGrazing")]
public class AvoidGrazingBehavior : FlockBehavior
{
    [Tooltip("How close before we start applying the separation force.")]
    public float avoidanceRadius = 1.5f;
    public float Strength;
    [Tooltip("Tuning constant for the exponential ramp (e.g., 4 or 5).")]
    public float rampExponent = 4f;
    public override Vector3 CalculateMove(ChozosAI agent, List<Transform> context, FlockManager flock)
    {
        if (agent.IsEating())
            return Vector3.zero;
        // If there are no neighbors, no need to adjust movement
        if (context.Count == 0)
            return Vector3.zero;

        Vector3 separationMove = Vector3.zero;
        int nAvoid = 0;

        foreach (Transform neighbor in context)
        {
            ChozosAI neighborUnit = neighbor.GetComponent<ChozosAI>();
            // Assume that only sheep that are grazing have isGrazing set to true.
            if (neighborUnit != null && neighborUnit.IsEating())
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

        }
        // Average out the total repulsion
        if (nAvoid > 0)
        {
            separationMove /= nAvoid;
        }

        return separationMove;
    }
}
