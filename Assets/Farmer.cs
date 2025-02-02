using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Farmer : MonoBehaviour
{
    public Seed SeedPrefab;
    private NavMeshAgent agent;
    public float speed = 3f;
    public float MinFoodDropTime;
    public float MaxFoodDropTime;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = speed;
        PickNewWanderTarget();
        StartCoroutine(WanderRoutine());
        StartCoroutine(FoodDropRoutine());
    }

    void PickNewWanderTarget()
    {
        Vector3 randomDirection = new Vector3(
            Random.Range(-GameManager.instance.PlayAreaSize, GameManager.instance.PlayAreaSize),
            0,
            Random.Range(-GameManager.instance.PlayAreaSize, GameManager.instance.PlayAreaSize)
        );
        agent.SetDestination(randomDirection);
    }

    IEnumerator FoodDropRoutine()
    {
        while (true)
        {
            Vector3 position = transform.position;
            position.y = 0;
            Instantiate(SeedPrefab, position, Quaternion.identity);
            yield return new WaitForSeconds(Random.Range(MinFoodDropTime, MaxFoodDropTime)); // Change movement every few seconds
        }
    }
    IEnumerator WanderRoutine()
    {
        while (true)
        {
            if (!agent.pathPending && agent.remainingDistance < 0.5f)
            {
                PickNewWanderTarget();
            }
            yield return new WaitForSeconds(Random.Range(0.5f, 2f)); // Change movement every few seconds
        }
    }
}
