using System;
using System.Collections.Generic;
using UnityEngine;

public class SawingGameManager : MonoBehaviour, IMinigame
{
    [Header("Saw Setup")]
    public Transform saw;                 // Saw object
    public float startLocalY = 0.8f;        // Starting from Y height (local to log)
    public float logBottomY = 0.2f;      // Local Y bottom limit
    public float[] logXPositions = new float[] { -0.9f, -0.3f, 0.3f, 0.9f };

    [Header("Saw Movement Settings")]
    public float downwardRate = 0.1f;     // Local Y movement per second

    [Header("Session Settings")]
    public float sessionDuration = 60f;   // Total game time in seconds

    [Header("Tracking")]
    private float sessionTimer;
    private int completedCuts = 0;
    private float currentCutTimer = 0f;
    private float lastZ;
    private int currentXIndex = 0;        // Tracks which X position to use next


    public int CompletedCuts => completedCuts;
    public float CurrentCutTime => currentCutTimer;
    public float SessionTimeLeft => Mathf.Max(0f, sessionTimer);
    public bool IsSessionComplete => sessionTimer <= 0f;

    public Action<IMinigame> OnGameComplete { get; set; }

    public Dictionary<string, float> GetGameMetrics()
    {
        return new Dictionary<string, float>
    {
        { "Completed Cuts", completedCuts }
    };
    }

    void Start()
    {
        lastZ = saw.localPosition.z;
        sessionTimer = sessionDuration;
        StartNextCut();
    }

    void LateUpdate()
    {
        if (sessionTimer <= 0f) return;

        // Countdown timer
        sessionTimer -= Time.deltaTime;

        TrackSawMovement();
        currentCutTimer += Time.deltaTime;

        if (sessionTimer <= 0f)
        {
            CompleteSession();
        }
    }

    void TrackSawMovement()
    {
        float currentZ = saw.localPosition.z;
        float zMovement = Mathf.Abs(currentZ - lastZ);

        if (zMovement > 0.005f) // Threshold for movement detection
        {
            saw.localPosition += Vector3.down * downwardRate * Time.deltaTime;
        }

        lastZ = currentZ;

        if (saw.localPosition.y <= logBottomY)
        {
            CompleteCurrentCut();
        }
    }

    void CompleteCurrentCut()
    {
        completedCuts++;
        Debug.Log($"Cut {completedCuts} complete! Time: {currentCutTimer:F2}s");

        currentCutTimer = 0f;
        StartNextCut();
    }

    void StartNextCut()
    {
        // Pick the next X position in order
        float selectedX = logXPositions[currentXIndex];

        // Move to next index, wrap around if needed
        currentXIndex = (currentXIndex + 1) % logXPositions.Length;

        Vector3 pos = saw.localPosition;
        pos.x = selectedX;     // move left/right
        pos.y = startLocalY;   // reset height
        saw.localPosition = pos;
    }

    void CompleteSession()
    {
        OnGameComplete?.Invoke(this);
        Debug.Log($"Session Complete! Total cuts: {completedCuts}");
        // Optionally, you can trigger UI or callback to send this value to your scoring system
    }

    public void ResetGame()
    {
        Debug.Log("Resetting Sawing Game");

        sessionTimer = sessionDuration;
        completedCuts = 0;
        currentCutTimer = 0f;
        currentXIndex = 0;


        // Reset saw height
        Vector3 pos = saw.localPosition;
        pos.y = startLocalY;
        saw.localPosition = pos;

        lastZ = saw.localPosition.z;

        StartNextCut();
    }

    public bool IsActive()
    {
        return gameObject.activeInHierarchy && sessionTimer > 0f;
    }
}