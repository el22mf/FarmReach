using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Std;

public class ChatterPublisher : MonoBehaviour
{
    ROSConnection ros;
    public string topicName = "/chatter";

    // How often to publish (seconds)
    public float publishMessageFrequency = 0.5f;

    // Timer to track elapsed time
    private float timeElapsed;

    void Start()
    {
        // Get or create the ROS connection
        ros = ROSConnection.GetOrCreateInstance();

        // Register this topic
        ros.RegisterPublisher<StringMsg>(topicName);
    }

    void Update()
    {
        timeElapsed += Time.deltaTime;

        if (timeElapsed > publishMessageFrequency)
        {
            // Create the message
            StringMsg message = new StringMsg("Hello from Unity at " + System.DateTime.Now.ToString());

            // Publish to ROS
            ros.Publish(topicName, message);

            // Reset timer
            timeElapsed = 0f;

            // Optional debug
            Debug.Log("Published message: " + message.data);
        }
    }
}