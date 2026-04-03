using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Geometry; // We'll use Pose2DMsg for x, y, theta

public class KinematicsPublisher : MonoBehaviour
{
    [Header("References")]
    public SerialReader serialReader;

    [Header("ROS Settings")]
    public string topicName = "/robot_joint_angles";
    public float publishFrequency = 50f; // Hz

    private ROSConnection ros;
    private float timer;

    void Start()
    {
        // Initialize ROS connection
        ros = ROSConnection.GetOrCreateInstance();

        // Register the topic using Pose2DMsg: x = armAngle, y = handAngle, theta = wristAngle
        ros.RegisterPublisher<Pose2DMsg>(topicName);
    }

    void Update()
    {
        if (serialReader == null)
            return;

        timer += Time.deltaTime;

        if (timer >= 1f / publishFrequency)
        {
            PublishKinematics();
            timer = 0f;
        }
    }

    void PublishKinematics()
    {
        // Read angles from SerialReader
        float armAngle = serialReader.armAngle;
        float handAngle = serialReader.handAngle;
        float wristAngle = serialReader.wristAngle;

        // Create ROS message
        Pose2DMsg msg = new Pose2DMsg
        {
            x = armAngle,
            y = handAngle,
            theta = wristAngle
        };

        // Publish
        ros.Publish(topicName, msg);

        // Optional debug
        Debug.Log($"Published Kinematics - Arm: {armAngle:F2}, Hand: {handAngle:F2}, Wrist: {wristAngle:F2}");
    }
}