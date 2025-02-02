using System.Collections;
using UnityEngine;

public class Seed : MonoBehaviour
{
    public Food FoodPrefab;
    public float GrowingTime;
    public int MinFoodCount;
    public int MaxFoodCount;
    public float SpawnRadius;


    public IEnumerator Start()
    {
        yield return new WaitForSeconds(GrowingTime);
        SpawnPlants();
        Destroy(this.gameObject);
    }


    public void SpawnPlants()
    {
        int plantCount = Random.Range(MinFoodCount, MaxFoodCount + 1);

        for (int i = 0; i < plantCount; i++)
        {
            Vector2 randomCircle = Random.insideUnitCircle * SpawnRadius;
            Vector3 spawnPosition = transform.position + new Vector3(randomCircle.x, 0, randomCircle.y);
            spawnPosition.y = 0;
            Instantiate(FoodPrefab, spawnPosition, Quaternion.identity);
        }
    }
}
