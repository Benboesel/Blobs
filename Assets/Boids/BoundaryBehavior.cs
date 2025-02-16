using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Boid/Boundary")]
public class BoundaryBehavior : FlockBehavior
{
    [Tooltip("Center of the allowed area.")]
    public Vector3 center = Vector3.zero;
    
    [Tooltip("Radius of the allowed area.")]
    public float areaRadius = 50f;
    
    [Tooltip("Strength of the boundary force. Higher values push boids back faster.")]
    public float boundaryStrength = 1f;
    
    public override Vector3 CalculateMove(Unit agent, List<Transform> context, FlockManager flock)
    {
        // Calculate the vector from the agent to the center of the area.
        Vector3 toCenter = center - agent.transform.position;
        float distance = toCenter.magnitude;
        
        // If the agent is inside the allowed area, no boundary force is needed.
        if (distance < areaRadius)
            return Vector3.zero;
        
        // If outside, compute a steering force toward the center.
        // The farther out they are, the stronger the force.
        // You can adjust this scaling for more natural behavior.
        float excess = distance - areaRadius;
        Vector3 steeringForce = toCenter.normalized * excess * boundaryStrength;
        return steeringForce;
    }
}
