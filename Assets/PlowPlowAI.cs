using UnityEngine;
using UnityEngine.AI;

public class PlowPlowAI : UnitAI
{
    public float detectionRadius = 5f;
    public LayerMask BlobLayer;
    private IMoveBehavior moveBehavior;
    public Transform BlobInHand;
    public float PickupDistance;

    public void Start()
    {
        moveBehavior = GetComponent<IMoveBehavior>();
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
            moveBehavior.Move(nearestBlob.transform.position);
            if (Vector3.Distance(transform.position, nearestBlob.transform.position) < PickupDistance)
            {
                PickUpBlob(nearestBlob);

            }
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
        moveBehavior.Move(randomDirection);
    }
    
    public void Update()
    {
        if (!BlobInHand)
        {
            SearchForBlob();
        }
        else
        {
            moveBehavior.Move(GameManager.instance.Depot.transform.position);
            if (Vector3.Distance(transform.position, GameManager.instance.Depot.transform.position) < 5f)
            {
                GameManager.instance.Depot.AddBlob(BlobInHand);
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
}
