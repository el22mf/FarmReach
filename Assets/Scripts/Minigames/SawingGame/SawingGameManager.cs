using System;
using System.Collections.Generic;
using System.Drawing.Text;
using UnityEngine;
using UnityEngine.UIElements;

public class SawingGameManager : MonoBehaviour, IMinigame
{
    [Header("Saw Setup")]
    public Transform saw;

    [Header("Guide Saw")]
    public Transform guideSaw;
    public float guideMoveDuration = 0.4f;

    private bool guideAnimating = false;
    private float guideAnimTimer = 0f;

    private Vector3 guideStartPos;
    private Vector3 guideTargetPos;

    [Header("Adaptive Guidance")]
    public float inactivityTime = 2f;
    public float movementThreshold = 0.002f;

    public float guideZDistance = 0.15f;

    private float inactivityTimer = 0f;

    [Header("Saw Movement Settings")]
    public float startLocalY = 0.8f;
    public float logBottomY = 0.2f;
    public float downwardRate = 0.1f;

    [Header("Session")]
    public float sessionDuration = 60f;

    private float sessionTimer;
    private int completedCuts = 0;
    private float currentCutTimer = 0f;

    // movement tracking
    private float lastZ;
    private float lastVelocityZ;
    private int lastDirection = 1;

    // rhythm tracking
    private int directionChanges = 0;
    private float rhythmTimer = 0f;

    private bool guideCooldownLock = false;
    public float guideCooldownTime = 1.0f;
    private float guideCooldownTimer = 0f;

    public int CompletedCuts => completedCuts;
    public float SessionTimeLeft => Mathf.Max(0f, sessionTimer);
    public bool IsSessionComplete => sessionTimer <= 0f;

    public Action<IMinigame> OnGameComplete { get; set; }

    public Dictionary<string, int> GetGameMetrics()
    {
        int finalScore = (completedCuts * 5 * DifficultyManager.Instance.difficulty);

        return new Dictionary<string, int>
        {
            { "Cuts Completed", completedCuts },
            { "Final Score", finalScore }
        };
    }

    public string GetGameType() => "sawing";

    public AudioSource timerSound;
    bool timerStarted = false;

    void Start()
    {
        guideSaw.gameObject.SetActive(false);

        sessionTimer = sessionDuration;
        lastZ = saw.localPosition.z;

        StartNextCut();
    }

    void LateUpdate()
    {
        if (sessionTimer <= 0f) return;

        sessionTimer -= Time.deltaTime;

        if (guideCooldownTimer > 0f)
        {
            guideCooldownTimer -= Time.deltaTime;
            if (guideCooldownTimer <= 0f)
                guideCooldownLock = false;
        }

        TrackSawMovement();
        UpdateGuideSaw();
        AnimateGuideSaw();

        currentCutTimer += Time.deltaTime;

        if (sessionTimer <= 10f && !timerStarted)
        {
            timerSound.Play();
            timerStarted = true;
        }

        if (sessionTimer <= 0f)
        {
            CompleteSession();
            timerStarted = false;
            timerSound.Stop();
        }
    }

    void TrackSawMovement()
    {
        float currentZ = saw.localPosition.z;

        float deltaZ = currentZ - lastZ;
        float zMovement = Mathf.Abs(deltaZ);

        float velocityZ = deltaZ / Time.deltaTime;

        int direction = (deltaZ >= 0f) ? 1 : -1;

        // =========================
        // RHYTHM TRACKING (NEW ADDITION)
        // =========================
        if (direction != lastDirection && zMovement > movementThreshold)
        {
            directionChanges++;
            rhythmTimer = 0f;
        }

        lastDirection = direction;
        lastVelocityZ = velocityZ;
        lastZ = currentZ;

        // =========================
        // ORIGINAL CORE MECHANIC (UNCHANGED INTENT)
        // =========================
        if (zMovement > 0.005f)
        {
            saw.localPosition += Vector3.down * downwardRate * Time.deltaTime;
        }

        // inactivity tracking
        if (zMovement > movementThreshold)
            inactivityTimer = 0f;
        else
            inactivityTimer += Time.deltaTime;

        rhythmTimer += Time.deltaTime;

        // cut completion
        if (saw.localPosition.y <= logBottomY)
        {
            CompleteCurrentCut();
        }
    }

    void UpdateGuideSaw()
    {
        float currentZ = saw.localPosition.z;
        float deltaZ = currentZ - lastZ;

        bool isTooSlow = Mathf.Abs(lastVelocityZ) < 0.05f;
        bool isStuck = directionChanges < 2 && rhythmTimer > 1.5f;

        rhythmTimer += Time.deltaTime;

        if (!guideAnimating &&
            !guideCooldownLock &&
            (inactivityTimer >= inactivityTime || isTooSlow || isStuck))
        {
            StartGuideMotion();
            inactivityTimer = 0f;
        }
    }

    void StartGuideMotion()
    {
        guideAnimating = true;
        guideAnimTimer = 0f;
        guideCooldownLock = true;

        guideStartPos = saw.localPosition;

        Vector3 forward = saw.forward.normalized;

        guideTargetPos = guideStartPos + forward * guideZDistance;

        guideSaw.localPosition = saw.localPosition;
        guideSaw.gameObject.SetActive(true);
    }

    void AnimateGuideSaw()
    {
        if (!guideAnimating) return;

        guideAnimTimer += Time.deltaTime;

        float t = guideAnimTimer / guideMoveDuration;

        if (t >= 1f)
        {
            guideAnimating = false;
            guideSaw.gameObject.SetActive(false);
            directionChanges = 0;

            guideCooldownTimer = guideCooldownTime;
            return;
        }

        float smoothT = Mathf.SmoothStep(0f, 1f, t);

        guideSaw.localPosition = Vector3.Lerp(
            guideStartPos,
            guideTargetPos,
            smoothT
        );
    }

    void CompleteCurrentCut()
    {
        completedCuts++;
        currentCutTimer = 0f;

        StartNextCut();
    }

    void StartNextCut()
    {
        Vector3 pos = saw.localPosition;
        pos.y = startLocalY;
        saw.localPosition = pos;

        // reset per-cut learning
        directionChanges = 0;
        rhythmTimer = 0f;
    }

    void CompleteSession()
    {
        OnGameComplete?.Invoke(this);
        Debug.Log($"Session Complete! Cuts: {completedCuts}");
    }

    public void ResetGame()
    {
        sessionTimer = sessionDuration;
        completedCuts = 0;
        currentCutTimer = 0f;

        Vector3 pos = saw.localPosition;
        pos.y = startLocalY;
        saw.localPosition = pos;

        lastZ = saw.localPosition.z;
        directionChanges = 0;
        rhythmTimer = 0f;

        StartNextCut();
    }
    public bool IsActive()
    {
        return gameObject.activeInHierarchy && sessionTimer > 0f;
    }
}