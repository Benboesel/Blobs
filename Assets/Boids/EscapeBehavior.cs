using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Boid/Escape")]
public class EscapeBehavior : FlockBehavior
{
    [Tooltip("Tag of the predator object(s).")]
    public string predatorTag = "Predator";

    [Tooltip("Softness factor from the paper. The force is 1 / (distance^2 + softness^2).")]
    public float softness = 10f;

    [Tooltip("Only apply escape force if predator is within this distance (optional).")]
    public float predatorRange = 20f;
    public float Fear;

    public override Vector3 CalculateMove(ChozosAI agent, List<Transform> neighbhors, FlockManager flock)
    {
        // Get the list of nearby predators from the flock manager
        if (flock.Predators.Count == 0)
            return Vector3.zero;

        Vector3 escapeForce = Vector3.zero;
        int predatorCount = 0;

        foreach (Transform predatorTransform in flock.Predators)
        {
            Vector3 offset = agent.transform.position - predatorTransform.position;
            float distSqr = offset.sqrMagnitude;

            // Only consider predators within the defined range.
            if (distSqr > predatorRange * predatorRange)
                continue;

            // Avoid division by zero.
            if (distSqr < 0.0001f)
                distSqr = 0.0001f;

            // Calculate the inverse-square repulsion force (with softness).
            float forceMag = Fear / (distSqr + softness * softness);

            escapeForce += offset.normalized * forceMag;
            predatorCount++;
        }

        if (predatorCount > 0)
            escapeForce /= predatorCount;
        escapeForce.y = 0;
        return escapeForce;
    }
}
