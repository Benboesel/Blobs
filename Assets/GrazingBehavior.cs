using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Boid/Graze")]
public class GrazeBehavior : FlockBehavior
{
    [Tooltip("How strongly the sheep are pulled toward the grass patch.")]
    public float grazeAttractionStrength = 1f;

    [Tooltip("Distance within which a sheep is considered to be grazing.")]
    public float grazingDistanceThreshold = 2f;
    public float slowDownZone;
    public float MinGrazingStrength;

    [Tooltip("Radius to search for grass patches.")]
    public float detectionRadius = 20f;
    public LayerMask GrassLayer;
    public float StopEatingDistance;

    // Dictionary to remember which grass patch a given sheep has chosen.
    private Dictionary<ChozosAI, GrassPatch> chosenPatchForUnit = new Dictionary<ChozosAI, GrassPatch>();


    private GrassPatch GetNearestGrassPatch(ChozosAI agent)
    {
        Collider[] grassColliders = Physics.OverlapSphere(agent.transform.position, detectionRadius, GrassLayer);
        if (grassColliders.Length == 0)
            return null;

        float bestScore = 0;
        GrassPatch grassPatch = null;
        foreach (Collider col in grassColliders)
        {
            GrassPatch patch = col.GetComponent<GrassPatch>();
            if (patch != null && !patch.IsDepleted())
            {
                float quality = patch.GetEffectiveQuality();
                float distance = Vector3.Distance(agent.transform.position, patch.transform.position);
                // Higher score for higher quality and shorter distance.
                float score = quality / (1f + distance);
                if (agent.IsDebug)
                {
                    Debug.Log(patch + "  " + score);
                }

                if (score > bestScore)
                {
                    bestScore = score;
                    grassPatch = patch;
                }
            }
        }
        return grassPatch;

    }

    public override Vector3 CalculateMove(ChozosAI agent, List<Transform> neighbhors, List<Transform> enemies, FlockManager flock)
    {
        // If we already have a chosen patch for this agent, use it.
        GrassPatch chosenPatch = null;
        chosenPatchForUnit.TryGetValue(agent, out chosenPatch);
        if (chosenPatch == null)
        {
            chosenPatch = GetNearestGrassPatch(agent);
            if (chosenPatch != null)
            {
                if (agent.IsDebug)
                {
                    Debug.Log("CHOSE PATCH " + chosenPatch);
                }
                chosenPatch.EnterGrazing(agent);
                chosenPatchForUnit[agent] = chosenPatch;
            }
        }

        if (chosenPatch == null) return Vector3.zero;

        Debug.DrawLine(agent.transform.position, chosenPatch.transform.position, Color.red);

        // Calculate the vector toward the chosen patch.
        Vector3 toGrass = chosenPatch.transform.position - agent.transform.position;
        float distanceToPatch = toGrass.magnitude;

        // If already close enough, then we consider the sheep to be grazing.
        if (distanceToPatch < grazingDistanceThreshold)
        {
            return EatGrass(chosenPatch, agent);
        }

        if (agent.IsEating() && (distanceToPatch > StopEatingDistance))
        {
            return Exit(agent);
        }
        return MoveTowardsGrass(chosenPatch, agent, distanceToPatch, toGrass);
    }

    private Vector3 EatGrass(GrassPatch grass, ChozosAI agent)
    {
        agent.SetEating(true);
        // Signal that the agent should stop moving (so it can graze)
        grass.Graze();
        agent.IncreaseFullness(Time.deltaTime);
        if (grass.IsDepleted())
        {
            return Exit(agent);
        }
        return Vector3.zero;
    }

    private Vector3 MoveTowardsGrass(GrassPatch grass, ChozosAI chozo, float distanceToPatch, Vector3 direction)
    {
        float forceMagnitude;
        if (distanceToPatch > slowDownZone)
        {
            forceMagnitude = grazeAttractionStrength;
        }
        else
        {
            // Linear interpolation: when distance equals slowDownZone, force = maxGrazeForce; when equals grazingDistanceThreshold, force = 0.
            forceMagnitude = Mathf.Lerp(MinGrazingStrength, grazeAttractionStrength,
                (distanceToPatch - grazingDistanceThreshold) / (slowDownZone - grazingDistanceThreshold));

        }
        // Return a normalized direction multiplied by the computed magnitude.
        return direction.normalized * forceMagnitude;
    }

    public Vector3 Exit(ChozosAI unit)
    {
        if (unit.IsDebug)
        {
            Debug.Log("EXIT PATCH");
        }
        unit.SetEating(false);
        ClearChosenPatch(unit);
        return Vector3.zero;
    }

    // Public method so other scripts (like the UnitAI) can check which patch this unit is targeting.
    public GrassPatch GetChosenPatch(ChozosAI agent)
    {
        GrassPatch patch = null;
        chosenPatchForUnit.TryGetValue(agent, out patch);
        return patch;
    }

    // Optionally, when a sheep stops grazing or switches state, clear its chosen patch.
    public void ClearChosenPatch(ChozosAI agent)
    {
        if (chosenPatchForUnit.ContainsKey(agent))
        {
            chosenPatchForUnit[agent].ExitGrazing(agent);
            chosenPatchForUnit.Remove(agent);

        }
    }
}
