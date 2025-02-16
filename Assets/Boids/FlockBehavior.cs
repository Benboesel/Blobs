using System.Collections.Generic;
using UnityEngine;

public abstract class FlockBehavior : ScriptableObject
{
    public bool hasSecondMultiplier;

    public abstract Vector3 CalculateMove(Unit unit, List<Transform> neighbhors, FlockManager flock);
}
