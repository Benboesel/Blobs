using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Boid/Follow")]
public class FollowBehavior : FlockBehavior
{
    public float slowDownZone;
    public float stopDistance;
    public float Strength;
    public override Vector3 CalculateMove(ChozosAI unit, List<Transform> neighbhors, List<Transform> enemies, FlockManager flock)
    {
        if (unit.FollowTarget == null) return Vector3.zero;
        // Calculate the vector toward the chosen patch.
        Vector3 directionToPlayer = unit.FollowTarget.transform.position - unit.transform.position;
        float distanceToPatch = directionToPlayer.magnitude;
        float forceMagnitude;

        if (distanceToPatch > slowDownZone)
        {
            forceMagnitude = Strength;
        }
        else
        {
            // Linear interpolation: when distance equals slowDownZone, force = maxGrazeForce; when equals grazingDistanceThreshold, force = 0.
            forceMagnitude = Mathf.Lerp(0f, Strength,
                (distanceToPatch - stopDistance) / (slowDownZone - stopDistance));

        }
        // Return a normalized direction multiplied by the computed magnitude.
        return directionToPlayer.normalized * forceMagnitude;
    }
}
