using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplePickingAccuracyBar : MonoBehaviour
{
    public RectTransform indicator;
    public RectTransform barBG;

    public float minAngle = 0f;  // angle at left edge
    public float maxAngle = 90f; // angle at right edge

    public void UpdateIndicator(float currentAngle)
    {
        float normalized = Mathf.InverseLerp(minAngle, maxAngle, currentAngle);
        normalized = Mathf.Clamp01(normalized);

        float x = Mathf.Lerp(-barBG.rect.width / 2f, barBG.rect.width / 2f, normalized);
        indicator.anchoredPosition = new Vector2(x, indicator.anchoredPosition.y);
    }
}

