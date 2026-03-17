using TMPro; // For text fields
using UnityEngine;
using System.Collections.Generic;

public class GameCompleteManager : MonoBehaviour
{
    public static GameCompleteManager Instance;

    [Header("UI References")]
    public GameObject gameCompleteCanvas;
    public TMP_Text metricsText;

    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        gameCompleteCanvas.SetActive(false);
    }

    /// <summary>
    /// Show the game complete screen with stats
    /// </summary>
    public void ShowResults(Dictionary<string, float> metrics)
    {
        gameCompleteCanvas.SetActive(true);

        if (metricsText != null)
        {
            metricsText.text = "";
            foreach (var kvp in metrics)
            {
                metricsText.text += $"{kvp.Key}: {kvp.Value:F2}\n";
            }
        }

        Time.timeScale = 0f; // pause gameplay
    }

    public void Hide()
    {
        gameCompleteCanvas.SetActive(false);
        Time.timeScale = 1f;
    }
}