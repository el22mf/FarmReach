using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class HammeringGameManager : MonoBehaviour, IMinigame
{
    [Header("Hammer Setup")]
    public Transform hammer;

    [Header("Nails")]
    public Transform[] nails;                 // Assign in order (6 nails)
    public float nailMaxDepth = 1f;
    public float visualDepth = 0.05f;         // How far nails move down visually

    [Header("Hammer Positioning")]
    public Vector3 hammerOffset = new Vector3(0f, 0.1f, 0.05f);
    public float followSpeed = 10f;

    [Header("Angle Settings")]
    public float loadAngle = 80f;             // Wrist pulled back
    public float hitThreshold = 20f;          // Forward swing trigger
    public float maxAngle = 90f;
    private float maxPullbackAngle = 0f;

    [Header("Hit Strength")]
    public float fullHitPower = 0.35f;        // ~3 full hits per nail

    [Header("Session")]
    public float sessionDuration = 60f;

    // Internal state
    private float sessionTimer;
    private float[] nailProgress;
    private int currentNail = 0;
    private int completedNails = 0;

    private bool isLoaded = false;
    private float previousAngle;

    private Vector3[] nailStartPositions;
    public float nailDepthDistance = 0.1f; // x = 0.2 -> x = 0.1

    public int CompletedNails => completedNails;
    public float SessionTimeLeft => Mathf.Max(0f, sessionTimer);
    public bool IsSessionComplete => sessionTimer <= 0f;


    public Action<IMinigame> OnGameComplete { get; set; }

    // Metrics
    public Dictionary<string, float> GetGameMetrics()
    {
        return new Dictionary<string, float>
        {
            { "Nails Completed", completedNails }
        };
    }

    void Start()
    {

        nailStartPositions = new Vector3[nails.Length];

        for (int i = 0; i < nails.Length; i++)
        {
            nailStartPositions[i] = nails[i].localPosition;
        }

        sessionTimer = sessionDuration;
        nailProgress = new float[nails.Length];

        previousAngle = GetHammerAngle();

        SnapHammerToCurrentNail();
    }

    void LateUpdate()
    {
        if (sessionTimer <= 0f) return;

        sessionTimer -= Time.deltaTime;

        TrackHammer();
        UpdateHammerPosition();

        if (sessionTimer <= 0f)
        {
            CompleteSession();
        }
    }

    void TrackHammer()
    {
        float currentAngle = GetHammerAngle();

        // Detect load
        if (!isLoaded && currentAngle <= -loadAngle)
        {
            isLoaded = true;
            // Reset max pullback when starting a new load
            maxPullbackAngle = currentAngle;
            // Debug.Log("Hammer loaded");
        }

        // Track furthest back angle while loaded
        if (isLoaded)
        {
            // Most negative angle = furthest back
            if (currentAngle < maxPullbackAngle)
                maxPullbackAngle = currentAngle;
        }

        // Detect hit
        // Right to left swing: crossing hitThreshold
        if (isLoaded && previousAngle < hitThreshold && currentAngle >= hitThreshold)
        {
            // Use maxPullbackAngle for strength
            float strength = Mathf.InverseLerp(0f, maxAngle, Mathf.Abs(maxPullbackAngle));

            ApplyHit(strength);

            isLoaded = false;
            maxPullbackAngle = 0f;
            // Debug.Log("Hit applied");
        }

        previousAngle = currentAngle;
    }

    float GetHammerAngle()
    {
        float angle = hammer.localEulerAngles.x;

        if (angle > 180f) angle -= 360f;

        return Mathf.Clamp(angle, -maxAngle, maxAngle);
    }

    void ApplyHit(float strength)
    {
        float power = strength * fullHitPower;

        nailProgress[currentNail] += power;

        Debug.Log($"Hit nail {currentNail} | Strength: {strength:F2}");

        UpdateNailVisual();

        if (nailProgress[currentNail] >= nailMaxDepth)
        {
            CompleteNail();
        }
    }

    void UpdateNailVisual()
    {
        float progress = nailProgress[currentNail] / nailMaxDepth;

        Vector3 start = nailStartPositions[currentNail];
        Vector3 end = start + Vector3.left * nailDepthDistance;

        nails[currentNail].localPosition = Vector3.Lerp(start, end, progress);
    }

    void CompleteNail()
    {
        completedNails++;
        Debug.Log($"Nail {currentNail} complete!");

        currentNail++;

        if (currentNail >= nails.Length)
        {
            // Instead of ending session, reset all nails
            ResetNails();
            return;
        }

        SnapHammerToCurrentNail();
    }

    void UpdateHammerPosition()
    {
        if (currentNail >= nails.Length) return;

        Transform nail = nails[currentNail];

        float progress = nailProgress[currentNail] / nailMaxDepth;

        Vector3 dynamicOffset = hammerOffset;
        dynamicOffset.y -= progress * visualDepth;

        Vector3 targetPos = nail.position + dynamicOffset;

        hammer.position = Vector3.Lerp(
            hammer.position,
            targetPos,
            Time.deltaTime * followSpeed
        );
    }

    void SnapHammerToCurrentNail()
    {
        if (currentNail >= nails.Length) return;

        hammer.position = nails[currentNail].position + hammerOffset;
    }

    void ResetNails()
    {
        // Reset progress
        nailProgress = new float[nails.Length];

        // Reset nail visuals
        for (int i = 0; i < nails.Length; i++)
        {
            nails[i].localPosition = nailStartPositions[i];
        }

        // Reset hammer
        currentNail = 0;
        SnapHammerToCurrentNail();

        isLoaded = false;
        previousAngle = GetHammerAngle();
        maxPullbackAngle = 0f;

        Debug.Log("All nails reset! Start again.");
    }

    void CompleteSession()
    {
        OnGameComplete?.Invoke(this);
        Debug.Log($"Session Complete! Nails done: {completedNails}");
    }

    public void ResetGame()
    {
        sessionTimer = sessionDuration;
        completedNails = 0;
        currentNail = 0;

        nailProgress = new float[nails.Length];

        isLoaded = false;
        previousAngle = GetHammerAngle();

        // Reset nail visuals
        foreach (Transform nail in nails)
        {
            Vector3 pos = nail.localPosition;
            pos.x = 0.2f;
            nail.localPosition = pos;
        }

        SnapHammerToCurrentNail();
    }

    public bool IsActive()
    {
        return gameObject.activeInHierarchy && sessionTimer > 0f;
    }
}