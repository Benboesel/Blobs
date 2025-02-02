using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; // For pathfinding

public class Blob : Unit
{
    public float speed = 3f;
    public float DefaultHunger;
    private float hunger = 100f;
    public GameObject slimePrefab;
    private bool seekingFood = false;
    public float MinPoopTime;
    public float MaxPoopTime;
    public float DefaultScale;

    void Start()
    {
        hunger = DefaultHunger;
        Agent.speed = speed;
        PickNewWanderTarget();
        StartCoroutine(WanderRoutine());
        StartCoroutine(SlimePooper());
    }

    public override void OnTakeDamage(float damage)
    {
        float healthPercentage = Health / DefaultHealth;
        transform.localScale = healthPercentage * DefaultScale * Vector3.one;
    }

    IEnumerator SlimePooper()
    {
        while (true)
        {
            Vector3 position = transform.position;
            position.y = 0;
            Instantiate(slimePrefab, position, Quaternion.identity);
            yield return new WaitForSeconds(Random.Range(MinPoopTime, MaxPoopTime)); // Change movement every few seconds
        }
    }

    void Update()
    {
        // LookForFood();
        // float fullness = hunger / DefaultHunger;
        // Mesh.material.color = Color.Lerp(HungryColor, FullColor, fullness);
        // Hunger decay over time
        // hunger -= hungerDecayRate * Time.deltaTime;

        // CheckHealth();
        // If the blob reaches its wander target and isn't seeking food, pick a new one
        if (!Agent.pathPending && Agent.remainingDistance < 0.5f && !seekingFood)
        {
            PickNewWanderTarget();
        }
    }

    // public void CheckHealth()
    // {
    //     if (hunger <= 0)
    //     {
    //         Destroy(gameObject); // Blob dies if it starves
    //     }
    // }

    void PickNewWanderTarget()
    {
        Vector3 randomDirection = new Vector3(
            Random.Range(-GameManager.instance.PlayAreaSize, GameManager.instance.PlayAreaSize),
            0,
            Random.Range(-GameManager.instance.PlayAreaSize, GameManager.instance.PlayAreaSize)
        );
        Agent.SetDestination(randomDirection);
    }

    // void LookForFood()
    // {
    //     Collider[] objectsInRange = Physics.OverlapSphere(transform.position, detectionRadius);
    //     foreach (var col in objectsInRange)
    //     {
    //         Food food = col.GetComponent<Food>(); // Check if object has Food component
    //         if (food != null)
    //         {
    //             agent.SetDestination(col.transform.position);
    //             seekingFood = true;
    //             return;
    //         }
    //     }

    //     // If no food is found, allow wandering to continue
    //     seekingFood = false;
    // }

    // private void OnTriggerEnter(Collider collision)
    // {
    //     Food food = collision.GetComponent<Food>();
    //     if (food != null)
    //     {
    //         hunger += food.nutritionValue; // Gain hunger based on food value
    //         Destroy(food.gameObject); // Eat the food

    //         if (hunger > ReproduceThreshold) // Reproduce if well-fed
    //         {
    //             MakeBaby();
    //         }

    //         PickNewWanderTarget(); // Keep moving after eating
    //     }
    // }

    // public void MakeBaby()
    // {
    //     hunger = DefaultHunger;
    //     Instantiate(blobPrefab, transform.position + (Vector3.one * 0.6f), Quaternion.identity);
    // }

    IEnumerator WanderRoutine()
    {
        while (true)
        {
            if (!seekingFood && !Agent.pathPending && Agent.remainingDistance < 0.5f)
            {
                PickNewWanderTarget();
            }
            yield return new WaitForSeconds(Random.Range(0.5f, 2f)); // Change movement every few seconds
        }
    }

    // public void OnDrawGizmos()
    // {
    //     Gizmos.color = GizmoColor;
    //     Gizmos.DrawWireSphere(transform.position, detectionRadius);
    // }
}
