using UnityEngine;

public class KinematicsProcessor : MonoBehaviour
{
    public SerialReader serialReader;

    public float r1 = 39f;//Arm Length
    public float r2 = 23f;//Hand Length

    public float armAngleKinematic;
    public float handAngleKinematic;
    public float wristAngleKinematic;
    public float zeroThreshold = 0.01f;

    public float x;
    public float y;

    void Update()
    {
        if (serialReader == null)
            return;

        float rawArm = serialReader.armAngle;
        float rawHand = serialReader.handAngle;

        // Convert arm convention:
        // raw: left=+90, mid=0, right=-90
        // kin: left=0, mid=90, right=180
        // Clamp raw between 90 to - 90
        armAngleKinematic = 90f - rawArm;

        // Adjust hand too if needed
        handAngleKinematic = rawHand;

        float t1 = armAngleKinematic * Mathf.Deg2Rad;
        float t2 = handAngleKinematic * Mathf.Deg2Rad;


        x = r1 * Mathf.Cos(t1) + r2 * Mathf.Cos(t1 + t2);
        y = r1 * Mathf.Sin(t1) + r2 * Mathf.Sin(t1 + t2);


        if (Mathf.Abs(x) < zeroThreshold)
            x = 0f;

        if (Mathf.Abs(y) < zeroThreshold)
            y = 0f;
        //x = 0;
        //y = 0;
    }
}