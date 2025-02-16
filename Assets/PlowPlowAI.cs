using UnityEngine;
using System.Collections.Generic;

public class PlowPlowAI : UnitAI
{
    public float detectionRadius = 5f;
    public LayerMask PickupLayer; // Detects both Slimes and Seeds
    private IMoveBehavior moveBehavior;
    public Pickupable ItemInHand;
    public float PickupDistance = 1.5f;
    public Pickupable currentClaim;

    private float searchCooldown = 0.5f; // How often to check for a closer item
    private float nextSearchTime = 0f;   // Time tracker for next search

    public void Awake()
    {
        moveBehavior = GetComponent<IMoveBehavior>();
    }
    public void SearchForItem()
    {
        Pickupable betterItem = FindNearestPickupable(); // Find the closest available item first
        if (betterItem != null)
        {
            // If we don't have a current claim, or the new item is a seed and we were previously going for a non-seed, switch
            bool shouldSwitch = currentClaim == null ||
                                (betterItem is UnitSeed && !(currentClaim is UnitSeed));

            if (shouldSwitch)
            {
                if (currentClaim != null)
                {
                    currentClaim.Release();
                }
                currentClaim = betterItem;
                currentClaim.Claim();
            }
        }
        if (currentClaim != null)
        {
            moveBehavior.Move(currentClaim.transform.position);

            if (Vector3.Distance(transform.position, currentClaim.transform.position) < PickupDistance)
            {
                PickUpItem(currentClaim);
            }
        }
        else
        {
            PickNewWanderTarget();
        }
    }

    Pickupable FindNearestPickupable()
    {
        Collider[] items = Physics.OverlapSphere(transform.position, detectionRadius, PickupLayer);
        Pickupable closestSeed = null;
        Pickupable closestOther = null;
        float nearestSeedDistance = Mathf.Infinity;
        float nearestOtherDistance = Mathf.Infinity;

        foreach (var col in items)
        {
            if (col.TryGetComponent(out Pickupable pickupable) && !pickupable.IsClaimed)
            {
                float distance = Vector3.Distance(transform.position, col.transform.position);

                // If we already have a claim, only switch if the new item is closer than the current claim
                if (currentClaim != null && distance >= Vector3.Distance(transform.position, currentClaim.transform.position))
                {
                    continue; // Skip this item since it's further than our current claim
                }

                if (pickupable.GetComponent<UnitSeed>() != null) // Prioritize seeds
                {
                    if (distance < nearestSeedDistance)
                    {
                        closestSeed = pickupable;
                        nearestSeedDistance = distance;
                    }
                }
                else // Other pickupables (like Slime)
                {
                    if (distance < nearestOtherDistance)
                    {
                        closestOther = pickupable;
                        nearestOtherDistance = distance;
                    }
                }
            }
        }
        // **ENFORCE SEED PRIORITY: Always return closest seed first, even if farther**
        if (closestSeed != null)
        {
            return closestSeed; // Always prioritize a seed if one exists
        }

        return closestOther; // Only return non-seed if no seeds exist
    }


    void PickNewWanderTarget()
    {
        Vector3 randomDirection = new Vector3(
            Random.Range(-GameManager.instance.PlayAreaSize, GameManager.instance.PlayAreaSize),
            0,
            Random.Range(-GameManager.instance.PlayAreaSize, GameManager.instance.PlayAreaSize)
        );
        moveBehavior.Move(randomDirection);
    }

    public void Update()
    {
        if (!ItemInHand)
        {
            SearchForItem();
        }
        else
        {
            if (ItemInHand.TryGetComponent(out Pickupable pickupable))
            {
                if (ItemInHand.gameObject.layer == LayerMask.NameToLayer("Slime"))
                {
                    MoveToDepot();
                }
                else if (ItemInHand.gameObject.layer == LayerMask.NameToLayer("Seed"))
                {
                    MoveToNearestChumbo();
                }
            }
        }
    }

    public void PickUpItem(Pickupable item)
    {
        if (item.TryGetComponent(out Pickupable pickupable))
        {
            Destroy(item.GetComponent<Collider>());
            item.transform.SetParent(this.transform);
            item.transform.localPosition = new Vector3(0, 1.0f, 0);
            ItemInHand = item;
            currentClaim = null; // Stop tracking claims since we're holding an item
        }
    }

    void MoveToDepot()
    {
        moveBehavior.Move(GameManager.instance.Depot.transform.position);
        if (Vector3.Distance(transform.position, GameManager.instance.Depot.transform.position) < 2f)
        {
            GameManager.instance.Depot.AddBlob(ItemInHand.transform);
            ReleaseItem();
        }
    }

    void MoveToNearestChumbo()
    {
        Transform nearestChumbo = FindNearestChumbo();
        if (nearestChumbo == null) return;

        moveBehavior.Move(nearestChumbo.transform.position);
        if (Vector3.Distance(transform.position, nearestChumbo.transform.position) < 2f)
        {
            ChumboAI chumbo = nearestChumbo.GetComponent<ChumboAI>();
            if (ItemInHand.TryGetComponent(out UnitSeed seed) && chumbo != null)
            {
                chumbo.AddSeed(seed);
                ReleaseItem();
            }
        }
    }

    /// <summary>
    /// Releases the item when dropped off.
    /// </summary>
    void ReleaseItem()
    {
        if (ItemInHand && ItemInHand.TryGetComponent(out Pickupable pickupable))
        {
            pickupable.Release(); // Make item available again
            ItemInHand = null;
        }
    }

    Transform FindNearestChumbo()
    {
        List<Unit> allChumbos = UnitManager.instance.GetUnitsByType(UnitType.Chumbo);
        Transform nearestChumbo = null;
        float shortestDistance = Mathf.Infinity;

        foreach (Unit chumbo in allChumbos)
        {
            float distance = Vector3.Distance(transform.position, chumbo.transform.position);
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                nearestChumbo = chumbo.transform;
            }
        }
        return nearestChumbo;
    }

    private void OnDestroy()
    {
        // If destroyed while carrying an item, release claim
        if (currentClaim != null)
        {
            currentClaim.Release();
        }
        if (ItemInHand != null)
        {
            ReleaseItem();
        }
    }
}
