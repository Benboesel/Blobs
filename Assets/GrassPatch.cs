using System.Collections;
using System.Collections.Generic;
using Unity.Entities.UniversalDelegates;
using UnityEngine;

public class GrassPatch : MonoBehaviour
{
    public float maxGrass = 100f;
    public float currentGrass = 100f;
    public float depletionRate = 5f;
    public float regrowthRate = 1f;

    // Use a HashSet to track which units are grazing on this patch.
    public List<ChozosAI> grazingUnits = new List<ChozosAI>();
    public Color fullColor;
    public Color emptyColor;
    public List<MeshRenderer> Meshes;
    public float RegrowthDelay;
    private float timeOfLastBite;

    // Call this when a unit starts grazing on this patch.
    public void EnterGrazing(ChozosAI unit)
    {
        // Debug.Log("CHOSE PATCH " + unit);
        // Adds the unit if it isn't already in the set.
        if (!grazingUnits.Contains(unit))
        {
            grazingUnits.Add(unit);
        }
    }

    // Call this when a unit stops grazing on this patch.
    public void ExitGrazing(ChozosAI unit)
    {
        // Debug.Log("EXIT PATCH " + unit);

        if (grazingUnits.Contains(unit))
        {
            grazingUnits.Remove(unit);
        }
    }

    // Returns a modified quality that decreases as more units are grazing.
    public float GetEffectiveQuality(float occupancyFactor = 2f)
    {
        if (grazingUnits.Count > 0) return 0;
        float quality = currentGrass / maxGrass;
        return quality;
        // return quality / (1f + occupancyFactor * grazingUnits.Count);
    }

    // Deplete grass as it's eaten.
    public void Graze()
    {
        currentGrass -= depletionRate * Time.deltaTime;

        UpdateVisuals();
        timeOfLastBite = Time.time;
        if (currentGrass <= 0f)
        {
            currentGrass = 0f;
        }
    }

    public void UpdateVisuals()
    {
        Vector3 scale = transform.localScale;
        float percentage = (currentGrass / maxGrass);
        scale.y = Mathf.Lerp(.05f, 1.0f, percentage);
        transform.localScale = scale;
        Color color = Color.Lerp(emptyColor, fullColor, percentage);
        foreach (MeshRenderer mesh in Meshes)
        {
            mesh.material.color = color;
        }
    }

    // Indicates when the patch is fully depleted.
    public bool IsDepleted()
    {
        return currentGrass == 0f;
    }

    public void Regrow()
    {
        // Optional regrowth logic.
        if (currentGrass < maxGrass)
        {
            currentGrass += regrowthRate * Time.deltaTime;
            currentGrass = Mathf.Min(currentGrass, maxGrass);
            UpdateVisuals();
        }
    }
    void Update()
    {
        if (Time.time - timeOfLastBite > RegrowthDelay)
        {
            Regrow();
        }
    }
}
