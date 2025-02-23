using DG.Tweening;
using UnityEngine;

public class Candy : MonoBehaviour
{
    public float rotationSpeed = 50f;  // Speed of rotation
    public float colorChangeSpeed = 1f; // Speed of color transition
    public float scalePulseSpeed = 2f; // Speed of scale pulsing
    public float scaleAmount = 0.2f; // Scale change intensity

    private Vector3 randomRotation;
    private float hue;
    public MeshRenderer Mesh;
    public float Alpha;

    void Start()
    {
        // Generate a random rotation direction
        randomRotation = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
    }

    public void Show()
    {
        Mesh.material.DOFade(1f, 0.5f); // Animate alpha to 1 over 0.5 seconds
    }

    public void Hide()
    {
        Mesh.material.DOFade(0f, 0.5f); // Animate alpha to 0 over 0.5 seconds
    }
    void Update()
    {
        // Smoothly rotate in a random direction
        transform.Rotate(randomRotation * rotationSpeed * Time.deltaTime);

        // Cycle through colors using HSV color model
        hue += colorChangeSpeed * Time.deltaTime;
        if (hue > 1f) hue -= 1f; // Loop hue value

        Color color = Color.HSVToRGB(hue, 1f, 1f);
        color.a = Mesh.material.color.a;
        Mesh.material.color = color;

        // Make the cube pulse in size
        float scaleFactor = 1f + Mathf.Sin(Time.time * scalePulseSpeed) * scaleAmount;
        transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
    }
}
