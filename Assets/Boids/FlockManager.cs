using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockManager : MonoBehaviour
{
    public CompositeBehaviour compositeBehaviour;
    public float NearbyRadius;
    public LayerMask LayerMask;
    public float Speed;
    public float maxVelocity = 10f; // Set your desired maximum velocity here
    public float rotationSpeed;
    public float PredatorDetectionRadius = 30f;
    public LayerMask PredatorLayerMask; // assign a layer that predators are on
    [HideInInspector] public List<Transform> Predators = new List<Transform>();
    public void Update()
    {
        List<Unit> flock = UnitManager.instance.GetUnitsByType(UnitType.Chozos);
        Predators = GetNearbyPredators(flock);
        foreach (Unit unit in flock)
        {
            List<Transform> context = GetNearbyObjects(unit);
            Vector3 moveVelocity = compositeBehaviour.CalculateMove(unit, context, this);
            moveVelocity *= Speed;

            if(unit.IsLatched)
            {
                moveVelocity *= 0.3f;
            }

            // Clamp the velocity so it doesn't exceed maxVelocity
            moveVelocity = Vector3.ClampMagnitude(moveVelocity, maxVelocity);

            unit.transform.position += moveVelocity * Time.deltaTime;
            unit.velocity = moveVelocity * Time.deltaTime;
            // Rotate agent to face its movement direction (if velocity is not zero)
            if (moveVelocity.sqrMagnitude > 0.01f)
            {
                unit.transform.rotation = Quaternion.Slerp(unit.transform.rotation,
                                                          Quaternion.LookRotation(moveVelocity),
                                                          Time.deltaTime * rotationSpeed);
            }
        }
    }

    public List<Transform> GetNearbyObjects(Unit unit)
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, NearbyRadius, LayerMask);
        List<Transform> sheepNeighbors = new List<Transform>();
        foreach (Collider col in colliders)
        {
            if (col.gameObject != unit.gameObject && col.CompareTag("Sheep"))
            {
                sheepNeighbors.Add(col.transform);
            }
        }
        return sheepNeighbors;
    }

    // New method to get nearby predators:
    public List<Transform> GetNearbyPredators(List<Unit> flock)
    {
        Vector3 flockCenter = Vector3.zero;
        foreach (Unit unit in flock)
        {
            flockCenter += unit.transform.position;
        }
        if (flock.Count > 0)
        {
            flockCenter /= flock.Count;
        }
        Collider[] colliders = Physics.OverlapSphere(flockCenter, PredatorDetectionRadius, PredatorLayerMask);
        List<Transform> predatorTransforms = new List<Transform>();
        foreach (Collider col in colliders)
        {
            predatorTransforms.Add(col.transform);
        }
        return predatorTransforms;
    }

}
