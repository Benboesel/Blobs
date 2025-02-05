using UnityEngine;
using UnityEngine.AI;

public class NavMeshMoveBehavior : MonoBehaviour, IMoveBehavior
{
    public NavMeshAgent Agent;

    public void Move(Vector3 destination)
    {
        Agent.SetDestination(destination);
    }


    public float DistanceToDestination()
    {
        return Agent.remainingDistance;
    }

    public void Reset()
    {
        Agent.ResetPath();
    }
}
