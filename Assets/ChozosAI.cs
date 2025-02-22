using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ChozosAI : MonoBehaviour
{
    private IMoveBehavior moveBehavior;
    public GameObject slimePrefab;
    public float MinPoopTime;
    public float MaxPoopTime;
    public enum State
    {
        Roaming,
        Grazing,
        Fleeing
    }
    public State CurrentState;
    private bool isEating = false;
    public bool IsDebug = false;
    public Vector3 Velocity;
    public bool IsLatched = false;
    public MeshRenderer Mesh;
    public Color EatingColor;
    private Color defaultColor;


    void Awake()
    {
        defaultColor = Mesh.material.color;
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
}
