using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "Boid/Composite")]
public class CompositeBehaviour : FlockBehavior
{
    [System.Serializable]
    public struct BehaviourAndWeight
    {
        public FlockBehavior behaviour;
        public float weight;
        public float predatorWeight;

    }

    public BehaviourAndWeight[] behaviors;
    [Tooltip("Flight zone radius (r) where the predator's influence is strongest.")]
    public float flightZoneRadius;

    [Tooltip("Scale value for the sigmoid function (typically 20).")]
    public float multiplierScale = 20f;

    public float CalculatePredatorInfluence(float enemyDistance)
    {
        if (enemyDistance >= flightZoneRadius)
            return 0f;

        return 1f - (enemyDistance / flightZoneRadius);
    }

    public override Vector3 CalculateMove(Unit agent, List<Transform> context, FlockManager flock)
    {
        // Final movement vector
        Vector3 move = Vector3.zero;


        float nearestDistance = Mathf.Infinity;
        foreach (Transform predator in flock.Predators)
        {
            float d = Vector3.Distance(agent.transform.position, predator.position);
            if (d < nearestDistance)
                nearestDistance = d;
        }

        if (nearestDistance == Mathf.Infinity)
            nearestDistance = 10000f;

        // float p = (1f / Mathf.PI) * Mathf.Atan((flightZoneRadius - nearestDistance) / multiplierScale) + 0.5f;
        float p = CalculatePredatorInfluence(nearestDistance);
        // Sum all behaviors
        for (int i = 0; i < behaviors.Length; i++)
        {
            BehaviourAndWeight b = behaviors[i];
            float weight = Mathf.Lerp(b.weight, b.predatorWeight, p);

            Vector3 partialMove = b.behaviour.CalculateMove(agent, context, flock) * weight;

            // Add partial move
            move += partialMove;
        }

        return move;
    }
}
