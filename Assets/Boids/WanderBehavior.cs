using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Boid/Wander")]
public class WanderBehavior : FlockBehavior
{
    public float wanderRadius = 1f;
    public float wanderDistance = 2f;
    public float wanderJitter = 0.2f;
    public float wanderStrength = 0.5f;

    // Each boid might store its own 'wanderTarget' so it's not identical for all boids
    private Dictionary<ChozosAI, Vector3> wanderTargets = new Dictionary<ChozosAI, Vector3>();

    public override Vector3 CalculateMove(ChozosAI agent, List<Transform> neighbhors, List<Transform> enemies, FlockManager flock)
    {
        if (!wanderTargets.ContainsKey(agent))
        {
            // Initialize with a random point on the circle
            wanderTargets[agent] = Random.insideUnitSphere * wanderRadius;
        }

        // Slightly jitter the wander target each frame
        wanderTargets[agent] += new Vector3(
            Random.Range(-1f, 1f) * wanderJitter,
            0,
            Random.Range(-1f, 1f) * wanderJitter
        );

        // Enforce circle boundary
        wanderTargets[agent] = wanderTargets[agent].normalized * wanderRadius;

        // Project circle in front of the boid
        Vector3 circleCenter = agent.transform.position + agent.transform.forward * wanderDistance;
        Vector3 targetPos = circleCenter + wanderTargets[agent];

        // Steer toward that position
        Vector3 wanderForce = (targetPos - agent.transform.position).normalized * wanderStrength;
        wanderForce.y = 0;
        return wanderForce;
    }
}
