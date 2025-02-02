using UnityEngine;
using UnityEngine.AI;

public class Harvester : MonoBehaviour
{
    // find nearest blob 
    // pick them up 
    // bring them back to blob depot
    public float detectionRadius = 5f;
    public LayerMask BlobLayer;
    private NavMeshAgent agent;
    public float speed = 3f;
    public Transform BlobInHand;
    public BlobDepot Depot;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = speed;
    }

    public void SearchForBlob()
    {
        Collider[] objectsInRange = Physics.OverlapSphere(transform.position, detectionRadius, BlobLayer);
        Slime nearestBlob = null;
        float nearestBlobDistance = Mathf.Infinity;
        foreach (var col in objectsInRange)
        {
            Slime blob = col.GetComponent<Slime>();
            if (blob != null)
            {
                float distance = Vector3.Distance(transform.position, blob.transform.position);
                if (distance < nearestBlobDistance)
                {
                    nearestBlob = blob;
                    nearestBlobDistance = distance;
                }
            }
        }
        if (nearestBlob != null)
        {
            agent.SetDestination(nearestBlob.transform.position);
        }
        else
        {
            PickNewWanderTarget();
        }
    }

    void PickNewWanderTarget()
    {
        Vector3 randomDirection = new Vector3(
            Random.Range(-GameManager.instance.PlayAreaSize, GameManager.instance.PlayAreaSize),
            0,
            Random.Range(-GameManager.instance.PlayAreaSize, GameManager.instance.PlayAreaSize)
        );
        agent.SetDestination(randomDirection);
    }
    public void Update()
    {
        if (!BlobInHand)
        {
            SearchForBlob();
        }
        else
        {
            agent.SetDestination(Depot.transform.position);
            if (Vector3.Distance(transform.position, Depot.transform.position) < 5f)
            {
                Depot.AddBlob(BlobInHand);
                BlobInHand = null;
            }
        }
    }

    public void PickUpBlob(Slime blob)
    {
        Transform blobObject = blob.transform;
        Destroy(blob);
        Destroy(blob.GetComponent<SphereCollider>());
        blobObject.transform.SetParent(this.transform);
        blobObject.localPosition = new Vector3(0, 1.0f, 0);
        BlobInHand = blobObject;
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (BlobInHand) return;
        Slime blob = collision.GetComponent<Slime>();
        if (blob != null)
        {
            PickUpBlob(blob);
        }
    }
}
