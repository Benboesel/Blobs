using UnityEngine;
using UnityEngine.AI;

public class Unit : MonoBehaviour
{
    private IAttackBehavior _attackBehavior;
    private IMoveBehavior _moveBehavior;
    private IDamageble _damageBehavior;
    private UnitAI _ai;
    public UnitType Type;

    void Awake()
    {
        // Assumes only one component implements each interface on this GameObject.
        _attackBehavior = GetComponent<IAttackBehavior>();
        _moveBehavior = GetComponent<IMoveBehavior>();
        _damageBehavior = GetComponent<IDamageble>();
        _ai = GetComponent<UnitAI>();
        _damageBehavior.OnDie += OnDie;
    }

    public void OnDie()
    {
        UnitManager.instance.RemoveUnit(this);
    }

    public bool TakeDamage(float amount)
    {
        return _damageBehavior.TakeDamage(amount);
    }

    public bool IsAlive()
    {
        return _damageBehavior.IsAlive();
    }

    public Vector3 GetTop()
    {
        return transform.TransformPoint(new Vector3(0, 1f, 0));
    }
}
