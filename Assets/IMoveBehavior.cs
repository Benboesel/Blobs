using UnityEngine;

public interface IMoveBehavior
{
    public void Move(Vector3 destination);

    public float DistanceToDestination();
    public void Reset();
}
