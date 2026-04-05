using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Geometry;

public class KinematicsProcessor : MonoBehaviour
{
    [Header("Mode")]
    public bool useROS = false;

    [Header("References")]
    public SerialReader serialReader;

    [Header("Arm Lengths")]
    public float r1 = 39f;
    public float r2 = 23f;

    [Header("Kinematic Angles")]
    public float armAngleKinematic;
    public float handAngleKinematic;

    [Header("Output")]
    public float x;
    public float y;

    public float zeroThreshold = 0.01f;

    // ===================== ROS =====================
    [Header("ROS Settings")]
    public string publishTopic = "/robot_joint_angles";
    public string subscribeTopic = "/robot_end_effector";
    public float rosFrequency = 50f;

    private ROSConnection ros;
    private float timer;

    private Pose2DMsg latestMsg;

    // ===================== INIT =====================
    void Start()
    {
        if (useROS)
        {
            ros = ROSConnection.GetOrCreateInstance();

            ros.RegisterPublisher<Pose2DMsg>(publishTopic);
            ros.Subscribe<Pose2DMsg>(subscribeTopic, ReceiveKinematics);

            Debug.Log("ROS Mode Enabled");
        }
    }

    // ===================== UPDATE =====================
    void Update()
    {
        if (serialReader == null)
            return;

        if (useROS)
        {
            HandleROS();
        }
        else
        {
            HandleUnityKinematics();
        }
    }

    // ===================== UNITY KINEMATICS =====================
    void HandleUnityKinematics()
    {
        float rawArm = serialReader.armAngle;
        float rawHand = serialReader.handAngle;

        // Convert arm convention
        armAngleKinematic = 90f - rawArm;
        handAngleKinematic = rawHand;

        float t1 = armAngleKinematic * Mathf.Deg2Rad;
        float t2 = handAngleKinematic * Mathf.Deg2Rad;

        x = r1 * Mathf.Cos(t1) + r2 * Mathf.Cos(t1 + t2);
        y = r1 * Mathf.Sin(t1) + r2 * Mathf.Sin(t1 + t2);

        ApplyZeroClamp();
    }

    // ===================== ROS MODE =====================
    void HandleROS()
    {
        timer += Time.deltaTime;

        if (timer >= 1f / rosFrequency)
        {
            PublishJointAngles();
            ApplyROSKinematics();
            timer = 0f;
        }
    }

    void PublishJointAngles()
    {
        Pose2DMsg msg = new Pose2DMsg
        {
            x = serialReader.armAngle,
            y = serialReader.handAngle,
            theta = serialReader.wristAngle
        };

        ros.Publish(publishTopic, msg);
    }

    void ReceiveKinematics(Pose2DMsg msg)
    {
        latestMsg = msg;
    }

    void ApplyROSKinematics()
    {
        if (latestMsg == null)
            return;

        x = (float)latestMsg.x;
        y = (float)latestMsg.y;

        ApplyZeroClamp();
    }

    // ===================== UTIL =====================
    void ApplyZeroClamp()
    {
        if (Mathf.Abs(x) < zeroThreshold)
            x = 0f;

        if (Mathf.Abs(y) < zeroThreshold)
            y = 0f;
    }
}