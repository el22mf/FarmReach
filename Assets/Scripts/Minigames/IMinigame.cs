using System;
using System.Collections.Generic;
using UnityEngine;

public interface IMinigame
{
    void ResetGame();
    bool IsActive();
    public Action<IMinigame> OnGameComplete { get; set; }
    Dictionary<string, int> GetGameMetrics();
    string GetGameType();
}