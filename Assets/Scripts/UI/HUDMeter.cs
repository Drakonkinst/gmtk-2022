using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDMeter : MonoBehaviour
{
    public RectTransform fill;
    public float initialValue = 1.0f;

    private RectTransform meter;
    public float acceleration = 0.1f;
    private float currentPercentage;
    private float targetPercentage;
    private float velocity = 0.0f;

    void Awake()
    {
        meter = GetComponent<RectTransform>();
        SetPercentageInstant(initialValue);
    }
    
    void FixedUpdate() {
        if (currentPercentage == targetPercentage)
        {
            // No need to do anything
            velocity = 0.0f;
            return;
        }

        if (currentPercentage < targetPercentage)
        {
            velocity += acceleration * Time.deltaTime;
            currentPercentage = Mathf.Min(currentPercentage + velocity, targetPercentage);
        }
        else if (currentPercentage > targetPercentage)
        {
            velocity += acceleration * Time.deltaTime;
            currentPercentage = Mathf.Max(currentPercentage - velocity, targetPercentage);
        }
        UpdateDisplayPercentage();
    }

    public void SetPercentage(float percentage)
    {
        targetPercentage = Mathf.Clamp01(percentage);
    }

    public void SetPercentageInstant(float percentage)
    {
        targetPercentage = Mathf.Clamp01(percentage);
        currentPercentage = targetPercentage;
        UpdateDisplayPercentage();
    }
    
    private void UpdateDisplayPercentage() {
        float maxLength = meter.sizeDelta.x;
        float length = currentPercentage * maxLength;
        fill.sizeDelta = new Vector2(length, fill.sizeDelta.y);
    }

    public float GetWidth()
    {
        return meter.sizeDelta.x;
    }

    public void SetWidth(float width)
    {
        meter.sizeDelta = new Vector2(width, meter.sizeDelta.y);
    }
}
