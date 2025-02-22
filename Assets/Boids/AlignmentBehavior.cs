using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Boid/Alignment")]
public class AlignmentBehavior : FlockBehavior
{
    [Tooltip("Radius within which neighbors contribute to alignment.")]
    public float alignmentRadius = 5f;

    public override Vector3 CalculateMove(ChozosAI agent, List<Transform> context, FlockManager flock)
    {
        // If there are no neighbors, no alignment needed
        if (context.Count == 0)
            return Vector3.zero;

        Vector3 alignmentMove = Vector3.zero;
        float count = 0;

        foreach (Transform neighborTransform in context)
        {
            // Check distance if you only want neighbors within alignmentRadius
            float distance = Vector3.Distance(agent.transform.position, neighborTransform.position);
            if (distance > alignmentRadius)
                continue;

            // Try to get the neighbor's velocity
            ChozosAI neighborUnit = neighborTransform.GetComponent<ChozosAI>();
            if (neighborUnit != null)
            {
                // Here we assume the neighbor has a Velocity or some way to get its current movement direction
                // Replace 'neighborUnit.Velocity' with however you're storing the agent's velocity/direction
                alignmentMove += neighborUnit.Velocity; 
                count++;
            }
        }

        // Average the velocity of all neighbors
        if (count > 0)
            alignmentMove /= count;

        return alignmentMove;
    }
}
