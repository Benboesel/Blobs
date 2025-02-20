using System.Collections;
using UnityEngine;
using DG.Tweening;

public class UnitSeed : Pickupable
{
    public UnitType Type;
    public MeshRenderer Mesh;
    public Transform Inside;
    public float PopOffRadiusMin;
    public float PopOffRadiusMax;
    public float PopOffHeight;
    public float AnimationTime;
    public Ease AnimationCurve;
    public Collider Collider;

    public void Initialize(float growingTime)
    {
        StartCoroutine(Grow(growingTime));
    }

    private IEnumerator Grow(float growingTime)
    {
        Inside.localScale = Vector3.one * 0.01f;
        Inside.DOScale(.6f, growingTime);
        yield return new WaitForSeconds(growingTime);
        Transform tree = transform.parent.transform;
        Vector3 direction = transform.position - tree.position;
        direction.y = 0;

        Vector3 floorPosition = tree.position + (direction.normalized * UnityEngine.Random.Range(PopOffRadiusMin, PopOffRadiusMax));
        transform.DOJump(floorPosition, PopOffHeight, 1, AnimationTime).SetEase(AnimationCurve);
        Collider.enabled = true;

    }

    public void Incubate(float growingTime)
    {
        StartCoroutine(IncubateCoroutine(growingTime));
    }

    private IEnumerator IncubateCoroutine(float growingTime)
    {
        Inside.DOScale(1f, growingTime);
        transform.DOScale(1.25f, growingTime);
        yield return new WaitForSeconds(growingTime);
        transform.SetParent(null);
        Vector3 startPosition = GetRandomPositionAroundSeed();
        transform.DOJump(startPosition, PopOffHeight, 1, AnimationTime).OnComplete(() => SpawnUnit());
    }

    public void SpawnUnit()
    {
        Unit unit = UnitManager.instance.SpawnUnit(Type, transform.position);
        Destroy(gameObject);
    }

    private Vector3 GetRandomPositionAroundSeed()
    {
        float radius = Mathf.Sqrt(UnityEngine.Random.Range(4, 8));

        // Get a random angle in radians
        float angle = UnityEngine.Random.Range(0f, Mathf.PI * 2);

        // Convert polar coordinates to cartesian (x, z)
        float x = Mathf.Cos(angle) * radius;
        float z = Mathf.Sin(angle) * radius;

        // Set spawn height relative to the tree
        return new Vector3(transform.position.x + x, 0f, transform.position.z + z);
    }
}
