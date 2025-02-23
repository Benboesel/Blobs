using UnityEngine;
using DG.Tweening;

public class CharacterAnimator : MonoBehaviour
{
    [Header("Arm References")]
    public Transform LeftHand;
    public Transform RightHand;

    [Header("Pickup Animation Settings")]
    [SerializeField] private Vector3 leftArmPickupPosition = new Vector3(0.2f, 0.3f, 0f);
    [SerializeField] private Vector3 rightArmPickupPosition = new Vector3(-0.2f, 0.3f, 0f);
    [SerializeField] private float animationDuration = 0.5f;
    [SerializeField] private Ease animationEase = Ease.OutQuad;

    private Vector3 leftArmDefaultPos;
    private Vector3 rightArmDefaultPos;
    public PlayerPickupSystem PickupSystem;

    void Start()
    {
        // Store default positions for resetting later
        leftArmDefaultPos = LeftHand.localPosition;
        rightArmDefaultPos = RightHand.localPosition;
        PickupSystem.OnPickup += Pickup;
        PickupSystem.OnDrop += Drop;
    }

    public void Pickup()
    {
        // Move arms up and inward using tunable values
        LeftHand.DOLocalMove(leftArmPickupPosition, animationDuration).SetEase(animationEase);
        RightHand.DOLocalMove(rightArmPickupPosition, animationDuration).SetEase(animationEase);
    }

    public void Drop()
    {
        // Animate arms back to default position
        LeftHand.DOLocalMove(leftArmDefaultPos, animationDuration).SetEase(animationEase);
        RightHand.DOLocalMove(rightArmDefaultPos, animationDuration).SetEase(animationEase);
    }
}
