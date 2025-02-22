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
        public float grazingWeight;

    }
    public GrazeBehavior GrazingBehavior;
    public BehaviourAndWeight[] behaviors;
    [Tooltip("Flight zone radius (r) where the predator's influence is strongest.")]
    public EscapeBehavior EscapeBehavior;

    public float CalculatePredatorInfluence(float enemyDistance)
    {
        if (enemyDistance >= EscapeBehavior.predatorRange)
            return 0f;

        return 1f - (enemyDistance / EscapeBehavior.predatorRange);
    }

    public override Vector3 CalculateMove(ChozosAI agent, List<Transform> neighbhors, FlockManager flock)
    {
        // Final movement vector
        Vector3 move = Vector3.zero;
        ChozosAI chozo = agent.GetComponent<ChozosAI>();

        float nearestDistance = Mathf.Infinity;
        foreach (Transform predator in flock.Predators)
        {
            float d = Vector3.Distance(agent.transform.position, predator.position);
            if (d < nearestDistance)
                nearestDistance = d;
        }

        if (nearestDistance == Mathf.Infinity)
            nearestDistance = 10000f;


        Vector3 totalMove = Vector3.zero;
        float totalWeight = 0f;
        float predatorPercentage = CalculatePredatorInfluence(nearestDistance); // placeholder; your actual code computes p from predator distance

        Vector3 grazingMove = GrazingBehavior.CalculateMove(agent, neighbhors, flock);
        if (GrazingBehavior.GetChosenPatch(agent) != null)
        {
            chozo.CurrentState = ChozosAI.State.Grazing;
        }
        else
        {
            chozo.CurrentState = ChozosAI.State.Roaming;
        }


        for (int i = 0; i < behaviors.Length; i++)
        {
            BehaviourAndWeight behaviorAndWeight = behaviors[i];
            float weight = GetWeight(chozo, behaviorAndWeight, predatorPercentage);
            Vector3 partialMove = behaviorAndWeight.behaviour.CalculateMove(agent, neighbhors, flock) * weight;

            // Only include behaviors that yield a nonzero result.
            if (partialMove != Vector3.zero)
            {
                totalMove += partialMove;
                totalWeight += weight;
            }

            if (agent.IsDebug)
            {
                DebugGraph.MultiLog("Total Move", DebugGraph.GetUniqueColor(i), partialMove.magnitude, behaviorAndWeight.behaviour.GetType().Name);
            }
        }

        // If some behaviors contributed, return the weighted average.
        if (totalWeight > 0f)
            return totalMove / totalWeight;
        else
            return Vector3.zero;
    }

    public float GetWeight(ChozosAI chozosAI, BehaviourAndWeight behaviourAndWeight, float predatorInfluence)
    {
        if (chozosAI.CurrentState == ChozosAI.State.Roaming)
        {
            return Mathf.Lerp(behaviourAndWeight.weight, behaviourAndWeight.predatorWeight, predatorInfluence);
        }
        else
        {
            return Mathf.Lerp(behaviourAndWeight.grazingWeight, behaviourAndWeight.predatorWeight, predatorInfluence);
        }
        // else
        // {
        //     return behaviourAndWeight.predatorWeight;
        // }
    }
}
