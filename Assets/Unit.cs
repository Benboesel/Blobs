using UnityEngine;
using UnityEngine.AI;

public class Unit : MonoBehaviour
{
    // Zordon can kill - they come from random direction towards the center, but then when they encounter blob they engage
    //, sucking them, causing them to grow and the blobs to shrink, if you kill them the blobs can eat their corpse.

    // Zordon will look for the nearest blob to kill 
    // 

    // sucker enemies who go for the center to suck the poop
    // 

    // Spawn Rate - increases per range 

    // one can eat slime and get bigger / eating faster because eating is based on their radius / harder to kill

    // defense unit that attacks both the enemies, using points as ammo? 

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
    }

    public bool TakeDamage(float amount)
    {
        return _damageBehavior.TakeDamage(amount);
    }

    public Vector3 GetTop()
    {
        return transform.TransformPoint(new Vector3(0, 0.5f, 0));
    }
}
