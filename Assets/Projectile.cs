using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Transform target;
    private float damage;
    private float speed;

    // Call this right after instantiating the projectile.
    public void Initialize(Transform target, float damage, float speed)
    {
        this.target = target;
        this.damage = damage;
        this.speed = speed;
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        // Move towards the target.
        Vector3 direction = (target.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;

        // Check if close enough to "hit" the target.
        if (Vector3.Distance(transform.position, target.position) < 0.5f)
        {
            Unit unit = target.GetComponent<Unit>();
            if (unit != null)
            {
                unit.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
    }
}
