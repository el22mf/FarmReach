using UnityEngine;

public class HammeringMetricsBridge : MonoBehaviour
{
    public MinigameMetrics metricsUI;      // Assign your metrics UI panel
    public float updateInterval = 0.2f;    // Update UI every 0.2s

    private float timeSinceLastUpdate = 0f;
    private HammeringGameManager gameManager;



    void Start()
    {
        gameManager = FindAnyObjectByType<HammeringGameManager>();
        if (gameManager == null)
        {
            Debug.LogWarning("No HammeringGameManager found in scene!");
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
            int totalNails = gameManager.CompletedNails;

            display = $"Time: {timeLeft:F1}s\nNails: {totalNails}";
        }
        else
        {
            int totalNails = gameManager.CompletedNails;
            display = $"Session Complete!\nTotal Cuts: {totalNails}";
        }

        metricsUI.SetMetrics(display);
    }
}