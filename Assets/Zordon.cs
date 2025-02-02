using System.Collections;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Timeline;

public class Zordon : Unit
{
    public LayerMask AttackLayer;
    public float DetectionRadius;
    public float AttackRange;
    public Unit Target;
    private bool isLeaching = false;
    public float Damage;
    public float SizePerDamage;

    public void Update()
    {
        if (isLeaching) return;
        LookForTarget();

        if (Target == null)
        {
            Agent.SetDestination(Vector3.zero);
        }
        if (Target != null)
        {
            Agent.SetDestination(Target.transform.position);
            if (Vector3.Distance(transform.position, Target.transform.position) < AttackRange)
            {
                StartCoroutine(Leach());
            }
        }
    }

    void LookForTarget()
    {
        Collider[] objectsInRange = Physics.OverlapSphere(transform.position, DetectionRadius, AttackLayer);
        float shortestDistance = Mathf.Infinity;
        Unit nearestUnit = null;

        foreach (var col in objectsInRange)
        {
            Unit blob = col.GetComponent<Unit>();
            if (blob == null)
                continue;

            float distance = Vector3.Distance(transform.position, blob.transform.position);
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                nearestUnit = blob;
            }
        }

        Target = nearestUnit;
    }

    public IEnumerator Leach()
    {
        isLeaching = true;
        Agent.enabled = false;
        Transform anchor = new GameObject().transform;
        anchor.position = Target.GetTop();
        transform.SetParent(anchor.transform);
        transform.DOLocalJump(new Vector3(0, 0, 0), 1f, 1, 0.5f);
        while (true)
        {
            anchor.position = Target.GetTop() + (transform.localScale.x * Vector3.up * 0.5f);
            bool died = Target.TakeDamage(Damage * Time.deltaTime);
            float size = transform.localScale.x;
            size += (Damage * SizePerDamage) * Time.deltaTime;
            transform.localScale = size * Vector3.one;
            if (died)
            {
                transform.SetParent(null);
                Destroy(Target.gameObject);
                Target = null;
                Agent.enabled = true;
                isLeaching = false;
                yield break;
            }
            yield return new WaitForEndOfFrame();
        }
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, DetectionRadius);
    }
}
