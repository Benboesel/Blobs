using UnityEngine;

public class CombatUnit : Unit
{
    public LayerMask AttackLayer;
    public float DetectionRadius;
    public float AttackRange;
    public Unit Target;

    public void Update()
    {
        if(Target != null)
        {
            Agent.SetDestination(Target.transform.position);
        }
    }
}
