using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public enum UnitType
{
    Chozos,
    JorbJorb,
    PlowPlow,
    Chumbo
}


public class UnitManager : Singleton<UnitManager>
{
    [Serializable]
    public class UnitPrefabPair
    {
        public UnitType unitType;
        public Unit prefab;
    }

    [SerializeField] public List<UnitPrefabPair> unitPrefabs = new List<UnitPrefabPair>();
    private Dictionary<UnitType, Unit> unitPrefabDictionary;

    [Header("Unit Counters")]
    public TextMeshProUGUI ChozosCountText;
    public TextMeshProUGUI JorbCountText;
    public TextMeshProUGUI PlowCountText;
    public TextMeshProUGUI ChumboCountText;

    private Dictionary<UnitType, List<Unit>> unitLists = new Dictionary<UnitType, List<Unit>>();
    private Dictionary<UnitType, TextMeshProUGUI> unitCountTexts = new Dictionary<UnitType, TextMeshProUGUI>();

    public void Start()
    {
        // Convert list into a dictionary for fast lookup
        unitPrefabDictionary = new Dictionary<UnitType, Unit>();
        foreach (UnitPrefabPair pair in unitPrefabs)
        {
            if (!unitPrefabDictionary.ContainsKey(pair.unitType))
            {
                Debug.Log("ADDING " + pair.unitType + "   " + pair.prefab);
                unitPrefabDictionary.Add(pair.unitType, pair.prefab);
            }
            else
            {
                Debug.LogWarning($"Duplicate UnitType detected: {pair.unitType}");
            }
        }

        // Initialize unit lists
        unitLists[UnitType.Chozos] = new List<Unit>();
        unitLists[UnitType.JorbJorb] = new List<Unit>();
        unitLists[UnitType.PlowPlow] = new List<Unit>();
        unitLists[UnitType.Chumbo] = new List<Unit>();

        // Map unit types to corresponding UI text fields
        unitCountTexts[UnitType.Chozos] = ChozosCountText;
        unitCountTexts[UnitType.JorbJorb] = JorbCountText;
        unitCountTexts[UnitType.PlowPlow] = PlowCountText;
        unitCountTexts[UnitType.Chumbo] = ChumboCountText;
        FindExistingUnits();
        // Initialize text display
        UpdateAllUnitCounts();
    }
    public List<Unit> GetUnitsByType(UnitType unitType)
    {
        if (unitLists.TryGetValue(unitType, out List<Unit> units))
        {
            return new List<Unit>(units); // Return a copy to prevent modification of the original list
        }

        Debug.LogWarning($"No units found for UnitType: {unitType}");
        return new List<Unit>(); // Return an empty list if the type doesn't exist
    }
    
    private void FindExistingUnits()
    {
        foreach (Unit unit in FindObjectsByType<Unit>(FindObjectsSortMode.None))
        {
            AddUnit(unit);
        }
    }

    public Unit GetPrefab(UnitType unitType)
    {
        if (unitPrefabDictionary.TryGetValue(unitType, out var prefab))
        {
            return prefab;
        }
        Debug.LogError($"Prefab not found for UnitType: {unitType}");
        return null;
    }

    public Unit SpawnUnit(UnitType unitType, Vector3 position)
    {
        Unit prefab = GetPrefab(unitType);
        if (prefab != null)
        {
            Unit unit = Instantiate(prefab, position, Quaternion.identity);
            AddUnit(unit);
            return unit;
        }
        Debug.LogError("No unit of that type");
        return null;
    }

    private void AddUnit(Unit unit)
    {
        if (unitLists.ContainsKey(unit.Type))
        {
            unitLists[unit.Type].Add(unit);
            UpdateUnitCount(unit.Type);
        }
        else
        {
            Debug.LogError($"No list found for UnitType: {unit.Type}");
        }
    }

    public void RemoveUnit(UnitType type, Unit unit)
    {
        if (unitLists.ContainsKey(type))
        {
            unitLists[type].Remove(unit);
            UpdateUnitCount(type);
        }
        else
        {
            Debug.LogError($"No list found for UnitType: {type}");
        }
    }

    private void UpdateUnitCount(UnitType type)
    {
        if (unitCountTexts.ContainsKey(type))
        {
            unitCountTexts[type].text = unitLists[type].Count.ToString();
        }
    }

    private void UpdateAllUnitCounts()
    {
        foreach (var type in unitLists.Keys)
        {
            UpdateUnitCount(type);
        }
    }
}


