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
    public LayerMask PredatorLayerMask; // assign a layer that predators are on

    public void LateUpdate()
    {
        List<Unit> chozos = UnitManager.instance.GetUnitsByType(UnitType.Chozos);

        List<ChozosAI> flock = new List<ChozosAI>();
        foreach (Unit unit in chozos)
        {
            flock.Add(unit.GetComponent<ChozosAI>());
        }
        foreach (ChozosAI unit in flock)
        {
            RoamOrFlee(unit);
        }
    }

    public void RoamOrFlee(ChozosAI unit)
    {
        List<Transform> context = GetNearbyObjects(unit);
        List<Transform> predators = GetNearbyPredators(unit);
        Vector3 moveVelocity = compositeBehaviour.CalculateMove(unit, context, predators, this);
        moveVelocity *= Speed;

        if (unit.IsLatched)
        {
            moveVelocity *= 0.3f;
        }
        moveVelocity.y = 0;
        moveVelocity = Vector3.ClampMagnitude(moveVelocity, maxVelocity);

        // Clamp the velocity so it doesn't exceed maxVelocity
        // moveVelocity = Vector3.ClampMagnitude(moveVelocity, maxVelocity);
        unit.transform.position += moveVelocity * Time.deltaTime;
        unit.Velocity = moveVelocity;
        // Rotate agent to face its movement direction (if velocity is not zero)
        if (moveVelocity.sqrMagnitude > 0.01f)
        {
            unit.transform.rotation = Quaternion.Slerp(unit.transform.rotation,
                                                      Quaternion.LookRotation(moveVelocity),
                                                      Time.deltaTime * rotationSpeed);
        }
    }

    public List<Transform> GetNearbyObjects(ChozosAI unit)
    {
        Collider[] colliders = Physics.OverlapSphere(unit.transform.position, NearbyRadius, LayerMask);
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
    public List<Transform> GetNearbyPredators(ChozosAI unit)
    {
        Collider[] colliders = Physics.OverlapSphere(unit.transform.position, compositeBehaviour.EscapeBehavior.predatorRange, PredatorLayerMask);
        List<Transform> enemies = new List<Transform>();
        foreach (Collider col in colliders)
        {
            enemies.Add(col.transform);
        }
        return enemies;
    }

}
