using TMPro;
using UnityEngine;

public class MinigameMetrics : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI metricsText;    // Assign the TMP text in inspector
    public float refreshRate = 0.2f;       // Update text every 0.2s

    private float timeSinceLastUpdate = 0f;

    private string currentDisplay = "";

    void Update()
    {
        timeSinceLastUpdate += Time.deltaTime;
        if (timeSinceLastUpdate >= refreshRate)
        {
            timeSinceLastUpdate = 0f;
            metricsText.text = currentDisplay;
        }
    }

    // Public method to update the text content
    public void SetMetrics(string displayText)
    {
        currentDisplay = displayText;
    }
}
