using System;
using System.Collections.Generic;
using UnityEngine;

public class ApplePickingGameManager : MonoBehaviour, IMinigame
{
    [Header("Apple Tree Setup")]
    public List<Apple> allApples;        // All apples in the scene
    public GameObject glowPrefab;        // Prefab for the sparkle/glow on target apple

    [Header("Target Settings")]
    public int totalTargets = 5;         // Number of apples to pick per run

    [Header("Accuracy Settings")]
    public float perfectAngle = 45f;     // Example: ideal supination angle for grabbing
    public float angleTolerance = 15f;   // +/- degrees for scoring

    [Header("Accuracy Tracking")]
    private List<float> targetAccuracies = new List<float>();
    private float currentTargetAccuracySum = 0f;
    private int currentTargetFrames = 0;

    [Header("Timing Tracking")]
    private List<float> appleTimes = new List<float>();
    private float currentAppleTimer;
    public float CurrentAppleTimer => currentAppleTimer;

    private float finalAverageAccuracy = 0f;
    public float FinalAverageAccuracy => finalAverageAccuracy;
    private float finalAverageTime = 0f;
    public float FinalAverageTime => finalAverageTime;

    private bool sessionCompleted = false;
    public bool IsSessionComplete => sessionCompleted;

    private List<Apple> targetApples = new List<Apple>();
    private int currentTargetIndex = -1;
    private GameObject activeGlow;

    private float timeSinceLastSample = 0f;

    public Action<IMinigame> OnGameComplete { get; set; }

    public Dictionary<string, float> GetGameMetrics()
    {
        return new Dictionary<string, float>
        {
            { "Average Time", FinalAverageTime },
            { "Average Accuracy", FinalAverageAccuracy }
        };
    }
    void Start()
    {
        InitializeApples();
        PickNextTarget();
    }

    void Update()
    {
        Apple current = GetCurrentTarget();
        if (current == null)
        {
            if (!sessionCompleted && targetAccuracies.Count > 0)
            {
                finalAverageAccuracy = GetSessionAverageAccuracy();
                finalAverageTime = GetAverageAppleTime();
                sessionCompleted = true;

                Debug.Log(
                    $"Apple Picking Complete! Avg Accuracy: {finalAverageAccuracy:F2}, Avg Time: {finalAverageTime:F2}s"
                );
            }
            return;
        }

        // Accuracy Sampling
        timeSinceLastSample += Time.deltaTime;
        if (timeSinceLastSample >= 0.2f)
        {
            timeSinceLastSample = 0f;

            float grabAngle = FindAnyObjectByType<HandToggle>().GetSupinationAngle();
            float accuracy = GetAccuracy(grabAngle);

            currentTargetAccuracySum += accuracy;
            currentTargetFrames++;
        }

        // Timer
        currentAppleTimer += Time.deltaTime;
    }

    public float GetAccuracy(float angle)
    {
        float delta = Mathf.Abs(angle - perfectAngle);
        float accuracy = 1f - Mathf.Clamp01(delta / angleTolerance);
        return accuracy;
    }

    public float GetSessionAverageAccuracy()
    {
        if (targetAccuracies.Count == 0) return 0f;
        float sum = 0f;
        foreach (float a in targetAccuracies) sum += a;
        return sum / targetAccuracies.Count;
    }

    public float GetAverageAppleTime()
    {
        if (appleTimes.Count == 0) return 0f;
        float sum = 0f;
        foreach (float t in appleTimes) sum += t;
        return sum / appleTimes.Count;
    }

    void InitializeApples()
    {
        targetApples.Clear();
        List<Apple> available = new List<Apple>(allApples);

        for (int i = 0; i < totalTargets && available.Count > 0; i++)
        {
            int randIndex = UnityEngine.Random.Range(0, available.Count);
            Apple target = available[randIndex];
            targetApples.Add(target);
            available.RemoveAt(randIndex);

            // Optionally, spawn glow here for the first target
        }
    }



    public void OnAppleDeposited(Apple apple)
    {
        Apple current = GetCurrentTarget();
        if (current == null) return;

        // Only count if it's the current target
        if (apple != current)
        {
            Debug.Log("Deposited apple was not the current target");
            return;
        }

        float averageAccuracy =
            (currentTargetFrames > 0)
            ? currentTargetAccuracySum / currentTargetFrames
            : 0f;

        targetAccuracies.Add(averageAccuracy);
        appleTimes.Add(currentAppleTimer);

        Debug.Log(
            $"Apple {currentTargetIndex + 1} deposited! Avg Accuracy: {averageAccuracy:F2}, Time: {currentAppleTimer:F2}s"
        );

        // Reset per-target tracking
        currentTargetAccuracySum = 0f;
        currentTargetFrames = 0;
        currentAppleTimer = 0f;

        CompleteCurrentTarget();
    }


    public void CompleteCurrentTarget()
    {
        PickNextTarget();
    }

    public void PickNextTarget()
    {
        Apple current = GetCurrentTarget();

        // Destroy previous glow first
        if (activeGlow != null)
            Destroy(activeGlow);

        currentTargetIndex++;

        if (currentTargetIndex >= targetApples.Count)
        {
            sessionCompleted = true; // mark complete
            current = null;
            return;
        }

        Apple target = targetApples[currentTargetIndex];

        activeGlow = Instantiate(glowPrefab, target.transform.position, Quaternion.identity, target.transform);

        Debug.Log("Current target: " + target.name + " (Index " + currentTargetIndex + ")");
    }

    public Apple GetCurrentTarget()
    {
        if (currentTargetIndex < 0 || currentTargetIndex >= targetApples.Count)
            return null;

        Apple target = targetApples[currentTargetIndex];

        if (target == null)
            return null;

        return target;
    }

    public void ResetGame()
    {
        Debug.Log("Resetting Apple Picking Game");

        // Reset session state
        sessionCompleted = false;

        // Clear tracking data
        targetAccuracies.Clear();
        appleTimes.Clear();

        currentTargetAccuracySum = 0f;
        currentTargetFrames = 0;
        currentAppleTimer = 0f;

        finalAverageAccuracy = 0f;
        finalAverageTime = 0f;

        timeSinceLastSample = 0f;

        // Reset target tracking
        currentTargetIndex = -1;
        targetApples.Clear();

        // Destroy glow if it exists
        if (activeGlow != null)
        {
            Destroy(activeGlow);
            activeGlow = null;
        }

        // Reset all apples
        foreach (var apple in allApples)
        {
            if (apple != null)
            {

                // If apples get disabled visually when picked,
                // ensure they are visible again:
                apple.gameObject.SetActive(true);
            }
        }

        // Reinitialize targets
        InitializeApples();
        PickNextTarget();
    }

    public bool IsActive()
    {
        return gameObject.activeInHierarchy && !sessionCompleted;
    }
} 
 