using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SawMovement : MonoBehaviour
{
    public float moveSpeed = 3f;

    [Header("References")]
    public KinematicsProcessor kinematicsProcessor;
    public SerialReader serialReader;

    [Header("Kinematic Output Range")]
    public float kinematicXMin = -41f; //39
    public float kinematicXMax = 41f; //39
    public float kinematicYMin = 14f; //25
    public float kinematicYMax = 62f; //25

    [Header("World Limits")]
    public float worldXMin = 0f;
    public float worldXMax = 0f;
    public float worldZMin = 0f;
    public float worldZMax = 1f;

    [Header("Wrist Tilt")]
    public bool invertWristTilt = false;
    public float wristTiltOffset = 0f;

    public float normalized_x = 0f;
    public float normalized_z = 0f;
    public float all_offset_z = 0f;
    public float all_offset_x = 0f;

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        //HandleMovement();
        ApplyKinematicOffset();
    }

    void HandleMovement()
    {
        float v = -(Input.GetAxis("Vertical")); // W / S keys (Z axis)

        Vector3 pos = transform.position;
        pos.z += v * moveSpeed * Time.deltaTime;

        transform.position = pos;
    }

    void ApplyKinematicOffset()
    {
        if (kinematicsProcessor == null)
            return;

        float rawX = kinematicsProcessor.x;
        float rawY = kinematicsProcessor.y;

        float x01 = Mathf.InverseLerp(kinematicXMin, kinematicXMax, rawX);
        float y01 = Mathf.InverseLerp(kinematicYMin, kinematicYMax, rawY);

        float targetX = Mathf.Lerp(worldXMin, worldXMax, x01);
        float targetZ = Mathf.Lerp(worldZMin, worldZMax, y01);

        normalized_x = targetX;
        normalized_z = targetZ;
        all_offset_x = rawX;
        all_offset_z = rawY;

        //targetX += manualOffset.x;
        //targetZ += manualOffset.z;

        targetX = Mathf.Clamp(targetX, worldXMin, worldXMax);
        targetZ = Mathf.Clamp(targetZ, worldZMin, worldZMax);


        Vector3 pos = transform.localPosition;
        pos.z = targetZ;
        transform.localPosition = pos;
    }

}
