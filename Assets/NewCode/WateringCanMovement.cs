using UnityEngine;

public class WateringCanMovement : MonoBehaviour
{
    [Header("References")]
    public KinematicsProcessor kinematicsProcessor;
    public SerialReader serialReader;

    [Header("Manual WASD Movement")]
    public float moveSpeed = 3f;
    private Vector3 manualOffset = Vector3.zero;

    [Header("Kinematic Output Range")]
    public float kinematicXMin = -41f; //39
    public float kinematicXMax = 41f; //39
    public float kinematicYMin = 14f; //25
    public float kinematicYMax = 62f; //25

    [Header("World Limits")]
    public float worldXMin = 0f;
    public float worldXMax = 5f;
    public float worldZMin = -2.5f;
    public float worldZMax = -0.5f;

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
        HandleMovement();
        ApplyKinematicOffset();
        ApplyWristTilt();
    }

    void HandleMovement()
    {
        float h = -Input.GetAxis("Horizontal");
        float v = -Input.GetAxis("Vertical");

        Vector3 move = new Vector3(h, 0f, v) * moveSpeed * Time.deltaTime;
        manualOffset += move;
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


        transform.localPosition = new Vector3(targetX,0, targetZ);
    }

    void ApplyWristTilt()
    {
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