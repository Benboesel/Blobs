using DG.Tweening;
using UnityEngine;

public class Pickupable : MonoBehaviour
{
    public bool IsClaimed = false;
    private Collider itemCollider;
    public bool IsPickedUp;
    public float DropIconHeight;
    private void Awake()
    {
        itemCollider = GetComponent<Collider>();
    }

    /// <summary>
    /// Marks this item as claimed and disables its collider to prevent others from picking it.
    /// </summary>
    public virtual bool Claim()
    {
        if (IsClaimed) return false; // Already claimed by another AI
        IsClaimed = true;
        if (itemCollider) itemCollider.enabled = false; // Prevent physics interactions
        return true;
    }

    /// <summary>
    /// Releases the claim and re-enables the collider.
    /// </summary>
    public virtual void Release()
    {
        IsClaimed = false;
        if (itemCollider) itemCollider.enabled = true;
    }

    public void DropOnGround(Vector3 finalPosition)
    {
        UnHover();
        transform.DOJump(finalPosition, 3.0f, 1, 0.5f).SetEase(Ease.InQuad).OnComplete(() => DropHitGround());
    }

    protected void DropHitGround()
    {
        IsPickedUp = false;
        Release();
    }

    public virtual void Pickup()
    {
        IsPickedUp = true;
        Claim();
    }

    public virtual void Hover()
    {

    }

    public virtual void UnHover()
    {

    }
}
