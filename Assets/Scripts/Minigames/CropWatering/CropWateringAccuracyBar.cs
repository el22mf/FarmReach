using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CropWateringAccuracyBar : MonoBehaviour
{
    public RectTransform indicator;
    public RectTransform barBG;

    public float minAngle = 30f;  // angle at left edge
    public float maxAngle = 120f; // angle at right edge

    public void UpdateIndicator(float currentAngle)
    {
        float normalized = Mathf.InverseLerp(minAngle, maxAngle, currentAngle);
        normalized = Mathf.Clamp01(normalized);

        float x = Mathf.Lerp(-barBG.rect.width / 2f, barBG.rect.width / 2f, normalized);
        indicator.anchoredPosition = new Vector2(x, indicator.anchoredPosition.y);
    }
}

