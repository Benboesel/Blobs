using System;
using UnityEngine;

public interface IDamageble
{
    public bool IsAlive();
    public bool TakeDamage(float damage);
    public void Heal(float amount);
    public void Die();
    public event Action OnDie;
}
