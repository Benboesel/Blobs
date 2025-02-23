using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class TreeOfLife : Building, IDropArea
{
    [Serializable]
    public class UnitPrefabPair
    {
        public UnitType UnitType;
        public Material Material;
        public float GrowingTime;
    }

    [SerializeField] public List<UnitPrefabPair> unitPrefabs = new List<UnitPrefabPair>();
    public UnitSeed SeedPrefab;
    public UIElement UI;
    public float Height;
    public float Radius;
    public float UIOffset;
    public float IconHeight;

    public void Start()
    {
        UI.HideImmediate();
    }

    public void AddBlob(Transform blob)
    {
        ScoreManager.instance.IncreaseScore(1);
        Destroy(blob.gameObject);
    }

    public override void OnSelected()
    {
        base.OnSelected();
        Debug.Log("ON SELECTED");
        UI.SetPosition(transform.position, UIOffset);
        UI.Show();
    }

    public override void OnDeselected()
    {
        base.OnDeselected();
        Debug.Log("ON DESELECTED");
        UI.Hide();
    }
    public void StartGrowingSeed(UnitType type)
    {
        foreach (UnitPrefabPair pair in unitPrefabs)
        {
            if (pair.UnitType == type)
            {
                // Pick a random position on a circle
                Vector3 spawnPosition = GetRandomPositionAroundTree();

                // Instantiate the seed
                UnitSeed seed = Instantiate(SeedPrefab, spawnPosition, Quaternion.identity);
                seed.transform.SetParent(transform);
                seed.Type = type;
                seed.Initialize(pair.GrowingTime);
                seed.Mesh.material = pair.Material;
            }
        }
    }


    /// <summary>
    /// Returns a random position on a circle around the Tree of Life.
    /// </summary>
    private Vector3 GetRandomPositionAroundTree()
    {
        float radius = Mathf.Sqrt(UnityEngine.Random.Range(0f, Radius * Radius));

        // Get a random angle in radians
        float angle = UnityEngine.Random.Range(0f, Mathf.PI * 2);

        // Convert polar coordinates to cartesian (x, z)
        float x = Mathf.Cos(angle) * radius;
        float z = Mathf.Sin(angle) * radius;

        // Set spawn height relative to the tree
        return new Vector3(transform.position.x + x, transform.position.y + Height, transform.position.z + z);
    }

    public void Hover()
    {
        OnHovered();
    }

    public void Unhover()
    {
        OnUnhovered();
    }

    public void Accept(Pickupable item)
    {
        AddBlob(item.transform);
    }

    public float DropAreaHeight()
    {
        return IconHeight;
    }
}
