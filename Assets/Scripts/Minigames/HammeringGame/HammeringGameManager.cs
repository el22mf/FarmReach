using System;
using System.Collections.Generic;
using UnityEngine;

public class HammeringGameManager : MonoBehaviour, IMinigame
{
    [Header("Hammer Setup")]
    public Transform hammer;

    [Header("Guide Hammer")]
    public Transform guideHammer;
    public float guideInterval = 2f;
    public float guideMoveDuration = 0.4f;
    private bool guideAnimating = false;
    private float guideAnimTimer = 0f;

    private float guideStartAngle;
    private float guideTargetAngle;

    [Header("Adaptive Guide")]
    public float inactivityTime = 2f;
    public float movementThreshold = 5f; // degrees

    private float inactivityTimer = 0f;
    private float lastAngle;

    [Header("Nails")]
    public Transform[] nails;
    public float nailMaxDepth = 1f;
    public float visualDepth = 0.05f;

    [Header("Hammer Positioning")]
    public Vector3 hammerOffset = new Vector3(0f, 0.1f, 0.05f);
    public float followSpeed = 10f;

    [Header("Angle Settings")]
    public float loadAngle = 10f;
    public float hitThreshold = 25f;
    public float maxAngle = 90f;
    private float maxPullbackAngle = 0f;

    [Header("Hit Strength")]
    public float fullHitPower = 0.35f;

    [Header("Session")]
    public float sessionDuration = 60f;

    private float sessionTimer;
    private float[] nailProgress;
    private int currentNail = 0;
    private int completedNails = 0;

    private bool isLoaded = false;
    private float previousAngle;

    private Vector3[] nailStartPositions;
    public float nailDepthDistance = 0.1f;

    public int CompletedNails => completedNails;
    public float SessionTimeLeft => Mathf.Max(0f, sessionTimer);
    public bool IsSessionComplete => sessionTimer <= 0f;

    public Action<IMinigame> OnGameComplete { get; set; }

    public Dictionary<string, int> GetGameMetrics()
    {
        int finalScore = (completedNails * 5 * DifficultyManager.Instance.difficulty) + 100;

        return new Dictionary<string, int>
        {
            { "Nails Completed", completedNails },
            { "Final Score", finalScore }
        };
    }

    public string GetGameType() => "hammering";

    void Start()
    {
        guideHammer.gameObject.SetActive(false);

        nailStartPositions = new Vector3[nails.Length];

        for (int i = 0; i < nails.Length; i++)
        {
            nailStartPositions[i] = nails[i].localPosition;
        }

        sessionTimer = sessionDuration;
        nailProgress = new float[nails.Length];

        previousAngle = GetHammerAngle();
        lastAngle = previousAngle;

        SnapHammerToCurrentNail();
    }

    void LateUpdate()
    {
        if (sessionTimer <= 0f) return;

        sessionTimer -= Time.deltaTime;

        TrackHammer();
        UpdateHammerPosition();

        UpdateGuideHammer();
        AnimateGuide();

        if (sessionTimer <= 0f)
        {
            CompleteSession();
        }
    }

    void TrackHammer()
    {
        float currentAngle = GetHammerAngle();

        if (!isLoaded && currentAngle <= -loadAngle)
        {
            isLoaded = true;
            maxPullbackAngle = currentAngle;
        }

        if (isLoaded)
        {
            if (currentAngle < maxPullbackAngle)
                maxPullbackAngle = currentAngle;
        }

        if (isLoaded && previousAngle < hitThreshold && currentAngle >= hitThreshold)
        {
            float strength = Mathf.InverseLerp(0f, maxAngle, Mathf.Abs(maxPullbackAngle));

            ApplyHit(strength);

            isLoaded = false;
            maxPullbackAngle = 0f;
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

    float GetGuideTargetAngle(float currentAngle)
    {
        if (currentAngle > -loadAngle)
        {
            return -loadAngle - 15f;
        }

        if (currentAngle < hitThreshold)
        {
            return hitThreshold + 15f;
        }

        return currentAngle;
    }

    void UpdateGuideHammer()
    {
        float currentAngle = GetHammerAngle();

        float angleDelta = Mathf.Abs(currentAngle - lastAngle);

        // Check if meaningful movement happened
        if (angleDelta > movementThreshold)
        {
            inactivityTimer = 0f; // user is active, reset
        }
        else
        {
            inactivityTimer += Time.deltaTime;
        }

        lastAngle = currentAngle;

        // Only trigger guide if user is inactive AND not already animating
        if (!guideAnimating && inactivityTimer >= inactivityTime)
        {
            StartGuideMotion();
            inactivityTimer = 0f;
        }
    }

    void StartGuideMotion()
    {
        guideAnimating = true;
        guideAnimTimer = 0f;

        guideStartAngle = GetHammerAngle();
        guideTargetAngle = GetGuideTargetAngle(guideStartAngle);

        guideHammer.position = hammer.position;
        guideHammer.gameObject.SetActive(true);
    }

    void AnimateGuide()
    {
        if (!guideAnimating) return;

        guideAnimTimer += Time.deltaTime;

        float t = guideAnimTimer / guideMoveDuration;

        if (t >= 1f)
        {
            guideAnimating = false;
            guideHammer.gameObject.SetActive(false);
            return;
        }

        float smoothT = Mathf.SmoothStep(0f, 1f, t);

        float angle = Mathf.Lerp(guideStartAngle, guideTargetAngle, smoothT);

        guideHammer.position = hammer.position;
        guideHammer.rotation = Quaternion.Euler(angle, -90f, 0f);
    }

    void ResetNails()
    {
        nailProgress = new float[nails.Length];

        for (int i = 0; i < nails.Length; i++)
        {
            nails[i].localPosition = nailStartPositions[i];
        }

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