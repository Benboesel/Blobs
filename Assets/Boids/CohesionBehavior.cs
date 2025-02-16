using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Boid/Cohesion")]

public class CohesionBehavior : FlockBehavior
{
    private Vector3 currentVelocity = Vector3.zero;

    public override Vector3 CalculateMove(Unit unit, List<Transform> neighbhors, FlockManager flock)
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
        cohesiveMove = avgPosition - unit.transform.position;
        return cohesiveMove;
    }
}
