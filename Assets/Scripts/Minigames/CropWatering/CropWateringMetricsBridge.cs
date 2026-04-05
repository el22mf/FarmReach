using UnityEngine;

public class CropWateringMetricsBridge : MonoBehaviour
{
    public MinigameMetrics metricsUI;   // Assign the MinigameMetrics component in inspector
    public float updateInterval = 0.2f; // Update UI every 0.2s
    private float timeSinceLastUpdate = 0f;

    private CropWateringGameManager gameManager;

    void Start()
    {
        gameManager = FindAnyObjectByType<CropWateringGameManager>();
        if (gameManager == null)
        {
            Debug.LogWarning("No CropWateringGameManager found in scene!");
        }

        if (metricsUI == null)
        {
            Debug.LogWarning("No MinigameMetrics UI assigned!");
        }
    }

    void Update()
    {
        if (gameManager == null || metricsUI == null) return;

        timeSinceLastUpdate += Time.deltaTime;
        if (timeSinceLastUpdate < updateInterval) return;
        timeSinceLastUpdate = 0f;

        string display = "";

        var currentCabbage = gameManager.GetCurrentTarget();
        if (currentCabbage != null)
        {
            // Current Accuracy (instant)
            float tilt = FindAnyObjectByType<WateringCan>().currentTilt;
            float accuracy = gameManager.GetAccuracy(tilt);

            // Current elapsed time for this cabbage
            float elapsed = gameManager.CurrentCabbageTimer;

            display = $"Accuracy: {accuracy:F2}\nTime: {elapsed:F2}s";
        }
        else
        {
            // Session averages after finishing all cabbages
            float avgAccuracy = gameManager.FinalAverageAccuracy;
            float avgTime = gameManager.FinalAverageTime;

            display = $"Avg Accuracy: {avgAccuracy:F2}\nAvg Time: {avgTime:F2}s";
        }

        metricsUI.SetMetrics(display);
    }
}
