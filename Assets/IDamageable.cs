using UnityEngine;

public interface IDamageble
{
    public bool IsAlive();
    public bool TakeDamage(float damage);
    public void Heal(float amount);
}
