using System;
using System.Collections.Generic;
using UnityEngine;

public class CropWateringGameManager : MonoBehaviour, IMinigame
{
    [Header("Cabbage Patch Setup")]
    public List<GrowCabbage> allCabbages;       // All cabbages in the patch
    public GameObject glowPrefab;                // Prefab for the sparkle/glow

    [Header("Target Settings")]
    public int totalTargets = 5;                 // How many cabbages per run
    public float targetStartScale = 0.2f;        // Initial scale for target cabbages
    public float nonTargetMinScale = 0.4f;       // Random min scale for other cabbages
    public float nonTargetMaxScale = 1.0f;       // Random max scale for other cabbages

    [Header("Accuracy Settings")]
    public float perfectAngle = 75f;      // 45deg absolute ideal tilt, plus 30deg visual adjustment
    public float angleTolerance = 50f;   // +/- 45 to 105deg for "medium" accuracy, real 15deg to 75deg

    [Header("Accuracy Tracking")]
    private List<float> targetAccuracies = new List<float>(); // per-target average
    private float currentTargetAccuracySum = 0f; // accumulate accuracy
    private int currentTargetFrames = 0;         // count frames for averaging

    private float finalAverageAccuracy = 0f;
    public float FinalAverageAccuracy => finalAverageAccuracy;



    [Header("Accuracy Sampling")]
    public float samplingInterval = 0.2f; // every 0.2s => 5 times per second
    private float timeSinceLastSample = 0f;

    [Header("Speed Tracking")]
    private List<float> cabbageTimes = new List<float>(); // Time to fully grow each cabbage

    private float currentCabbageTimer;
    public float CurrentCabbageTimer => currentCabbageTimer;   // Timer for current cabbage

    private bool sessionCompleted = false;

    private float finalAverageTime = 0f;
    public float FinalAverageTime => finalAverageTime;

    public bool IsSessionComplete => sessionCompleted;

    private bool sessionStarted = false;


    public Action<IMinigame> OnGameComplete { get; set; }

    public Dictionary<string, float> GetGameMetrics()
    {
        return new Dictionary<string, float>
        {
            { "Average Time", FinalAverageTime },
            { "Average Accuracy", FinalAverageAccuracy }
        };
    }


    private List<GrowCabbage> targetCabbages = new List<GrowCabbage>();
    private int currentTargetIndex = -1;
    private GameObject activeGlow;

    void Start()
    {
        sessionStarted = true;

        InitializeCabbages();
        PickNextTarget();
    }

    void Update()
    {
        GrowCabbage current = GetCurrentTarget();
        if (current == null)
        {
            if (!sessionCompleted && targetAccuracies.Count > 0)
            {
                finalAverageAccuracy = GetSessionAverageAccuracy();
                finalAverageTime = GetAverageCabbageTime();
                sessionCompleted = true;

                OnGameComplete?.Invoke(this);

                Debug.Log(
                    $"Session complete! Avg Accuracy: {finalAverageAccuracy:F2}, Avg Time: {finalAverageTime:F2}s"
                );
            }

            return;
        }


        // Accuracy Sampling (run every samplingInterval seconds)
        timeSinceLastSample += Time.deltaTime;
        if (timeSinceLastSample >= samplingInterval)
        {
            timeSinceLastSample = 0f;
            float tilt = FindAnyObjectByType<WateringCan>().currentTilt;
            float accuracy = GetAccuracy(tilt);

            // Accumulate for per-cabbage average
            currentTargetAccuracySum += accuracy;
            currentTargetFrames++;

            // Optional: debug
            // Debug.Log($"Tilt: {tilt:F1}, Sampled Accuracy: {accuracy:F2}");
        }

        // Speed Timer
        currentCabbageTimer += Time.deltaTime;

        // Growth Check
        float currentSize = current.cabbageVisual.localScale.magnitude;
        float fullSize = current.fullScale.magnitude;

        if (Mathf.Approximately(currentSize, fullSize) || currentSize >= fullSize * 0.99f)
        {
            // Save Average Accuracy for this cabbage
            float averageAccuracy = (currentTargetFrames > 0) ? currentTargetAccuracySum / currentTargetFrames : 0f;
            targetAccuracies.Add(averageAccuracy);

            // Save Time Taken for this cabbage
            cabbageTimes.Add(currentCabbageTimer);

            Debug.Log($"Target {currentTargetIndex + 1} completed! Avg Accuracy: {averageAccuracy:F2}, Time: {currentCabbageTimer:F2}s");

            // Reset per-target accumulators
            currentTargetAccuracySum = 0f;
            currentTargetFrames = 0;
            currentCabbageTimer = 0f;

            CompleteCurrentTarget();
        }
    }


