using UnityEngine;

public class Pickupable : MonoBehaviour
{
    public bool IsClaimed = false;
    private Collider itemCollider;

    private void Awake()
    {
        itemCollider = GetComponent<Collider>();
    }

    /// <summary>
    /// Marks this item as claimed and disables its collider to prevent others from picking it.
    /// </summary>
    public bool Claim()
    {
        if (IsClaimed) return false; // Already claimed by another AI
        IsClaimed = true;
        if (itemCollider) itemCollider.enabled = false; // Prevent physics interactions
        return true;
    }

    /// <summary>
    /// Releases the claim and re-enables the collider.
    /// </summary>
    public void Release()
    {
        IsClaimed = false;
        if (itemCollider) itemCollider.enabled = true;
    }
}
