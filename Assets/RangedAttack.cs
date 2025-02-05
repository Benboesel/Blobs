using UnityEngine;

public class RangeAttackBehavior : MonoBehaviour, IAttackBehavior
{
    public GameObject projectilePrefab;
    public Transform firePoint; // The point where the projectile spawns (e.g. a child transform)
    public float damage = 5f;
    public float fireRate = 1f; // Shots per second
    public float projectileSpeed = 10f;

    private float lastFireTime = 0f;

    public void Attack(Unit target)
    {
        if (Time.time >= lastFireTime + (1f / fireRate))
        {
            if (projectilePrefab != null && firePoint != null)
            {
                GameObject projectileObj = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
                Projectile projectile = projectileObj.GetComponent<Projectile>();
                if (projectile != null)
                {
                    projectile.Initialize(target.transform, damage, projectileSpeed);
                }
                lastFireTime = Time.time;
            }
        }
    }
}
