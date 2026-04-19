using System.Collections.Generic;
using UnityEngine;

public class centralGameManager : MonoBehaviour
{
    public List<MonoBehaviour> minigameScripts; // assign all minigame scripts implementing IMinigame
    public GameObject metricsPanel;

    private List<IMinigame> minigames = new List<IMinigame>();

    void Awake()
    {
        foreach (var mg in minigameScripts)
        {
            if (mg is IMinigame ig)
            {
                minigames.Add(ig);

                //Subscribe to completion event
                ig.OnGameComplete = HandleGameComplete;
            }
            else
            {
                Debug.LogWarning($"Script {mg.name} does not implement IMinigame!");
            }
        }
    }

    void Update()
    {
        // Show metrics panel if any minigame is active
    }

    void HandleGameComplete(IMinigame game)
    {
        Debug.Log("Game completed!");

        Time.timeScale = 0f;
        metricsPanel.SetActive(false);

        var metrics = game.GetGameMetrics();
        GameCompleteManager.Instance.ShowResults(metrics, game);
    }


    /// <summary>
    /// Reset all minigames
    /// </summary>
    public void ResetAllMinigames()
    {
        foreach (var mg in minigames)
            mg.ResetGame();
    }
}