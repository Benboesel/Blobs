using DG.Tweening;
using UnityEngine;

public class ChumboAI : UnitAI
{
    private IMoveBehavior moveBehavior;
    public float MinRadius;
    public float MaxRadius;
    public float MinHeight;
    public float MaxHeight;
    public Ease AnimationCurve;
    public float AnimationDuration;
    public float IncubationTime;

    void Start()
    {
        moveBehavior = GetComponent<IMoveBehavior>();
        PickNewWanderTarget();
    }


    void Update()
    {
        if (moveBehavior.DistanceToDestination() < 0.5f)
        {
            PickNewWanderTarget();
        }
    }

    public void AddSeed(UnitSeed seed)
    {
        seed.transform.SetParent(this.transform);
        seed.transform.DOLocalMove(GetRandomPositionInside(), AnimationDuration).SetEase(AnimationCurve);
        seed.Incubate(IncubationTime);
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

    private Vector3 GetRandomPositionInside()
    {
        float radius = UnityEngine.Random.Range(MinRadius, MaxRadius);

        // Get a random angle in radians
        float angle = UnityEngine.Random.Range(0f, Mathf.PI * 2);

        // Convert polar coordinates to cartesian (x, z)
        float x = Mathf.Cos(angle) * radius;
        float z = Mathf.Sin(angle) * radius;

        float height = UnityEngine.Random.Range(MinHeight, MaxHeight);

        // Set spawn height relative to the tree
        return new Vector3(x, height, z);
    }
}