    public float GetAccuracy(float tiltAngle)
    {
        float delta = Mathf.Abs(tiltAngle - perfectAngle); // difference from ideal
        float maxDelta = angleTolerance; // beyond this, accuracy = 0
        float curve = 1f; // exponent for shaping the drop-off

        // Compute smooth accuracy
        float accuracy = 1f - Mathf.Pow(Mathf.Clamp01(delta / maxDelta), curve);

        return accuracy;
    }

    public float GetSessionAverageAccuracy()
    {
        if (targetAccuracies.Count == 0) return 0f;

        float sum = 0f;
        foreach (float a in targetAccuracies)
            sum += a;

        return sum / targetAccuracies.Count;
    }

    public float GetAverageCabbageTime()
    {
        if (cabbageTimes.Count == 0) return 0f;
        float sum = 0f;
        foreach (float t in cabbageTimes) sum += t;
        return sum / cabbageTimes.Count;
    }

    void InitializeCabbages()
    {
        // Randomly select the target cabbages
        targetCabbages.Clear();
        List<GrowCabbage> available = new List<GrowCabbage>(allCabbages);

        for (int i = 0; i < totalTargets && available.Count > 0; i++)
        {
            int randIndex = UnityEngine.Random.Range(0, available.Count);
            GrowCabbage target = available[randIndex];
            targetCabbages.Add(target);
            available.RemoveAt(randIndex);

            // Set target cabbage to starting size
            target.SetScale(targetStartScale);
        }

        // Set all other cabbages to random size
        foreach (GrowCabbage cabbage in available)
        {
            float randomScale = UnityEngine.Random.Range(nonTargetMinScale, nonTargetMaxScale);
            cabbage.SetScale(randomScale);
        }
    }

    public void PickNextTarget()
    {
        // Remove old glow
        if (activeGlow != null)
        {
            Destroy(activeGlow);
            activeGlow = null;
        }

        currentTargetIndex++;

        if (currentTargetIndex >= targetCabbages.Count)
        {
            Debug.Log("All targets completed!");
            return;
        }

        GrowCabbage target = targetCabbages[currentTargetIndex];

        // Spawn glow on current target
        activeGlow = Instantiate(glowPrefab, target.transform.position, Quaternion.identity, target.transform);
    }


    public void CompleteCurrentTarget()
    {
        PickNextTarget();
    }

    public GrowCabbage GetCurrentTarget()
    {
        if (currentTargetIndex < 0 || currentTargetIndex >= targetCabbages.Count)
            return null;
        return targetCabbages[currentTargetIndex];
    }

    public void ResetGame()
    {
        // Reset tracking variables
        targetAccuracies.Clear();
        cabbageTimes.Clear();
        currentTargetAccuracySum = 0f;
        currentTargetFrames = 0;
        currentCabbageTimer = 0f;
        finalAverageAccuracy = 0f;
        finalAverageTime = 0f;
        sessionCompleted = false;

        // Remove any active glow
        if (activeGlow != null)
        {
            Destroy(activeGlow);
            activeGlow = null;
        }

        currentTargetIndex = -1;

        // Re-initialize cabbage scales and pick new targets
        InitializeCabbages();

        // Pick first target again
        PickNextTarget();
    }

    public bool IsActive()
    {
        return sessionStarted && !sessionCompleted; // true while minigame is running
    }
}
