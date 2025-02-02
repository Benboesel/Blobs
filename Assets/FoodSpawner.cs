using System.Collections;
using UnityEngine;

public class FoodSpawner : MonoBehaviour
{
    public GameObject foodPrefab;
    public int maxFood = 20;
    public float spawnRate = 2f;
    public Vector2 spawnArea = new Vector2(20f, 20f);

    void Start()
    {
        StartCoroutine(SpawnFood());
    }

    IEnumerator SpawnFood()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnRate);
            if (GameObject.FindObjectsOfType<Food>().Length < maxFood)
            {
                Vector3 spawnPos = new Vector3(
                    Random.Range(-spawnArea.x / 2, spawnArea.x / 2),
                    0.5f, // Slightly above the ground
                    Random.Range(-spawnArea.y / 2, spawnArea.y / 2)
                );
                Instantiate(foodPrefab, spawnPos, Quaternion.identity);
            }
        }
    }
}
