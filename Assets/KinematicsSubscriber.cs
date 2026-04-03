using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Geometry; // Pose2DMsg

public class KinematicsSubscriber : MonoBehaviour
{
    [Header("ROS Settings")]
    public string topicName = "/robot_end_effector";
    public float updateFrequency = 50f; // Hz

    private float timer;
    private Pose2DMsg latestMsg;

    [Header("Data Output")]
    public float x;
    public float y;
    public float theta;

    private ROSConnection ros;

    void Start()
    {
        // Get or create ROS connection
        ros = ROSConnection.GetOrCreateInstance();

        // Subscribe to the ROS topic
        ros.Subscribe<Pose2DMsg>(topicName, ReceiveKinematics);
        Debug.Log($"Subscribed to ROS topic: {topicName}");
    }

    void ReceiveKinematics(Pose2DMsg msg)
    {
        // Store the latest message
        latestMsg = msg;
    }

    void Update()
    {
        if (latestMsg == null)
            return;

        timer += Time.deltaTime;

        if (timer >= 1f / updateFrequency)
        {
            // Apply latest message
            x = (float)latestMsg.x;
            y = (float)latestMsg.y;
            theta = (float)latestMsg.theta;

            // Optional: move a GameObject, update visuals here

            timer = 0f;
        }
    }
}