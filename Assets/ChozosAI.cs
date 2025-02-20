using System.Collections;
using System.Collections.Generic;
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

            yield return new WaitForSeconds(Random.Range(MinPoopTime, MaxPoopTime)); // Change movement every few seconds
            Vector3 position = transform.position;
            position.y = 0;
            Instantiate(slimePrefab, position, Quaternion.identity);
        }
    }       
}
