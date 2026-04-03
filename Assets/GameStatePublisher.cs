using UnityEngine;
using RosMessageTypes.Std;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Geometry;

public class GameStatePublisher : MonoBehaviour
{
    public static GameStatePublisher Instance;

    private ROSConnection ros;
    private string taskCompleteTopic = "/task_complete";
    private string gameCompleteTopic = "/game_complete";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            ros = ROSConnection.GetOrCreateInstance();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PublishTaskComplete(int userId, int gameId, int taskId)
    {
        BoolMsg msg = new BoolMsg(true);
        ros.Publish(taskCompleteTopic, msg);
        Debug.Log($"Published TaskComplete - User: {userId}, Game: {gameId}, Task: {taskId}");
        // Optionally, add metadata using a custom message if needed
    }

    public void PublishGameComplete(int userId, int gameId)
    {
        BoolMsg msg = new BoolMsg(true);
        ros.Publish(gameCompleteTopic, msg);
        Debug.Log($"Published GameComplete - User: {userId}, Game: {gameId}");
        // Optionally, add metadata using a custom message if needed
    }
}