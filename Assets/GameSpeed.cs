using System.Collections;
using System.Collections.Generic;
// using DG.Tweening;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI; // Import UI namespace

public class GameSpeed : MonoBehaviour
{
    [Range(0.01f, 100f)]
    public float Speed;

    // private Tweener speedTween; // Store the current speed tween
    // public Slider speedSlider; // Reference to the UI Slider
    // public bool IsPaused = false;

    public void Update()
    {
        Time.timeScale = Speed;
    }

    public void Start()
    {
        // Time.timeScale = DefaultSpeed;

        // // Ensure the slider reflects the initial speed and set up the listener
        // if (speedSlider != null)
        // {
        //     speedSlider.value = DefaultSpeed;
        //     speedSlider.onValueChanged.AddListener(OnSliderValueChanged);
        // }
    }

    private void OnDestroy()
    {
        // Clean up listener
        // if (speedSlider != null)
        // {
        //     speedSlider.onValueChanged.RemoveListener(OnSliderValueChanged);
        // }
    }

    private void OnSliderValueChanged(float value)
    {
        // Update game speed directly or with animation
        // DefaultSpeed = value;
        // UpdateGameSpeed(value);
        // Optional: Smooth animation instead of instant update
        // AnimateSpeed(value, 0.5f, Ease.Linear);
    }

    // public void AnimateSpeed(float targetSpeed, float animationTime, Ease ease)
    // {
    //     // Kill any existing speed animation
    //     if (speedTween != null && speedTween.IsActive())
    //     {
    //         speedTween.Kill();
    //     }

    //     // Animate Time.timeScale to the target speed over time
    //     speedTween = DOTween.To(UpdateGameSpeed, Time.timeScale, targetSpeed, animationTime).SetEase(ease).SetUpdate(true);
    // }

    // public void Pause()
    // {
    //     IsPaused = true;
    //     AnimateSpeed(0f, 0.2f, Ease.OutQuad);
    //     speedSlider.interactable = false;
    // }

    // public void UnPause()
    // {
    //     IsPaused = false;
    //     AnimateSpeed(DefaultSpeed, 0.2f, Ease.OutQuad);
    //     speedSlider.interactable = true;
    // }

    public void UpdateGameSpeed(float speed)
    {
        Time.timeScale = speed;
    }

    // public void AnimateSpeedToDefault(float animationTime, Ease ease)
    // {
    //     AnimateSpeed(DefaultSpeed, animationTime, ease);
    // }
}
