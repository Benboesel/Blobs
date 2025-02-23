using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target; // The character to follow
    public float followSpeed = 5f; // How quickly the camera follows the target
    public Vector3 offset = new Vector3(0f, 10f, -10f); // Default camera offset
    public float zoomSpeed = 10f; // Zoom speed
    public float minZoom = 5f; // Minimum zoom distance
    public float maxZoom = 20f; // Maximum zoom distance

    private Vector3 velocity = Vector3.zero;
    private float currentZoom;

    void Start()
    {
        currentZoom = offset.magnitude;
    }

    void LateUpdate()
    {
        if (!target) return;

        // Handle zooming
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        currentZoom -= scrollInput * zoomSpeed;
        currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);

        // Calculate desired position
        Vector3 desiredPosition = target.position + offset.normalized * currentZoom;
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, 1f / followSpeed);

        // Always look at the target
        // transform.LookAt(target.position);
    }
}
