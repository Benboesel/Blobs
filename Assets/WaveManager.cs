using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // Needed for TextMeshProUGUI

// Simple enum for spawn directions.
public enum SpawnDirection
{
    North,
    South,
    East,
    West
}

// A grouping within a wave (e.g., 5 Zardons from the North).
[System.Serializable]
public class WaveGroup
{
    [Tooltip("Type of unit to spawn.")]
    public UnitType unitType;
    
    [Tooltip("Number of units to spawn in this group.")]
    public int count;
    
    [Tooltip("Direction from which these units will spawn.")]
    public SpawnDirection spawnDirection;
}

// A wave composed of one or more groups.
[System.Serializable]
public class Wave
{
    [Tooltip("List of unit groupings for this wave.")]
    public List<WaveGroup> groups;
}

public class WaveManager : MonoBehaviour
{
    [Header("Wave Settings")]
    [Tooltip("Time (in seconds) between each wave.")]
    [SerializeField] private float waveInterval = 60f;
    
    [Tooltip("List of waves with their specific unit groupings.")]
    [SerializeField] private List<Wave> waves = new List<Wave>();
    
    [Tooltip("Time until the first wave spawns.")]
    [SerializeField] public float timeTillFirstWave = 10f; 

    [Header("Spawn Position Settings")]
    [Tooltip("Reference point for spawning. If left empty, this object's position is used.")]
    [SerializeField] private Transform spawnOrigin;
    
    [Tooltip("Distance from the spawn origin in the chosen direction.")]
    [SerializeField] private float spawnDistance = 10f;

    [Header("UI Settings")]
    [Tooltip("Text that displays the time until next wave.")]
    [SerializeField] private TextMeshProUGUI waveCountdownText;

    // Track the current wave index.
    private int currentWaveIndex = 0;
    // Stores the absolute time when the next wave will spawn.
    private float nextWaveSpawnTime;

    private void Start()
    {
        // Set the next wave time using timeTillFirstWave.
        nextWaveSpawnTime = Time.time + timeTillFirstWave;
        StartCoroutine(SpawnWaves());
    }

    private IEnumerator SpawnWaves()
    {
        if (waves.Count == 0)
        {
            Debug.LogWarning("No waves defined in the WaveManager!");
            yield break;
        }
        
        // Wait for the first wave.
        yield return new WaitForSeconds(timeTillFirstWave);
        SpawnWave(waves[currentWaveIndex]);
        currentWaveIndex++;
        // Set timer for next wave.
        nextWaveSpawnTime = Time.time + waveInterval;

        // Continually spawn waves.
        while (true)
        {
            yield return new WaitForSeconds(waveInterval);
            SpawnWave(waves[currentWaveIndex]);
            currentWaveIndex++;
            // If you want to loop waves, you could add a check here.
            nextWaveSpawnTime = Time.time + waveInterval;
        }
    }
    
    private void SpawnWave(Wave wave)
    {
        Debug.Log($"Spawning Wave {currentWaveIndex}");
        
        foreach (WaveGroup group in wave.groups)
        {
            // Determine the base spawn position for this group.
            Vector3 basePosition = CalculateSpawnPosition(group.spawnDirection);
            
            // Spawn the specified number of units for this group.
            for (int i = 0; i < group.count; i++)
            {
                // Add a small random offset so the units don't spawn right on top of each other.
                Vector3 offset = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
                Vector3 spawnPos = basePosition + offset;
                UnitManager.instance.SpawnUnit(group.unitType, spawnPos);
            }
            
            Debug.Log($"Spawned {group.count} units of type {group.unitType} from {group.spawnDirection}");
        }
    }

    // Calculate a spawn position based on the given direction.
    private Vector3 CalculateSpawnPosition(SpawnDirection direction)
    {
        Vector3 origin = (spawnOrigin != null) ? spawnOrigin.position : transform.position;
        switch (direction)
        {
            case SpawnDirection.North:
                return origin + new Vector3(0, 0, spawnDistance);
            case SpawnDirection.South:
                return origin + new Vector3(0, 0, -spawnDistance);
            case SpawnDirection.East:
                return origin + new Vector3(spawnDistance, 0, 0);
            case SpawnDirection.West:
                return origin + new Vector3(-spawnDistance, 0, 0);
            default:
                return origin;
        }
    }

    private void Update()
    {
        if (waveCountdownText != null)
        {
            // Calculate how much time is left until the next wave.
            float timeRemaining = nextWaveSpawnTime - Time.time;
            if (timeRemaining < 0)
                timeRemaining = 0;
            
            // Update the text to show the remaining time in seconds.
            waveCountdownText.text = $"{timeRemaining:F0}s";
            
            // As timeRemaining approaches 0, interpolate the text color from white to red.
            // When timeRemaining equals waveInterval, t = 0 (white); when it's 0, t = 1 (red).
            float t = 1 - Mathf.Clamp01(timeRemaining / 20f);
            waveCountdownText.color = Color.Lerp(Color.white, Color.red, t);
        }
    }
}
