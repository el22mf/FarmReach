using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Std;
using System;
using System.Collections.Generic;
using TMPro; // For text fields
using UnityEngine;

public class GameCompleteManager : MonoBehaviour
{
    public static GameCompleteManager Instance;

    [Header("UI References")]
    public GameObject gameCompleteCanvas;
    public TMP_Text metricsText;

    private ROSConnection ros;
    private IMinigame currentGame;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<StringMsg>("/game_complete");

        gameCompleteCanvas.SetActive(false);
    }

    public void SendGameComplete(string gameType, float score)
    {
        GameResult result = new GameResult
        {
            gameType = gameType,
            score = score
        };

        string json = JsonUtility.ToJson(result);
        ros.Publish("/game_complete", new StringMsg(json));
    }
    public void ShowResults(Dictionary<string, int> metrics, IMinigame game)
    {
        gameCompleteCanvas.SetActive(true);

        if (metricsText != null)
        {
            metricsText.text = "";
            foreach (var kvp in metrics)
            {
                metricsText.text += $"{kvp.Key}: {kvp.Value}\n";
            }
        }

        SendGameComplete(
            game.GetGameType(),
            metrics["Final Score"]
        );

        Time.timeScale = 0f; // pause gameplay
    }

    public void Hide()
    {
        gameCompleteCanvas.SetActive(false);
        Time.timeScale = 1f;
    }
}