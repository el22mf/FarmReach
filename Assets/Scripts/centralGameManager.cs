using System.Collections.Generic;
using UnityEngine;

public class centralGameManager : MonoBehaviour
{
    public List<MonoBehaviour> minigameScripts; // assign all minigame scripts implementing IMinigame


    private List<IMinigame> minigames = new List<IMinigame>();

    void Awake()
    {
        // Populate minigames list from the scripts
        foreach (var mg in minigameScripts)
        {
            if (mg is IMinigame ig)
                minigames.Add(ig);
            else
                Debug.LogWarning($"Script {mg.name} does not implement IMinigame!");
        }

    }

    void Update()
    {
        // Show metrics panel if any minigame is active
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