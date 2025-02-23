using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Boid/Cohesion")]

public class CohesionBehavior : FlockBehavior
{
    
    public override Vector3 CalculateMove(ChozosAI agent, List<Transform> neighbhors, List<Transform> enemies, FlockManager flock)
    {
        if (neighbhors.Count == 0)
        {
            return Vector3.zero;
        }

        Vector3 cohesiveMove = Vector3.zero;
        Vector3 avgPosition = Vector3.zero;

        foreach (Transform neighbor in neighbhors)
        {
            avgPosition += neighbor.position;
        }
        avgPosition /= neighbhors.Count;
        cohesiveMove = avgPosition - agent.transform.position;
        return cohesiveMove;
    }
}
