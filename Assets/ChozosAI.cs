using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Animations;

public class ChozosAI : MonoBehaviour
{
    private IMoveBehavior moveBehavior;
    public GameObject slimePrefab;
    public enum State
    {
        Roaming,
        Grazing,
        Fleeing,
        Follow
    }
    public State CurrentState;
    private bool isEating = false;
    public bool IsDebug = false;
    public Vector3 Velocity;
    public bool IsLatched = false;
    public MeshRenderer Mesh;
    public Color EatingColor;
    private Color defaultColor;
    public float fullness = 0f;
    public float fullnessThreshold = 100f; // When full, sheep will poop.
    public float fullnessRate = 10f;       // Fullness units per second while grazing.
    public float PoopHeight;
    public float PoopAnimationTime;
    public Ease PoopAnimationCurve;
    public float MinPoopRadius;
    public float MaxPoopRadius;
    public Transform FollowTarget;

    void Awake()
    {
        defaultColor = Mesh.material.color;
        moveBehavior = GetComponent<IMoveBehavior>();
    }

    // public IEnumerator Start()
    // {
    //     while (true)
    //     {
    //         Poop();
    //         yield return new WaitForSeconds(3.0f);
    //     }
    // }

    public void SetFollowTarget(Transform target)
    {
        FollowTarget = target;
        CurrentState = State.Follow;
    }

    public void ClearFollowTarget()
    {
        FollowTarget = null;
        // You might want to revert to a default state (e.g., Roaming)
        CurrentState = State.Roaming;
    }

    private Vector3 GetRandomPositionAroundUnit()
    {
        float radius = Mathf.Sqrt(UnityEngine.Random.Range(MinPoopRadius, MaxPoopRadius));

        // Get a random angle in radians
        float angle = UnityEngine.Random.Range(0f, Mathf.PI * 2);

        // Convert polar coordinates to cartesian (x, z)
        float x = Mathf.Cos(angle) * radius;
        float z = Mathf.Sin(angle) * radius;

        // Set spawn height relative to the tree
        return new Vector3(transform.position.x + x, 0f, transform.position.z + z);
    }
    public void Poop()
    {
        Vector3 position = transform.position;
        position.y = 0;
        GameObject poop = Instantiate(slimePrefab, position, Quaternion.identity);
        poop.transform.DOJump(GetRandomPositionAroundUnit(), PoopHeight, 1, PoopAnimationTime).SetEase(PoopAnimationCurve);

    }

    public bool IsEating()
    {
        return isEating;
    }

    public void SetEating(bool eating)
    {
        isEating = eating;
        if (isEating)
        {
            Mesh.material.DOColor(EatingColor, 0.5f).SetEase(Ease.OutQuad);

        }
        else
        {
            Mesh.material.DOColor(defaultColor, 0.5f).SetEase(Ease.OutQuad);
        }
    }

    public void IncreaseFullness(float deltaTime)
    {
        fullness += fullnessRate * deltaTime;
        if (fullness >= fullnessThreshold)
        {
            Poop();
            ResetFullness();
        }
    }

    public void ResetFullness()
    {
        fullness = 0f;
    }
}
