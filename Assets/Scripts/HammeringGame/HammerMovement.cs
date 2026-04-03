using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HammerMovement : MonoBehaviour
{
    public KinematicsProcessor kinematicsProcessor;
    public SerialReader serialReader;

    public bool invertWristTilt = false;
    public float wristTiltOffset = 0f;

    public float tiltSpeed = 50f; // how fast it tilts when you press Q/E

    void Update()
    {
        HandleTilt();
    }

    void HandleTilt()
    {
        // Tilt forward/back with Q/E (this will control pouring)
        if (serialReader == null)
            return;

        float wrist = serialReader.wristAngle;

        //Clamp it
        wrist = Mathf.Clamp(wrist, -90f, 90f);

        if (invertWristTilt)
            wrist = -wrist;

        wrist += wristTiltOffset;

        Vector3 euler = transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(wrist, euler.y, euler.z);
    }
}
