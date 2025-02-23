using UnityEngine;
using System.Collections.Generic;
using System;

public class PlayerPickupSystem : MonoBehaviour
{
    public LayerMask pickupLayerMask;
    public LayerMask dropAreaLayerMask;
    public float pickupRange = 5f;
    public Transform holdPoint; // Set this above player's head
    public float pickupSpeed = 5f;
    public KeyCode pickupKey = KeyCode.E;
    private Pickupable hoveredPickupable;
    private Pickupable heldPickupable;
    private IDropArea hoveredDropArea;
    public float DropDistance;
    public Transform InteractibleIcon;
    public Action OnPickup;
    public Action OnDrop;
    void Update()
    {
        if (heldPickupable == null)
        {
            HandlePickupHover();
            if (Input.GetKeyDown(pickupKey) && hoveredPickupable != null)
            {
                PickupObject(hoveredPickupable);
            }
        }
        else
        {
            FollowPlayer();
            HandleDropAreaHover();
            if (Input.GetKeyDown(pickupKey))
            {
                DropObject();
            }
        }
    }

    void HandlePickupHover()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, pickupRange, pickupLayerMask);
        Pickupable closest = null;
        float closestDist = float.MaxValue;

        foreach (Collider col in colliders)
        {
            Pickupable pickupable = col.GetComponent<Pickupable>();
            if (pickupable != null)
            {
                float dist = Vector3.Distance(transform.position, col.transform.position);
                if (dist < closestDist)
                {
                    closest = pickupable;
                    closestDist = dist;
                }
            }
        }

        if (hoveredPickupable != closest)
        {
            if (hoveredPickupable != null)
            {
                hoveredPickupable.UnHover();
                HideInteractibleIcon();
            }

            hoveredPickupable = closest;
            if (hoveredPickupable != null)
            {
                hoveredPickupable.Hover();
                ShowInteractibleIcon(hoveredPickupable.transform, hoveredPickupable.DropIconHeight);
            }
        }
    }

    public void HideInteractibleIcon()
    {
        InteractibleIcon.gameObject.SetActive(false);
    }

    public void ShowInteractibleIcon(Transform anchor, float offset)
    {
        InteractibleIcon.gameObject.SetActive(true);
        Vector3 position = anchor.position;
        position.y += offset;
        InteractibleIcon.transform.position = position;
    }

    void PickupObject(Pickupable pickupable)
    {
        heldPickupable = pickupable;
        heldPickupable.Pickup();
        HideInteractibleIcon();
        OnPickup?.Invoke();
    }

    void FollowPlayer()
    {
        if (heldPickupable != null)
        {
            heldPickupable.transform.position = Vector3.Lerp(heldPickupable.transform.position, holdPoint.position, Time.deltaTime * pickupSpeed);
        }
    }

    void HandleDropAreaHover()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, pickupRange, dropAreaLayerMask);
        IDropArea closestDrop = null;
        float closestDist = float.MaxValue;

        foreach (Collider col in colliders)
        {
            IDropArea dropArea = col.GetComponent<IDropArea>();
            if (dropArea != null)
            {
                float dist = Vector3.Distance(transform.position, col.transform.position);
                if (dist < closestDist)
                {
                    closestDrop = dropArea;
                    closestDist = dist;
                }
            }
        }

        if (hoveredDropArea != closestDrop)
        {
            if (hoveredDropArea != null)
            {
                hoveredDropArea.Unhover();
                HideInteractibleIcon();
            }

            hoveredDropArea = closestDrop;
            if (hoveredDropArea != null)
            {
                hoveredDropArea.Hover();
                ShowInteractibleIcon(hoveredDropArea.GetTransform(), hoveredDropArea.DropAreaHeight());
            }
        }
    }

    void DropObject()
    {
        if (heldPickupable != null)
        {
            if (hoveredDropArea != null)
            {
                hoveredDropArea.Accept(heldPickupable);
                hoveredDropArea.Unhover();
                HideInteractibleIcon();
            }
            else
            {
                Vector3 floorPosition = transform.position + (transform.forward * DropDistance);
                floorPosition.y = 0;
                heldPickupable.DropOnGround(floorPosition);
            }
            heldPickupable = null;
            OnDrop?.Invoke();
        }
    }
}
