using System;
using System.Collections.Generic;
using UnityEngine;

public class ApplePickingGameManager : MonoBehaviour, IMinigame
{
    [Header("Apple Tree Setup")]
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
    private int currentTargetIndex = 0;
    private GameObject activeGlow;

    private float timeSinceLastSample = 0f;

    private bool sessionStarted = false;

    HandToggle hand;

    public Action<IMinigame> OnGameComplete { get; set; }
    public Dictionary<string, float> GetGameMetrics()
    {
        return new Dictionary<string, float>
    {
        { "Average Time", FinalAverageTime }
    };
    }

    void Start()
    {
        sessionStarted = true;
        InitializeApples();


        // Initialize apples from layer
        if (targetApples.Count > 0)
        {
            currentTargetIndex = 0;
            PickNextTarget(false);
        }
        else
        {
            Debug.LogError("No target apples after initialization!");
        }
    }

    void Update()
    {

        if (hand == null)
            hand = FindAnyObjectByType<HandToggle>();

        Apple current = GetCurrentTarget();
        if (current == null)
        {
            if (!sessionCompleted && targetAccuracies.Count > 0)
            {
                finalAverageAccuracy = GetSessionAverageAccuracy();
                finalAverageTime = GetAverageAppleTime();
                sessionCompleted = true;

                OnGameComplete?.Invoke(this);

                Debug.Log(
                    $"Apple Picking Complete! Avg Accuracy: {finalAverageAccuracy:F2}, Avg Time: {finalAverageTime:F2}s"
                );
            }
            return;
        }

        // Accuracy Sampling
        timeSinceLastSample += Time.deltaTime;
        timeSinceLastSample += Time.deltaTime;
        if (timeSinceLastSample >= 0.2f && hand != null)
        {
            timeSinceLastSample = 0f;

            float grabAngle = hand.GetSupinationAngle();
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
        // Clear old lists
        targetApples.Clear();

        List<Apple> allApples = new List<Apple>();

        // Find all Apple scripts in the scene
        Apple[] applesInScene = FindObjectsOfType<Apple>(true); // 'true' includes inactive objects
        allApples.AddRange(applesInScene);

        Debug.Log("Runtime allApples count: " + allApples.Count);
        foreach (var a in allApples)
            Debug.Log("Apple found: " + a.name);

        // Randomly select targets
        List<Apple> available = new List<Apple>(allApples);
        for (int i = 0; i < totalTargets && available.Count > 0; i++)
        {
            int randIndex = UnityEngine.Random.Range(0, available.Count);
            Apple target = available[randIndex];
            targetApples.Add(target);
            available.RemoveAt(randIndex);
        }

    }



    public void OnAppleDeposited(Apple apple)
    {
        Apple current = GetCurrentTarget();
        //if (current == null) return;

        // Only count if it's the current target
        //if (apple != current)
        //{
        //    Debug.Log("Deposited apple was not the current target");
        //    return;
        //}

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

    public void PickNextTarget(bool increment = true)
    {
        if (activeGlow != null)
        {
            Destroy(activeGlow);
            activeGlow = null;
        }

        if (increment)
            currentTargetIndex++;

        if (currentTargetIndex >= targetApples.Count)
        {
            sessionCompleted = true;
            return;
        }

        Apple target = targetApples[currentTargetIndex];

        activeGlow = Instantiate(glowPrefab, target.transform.position, Quaternion.identity, target.transform);
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
        currentTargetIndex = 0;
        targetApples.Clear();

        // Destroy glow if it exists
        if (activeGlow != null)
        {
            Destroy(activeGlow);
            activeGlow = null;
        }

        // Reset all apples
        // Re-find all apples in the scene and reset them
        Apple[] applesInScene = FindObjectsOfType<Apple>(true);

        foreach (Apple apple in applesInScene)
        {
            if (apple != null)
            {
                apple.isPicked = false;       // reset picked flag
                apple.gameObject.SetActive(true);  // make sure it's visible again
            }
        }

        // Reinitialize targets
        InitializeApples();
        PickNextTarget();
    }

    public bool IsActive()
    {
        return sessionStarted && !sessionCompleted;
    }
}
