using UnityEngine;
using static MainMenu;

public class clickScript : MonoBehaviour
{
    public string zoneName;
    public int zoneIndex;

    // Minigames specific to this zone
    public GameObject[] minigames;

    public GameObject metricsPanel;
    public GameObject overviewMenuCanvas;

    private void Start()
    {
        // Ensure all local minigames start inactive
        foreach (var game in minigames)
        {
            if (game != null)
                game.SetActive(false);
        }
    }

    private void OnMouseDown()
    {
        if (!GameState.sessionActive) return;

        Debug.Log("Clicked: " + zoneName);

        // Switch cameras
        CameraManager cam = FindAnyObjectByType<CameraManager>();
        if (cam != null)
        {
            cam.SwitchToZone(zoneIndex);
        }

        // Deactivate all local minigames first
        foreach (var game in minigames)
        {
            if (game != null)
                game.SetActive(false);
        }

        // Activate the first minigame in this zone
        if (minigames.Length > 0 && minigames[0] != null)
        {
            minigames[0].SetActive(true);
            metricsPanel.SetActive(true);
            overviewMenuCanvas.SetActive(false);
        }
    }
}