using UnityEngine;

public class BasicDamageable : MonoBehaviour, IDamageble
{
    public float MaxHealth = 100f;
    private float defaultScale;
    private float currentHealth;
    private bool isAlive;

    void Start()
    {
        isAlive = true;
        defaultScale = transform.localScale.x;
        currentHealth = MaxHealth;
    }

    public bool TakeDamage(float amount)
    {
        currentHealth -= amount;
        Debug.Log($"{gameObject.name} took {amount} damage, {currentHealth} health remaining.");

        if (currentHealth <= 0)
        {
            Die();
            return true;
        }
        UpdateSize();
        return false;
    }

    private void Die()
    {
        isAlive = false;
        Debug.Log($"{gameObject.name} has died.");
        Destroy(gameObject);
    }

    public void UpdateSize()
    {
        transform.localScale = Vector3.one * (currentHealth / MaxHealth) * defaultScale;
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
        UpdateSize();
    }

    public bool IsAlive()
    {
        return isAlive;
    }
}
