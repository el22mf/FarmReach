using UnityEngine;

public class ApplePickingMetricsBridge : MonoBehaviour
{
    public MinigameMetrics metricsUI;   // Assign the MinigameMetrics component in inspector
    public float updateInterval = 0.2f; // Update UI every 0.2s
    private float timeSinceLastUpdate = 0f;

    private ApplePickingGameManager gameManager;

    void Start()
    {
        gameManager = FindAnyObjectByType<ApplePickingGameManager>();
        if (gameManager == null)
        {
            Debug.LogWarning("No ApplePickingGameManager found in scene!");
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

        var currentApple = gameManager.GetCurrentTarget();
        if (currentApple != null)
        {
            // Current Accuracy (instant)
            float tilt = FindAnyObjectByType<HandToggle>().CurrentTilt;

            // Current elapsed time for this cabbage
            float elapsed = gameManager.CurrentAppleTimer;

            display = $"Time: {elapsed:F2}s";
        }
        else
        {
            // Session averages after finishing all apples
            float avgTime = gameManager.FinalAverageTime;

            display = $"Avg Time: {avgTime:F2}s";
        }

        metricsUI.SetMetrics(display);
    }
}
