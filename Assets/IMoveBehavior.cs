using UnityEngine;

public interface IMoveBehavior
{
    public void Move(Vector3 destination);

    public float DistanceToDestination();
    public void ClearDestination();
    public void TeleportTo(Vector3 destination);
    public void Disable();
    public void Enable();
}
