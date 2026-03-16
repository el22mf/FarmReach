using UnityEngine;

public class SawingMetricsBridge : MonoBehaviour
{
    public MinigameMetrics metricsUI;      // Assign your metrics UI panel
    public float updateInterval = 0.2f;    // Update UI every 0.2s

    private float timeSinceLastUpdate = 0f;
    private SawingGameManager gameManager;

    void Start()
    {
        gameManager = FindAnyObjectByType<SawingGameManager>();
        if (gameManager == null)
        {
            Debug.LogWarning("No SawingGameManager found in scene!");
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

        string display;

        if (!gameManager.IsSessionComplete)
        {
            float timeLeft = gameManager.SessionTimeLeft;
            int totalCuts = gameManager.CompletedCuts;

            display = $"Time: {timeLeft:F1}s\nCuts: {totalCuts}";
        }
        else
        {
            int totalCuts = gameManager.CompletedCuts;
            display = $"Session Complete!\nTotal Cuts: {totalCuts}";
        }

        metricsUI.SetMetrics(display);
    }
}