using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Std; // For Int32MultiArrayMsg

public class RosArmXYReader : MonoBehaviour
{
    // ROS Connector
    private ROSConnection ros;

    // ROS topic to subscribe to
    public string topicName = "/arm_xy";

    void Start()
    {
        // Get ROS connector
        ros = ROSConnection.GetOrCreateInstance();

        // Subscribe to the topic
        ros.Subscribe<Int32MultiArrayMsg>(topicName, ArmXYCallback);

        Debug.Log("Subscribed to " + topicName);
    }

    // Callback when message is received
    private void ArmXYCallback(Int32MultiArrayMsg msg)
    {
        if (msg.data != null && msg.data.Length >= 2)
        {
            int X = msg.data[0];
            int Y = msg.data[1];

            Debug.Log($"[ROS] Arm position: X = {X} mm, Y = {Y} mm");
        }
        else
        {
            Debug.LogWarning("[ROS] Received message with insufficient data.");
        }
    }
}