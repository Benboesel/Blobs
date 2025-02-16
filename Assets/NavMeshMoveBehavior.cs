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

    public void ClearDestination()
    {
        Agent.ResetPath();
    }

    public void TeleportTo(Vector3 destination)
    {
        throw new System.NotImplementedException();
    }

    public void Disable()
    {
        throw new System.NotImplementedException();
    }

    public void Enable()
    {
        throw new System.NotImplementedException();
    }
}
