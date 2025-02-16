using Pathfinding;
using UnityEngine;

public class AStartMoveBehavior : MonoBehaviour, IMoveBehavior
{
    public FollowerEntity Mover;

    public void Move(Vector3 destination)
    {
        Mover.SetDestination(destination);
    }

    public float DistanceToDestination()
    {
        return Mover.remainingDistance;
    }

    public void ClearDestination()
    {

    }

    public void TeleportTo(Vector3 destination)
    {
        Mover.Teleport(destination, true);
    }

    public void Disable()
    {
        Mover.enableLocalAvoidance = false;
        Mover.isStopped = true;
        Mover.canMove = true;
    }

    public void Enable()
    {
        Mover.enableLocalAvoidance = true;
        Mover.isStopped = false;
        Mover.canMove = true;

    }
}
