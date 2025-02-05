using System.Collections;
using UnityEngine;

public class ChozosAI : MonoBehaviour
{
    private IMoveBehavior moveBehavior;
    public GameObject slimePrefab;
    public float MinPoopTime;
    public float MaxPoopTime;

    void Awake()
    {
        moveBehavior = GetComponent<IMoveBehavior>();
    }

    public void Start()
    {
        StartCoroutine(SlimePooper());
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
        if (moveBehavior.DistanceToDestination() < 0.5f)
        {
            PickNewWanderTarget();
        }
    }

    void PickNewWanderTarget()
    {
        Vector3 randomDirection = new Vector3(
            Random.Range(-GameManager.instance.PlayAreaSize, GameManager.instance.PlayAreaSize),
            0,
            Random.Range(-GameManager.instance.PlayAreaSize, GameManager.instance.PlayAreaSize)
        );
        moveBehavior.Move(randomDirection);
    }
}
