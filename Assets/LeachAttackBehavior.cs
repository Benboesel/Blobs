using System.Collections;
using UnityEngine;
using DG.Tweening; // Ensure you have DOTween imported

public class LeachAttackBehavior : MonoBehaviour, IAttackBehavior
{
    [Header("Leach Settings")]
    public float Damage = 10f;
    // public float AttackRange = 2f; // How close we need to be to begin the leach
    public float SizePerDamage = 0.1f;

    [Header("Jump Animation Settings")]
    public float JumpHeight = 1f;
    public float JumpDuration = 0.5f;
    public int JumpNum = 1;

    private bool isLeaching = false;
    private Unit currentTarget;

    // If using a NavMeshAgent for movement, cache a reference
    private IMoveBehavior moveBehavior;
    private BasicDamageable damageable;
    private Transform anchor;
    void Awake()
    {
        moveBehavior = GetComponent<IMoveBehavior>();
        damageable = GetComponent<BasicDamageable>();
    }

    /// <summary>
    /// Initiates the leach attack if the target is within range and we’re not already attacking.
    /// </summary>
    public void Attack(Unit target)
    {
        // Prevent multiple leach attempts
        if (isLeaching)
            return;

        // Check if the target is close enough to attack
        // if (Vector3.Distance(transform.position, target.transform.position) > AttackRange)
        //     return;

        currentTarget = target;
        JumpToUnit();

    }

    public void JumpToUnit()
    {
        isLeaching = true;
        // agent.isStopped = true;
        // Stop movement while leaching
        moveBehavior.Disable();

        // Create an anchor for positioning relative to the target.
        // (Assumes the target has a method GetTop() that returns a Vector3 position above it.)
        anchor = new GameObject("LeachAnchor").transform;
        anchor.position = currentTarget.GetTop();

        // Parent this enemy to the anchor so that it "sticks" to the target.
        transform.SetParent(anchor);

        // Animate the jump onto the target using DOTween.
        Sequence sequence = transform.DOLocalJump(Vector3.zero, JumpHeight, JumpNum, JumpDuration);
        sequence.OnComplete(() => StartCoroutine(Leach()));
        sequence.OnUpdate(() => anchor.position = currentTarget.GetTop());
    }

    /// <summary>
    /// Performs the leach attack over time.
    /// </summary>
    private IEnumerator Leach()
    {
        // Debug.Log("Start LEACH");
        while (true)
        {
            // If for any reason the target becomes null, exit.
            if (currentTarget == null)
                break;
            // currentTarget.IsLatched = true;
            // Update the anchor’s position so our unit stays on the target’s back.
            anchor.position = currentTarget.GetTop();

            // Apply damage over time.
            // (Assumes that currentTarget.TakeDamage returns true if the target dies.)
            bool targetDied = currentTarget.TakeDamage(Damage * Time.deltaTime);

            // Grow in size as damage is dealt.
            damageable.Heal(Damage * SizePerDamage * Time.deltaTime);
            // Debug.Log("LEACHING");

            if (targetDied)
            {
                // If the target dies, unparent and clean up.
                transform.SetParent(null, true);
                currentTarget = null;
                break;
            }

            yield return new WaitForEndOfFrame();
        }
        // currentTarget.IsLatched = false;
        // Debug.Log("STOP LEACH");
        moveBehavior.TeleportTo(transform.position);
        moveBehavior.Enable();
        // Clean up the anchor.
        Destroy(anchor.gameObject);

        isLeaching = false;
    }
}
