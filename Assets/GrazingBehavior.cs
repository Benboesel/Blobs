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

    // Dictionary to remember which grass patch a given sheep has chosen.
    private Dictionary<ChozosAI, GrassPatch> chosenPatchForUnit = new Dictionary<ChozosAI, GrassPatch>();

    public override Vector3 CalculateMove(ChozosAI agent, List<Transform> context, FlockManager flock)
    {
        // If we already have a chosen patch for this agent, use it.
        GrassPatch chosenPatch = null;
        if (!chosenPatchForUnit.TryGetValue(agent, out chosenPatch))
        {
            // Otherwise, search for one.
            Collider[] grassColliders = Physics.OverlapSphere(agent.transform.position, detectionRadius, GrassLayer);
            if (grassColliders.Length == 0)
                return Exit(agent);

            float bestScore = 0;
            foreach (Collider col in grassColliders)
            {
                GrassPatch patch = col.GetComponent<GrassPatch>();
                if (patch != null && !patch.IsDepleted())
                {
                    // For example, use a score based on quality and distance.
                    // Here, quality is simply currentGrass/maxGrass and we favor closer patches.
                    float quality = patch.GetEffectiveQuality();
                    float distance = Vector3.Distance(agent.transform.position, patch.transform.position);
                    // Higher score for higher quality and shorter distance.
                    float score = quality / (1f + distance);
                    // Optionally, add some small random noise to avoid ties.
                    // quality += Random.Range(0f, 0.1f);

                    if (quality > bestScore)
                    {
                        bestScore = quality;
                        chosenPatch = patch;
                    }
                }
            }

            if (chosenPatch != null)
            {
                chosenPatch.EnterGrazing(agent);
                chosenPatchForUnit[agent] = chosenPatch;
            }
            else
                return Exit(agent);
        }
        if (agent.IsDebug)
        {
            Debug.Log(chosenPatch);
        }
        if (chosenPatch != null)
        {
            Debug.DrawLine(agent.transform.position, chosenPatch.transform.position, Color.red);
        }

        // Calculate the vector toward the chosen patch.
        Vector3 toGrass = chosenPatch.transform.position - agent.transform.position;
        float distanceToPatch = toGrass.magnitude;

        // If already close enough, then we consider the sheep to be grazing.
        if (distanceToPatch < grazingDistanceThreshold)
        {
            agent.SetEating(true);
            // Signal that the agent should stop moving (so it can graze)
            chosenPatch.Graze();
            if (chosenPatch.IsDepleted())
            {
                chosenPatch.ExitGrazing(agent);
                return Exit(agent);
            }
            return Vector3.zero;
        }
        else
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
            return toGrass.normalized * forceMagnitude;
        }
    }

    public Vector3 Exit(ChozosAI unit)
    {
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
            chosenPatchForUnit.Remove(agent);
    }
}
