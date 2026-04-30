using UnityEngine;

public class TestCursorMovement : MonoBehaviour
{
    [Header("References")]
    public KinematicsProcessor kinematicsProcessor;
    public SerialReader serialReader;

    [Header("Manual WASD Movement")]
    public float moveSpeed = 3f;
    private Vector3 manualOffset = Vector3.zero;

    [Header("Kinematic Output Range")]
    public float kinematicXMin = -41f;
    public float kinematicXMax = 41f;
    public float kinematicYMin = 14f;
    public float kinematicYMax = 62f;

    [Header("World Limits")]
    public float worldXMin = 0f;
    public float worldXMax = 5f;
    public float worldYMin = -2.5f;
    public float worldYMax = -0.5f;

    [Header("Wrist Tilt")]
    public bool invertWristTilt = false;
    public float wristTiltOffset = 0f;

    [Header("Smoothing")]
    public float positionSmoothing = 10f;

    public float normalized_x = 0f;
    public float normalized_y = 0f;
    public float raw_x = 0f;
    public float raw_y = 0f;

    private Vector3 startPosition;
    private Vector3 targetPosition;

    void Start()
    {
        startPosition = transform.position;
        targetPosition = startPosition;
    }

    void Update()
    {
        HandleMovement();
        ApplyKinematicOffset();
        ApplyWristTilt();
    }

    void HandleMovement()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(h, 0f, v) * moveSpeed * Time.deltaTime;
        manualOffset += move;
    }

    void ApplyKinematicOffset()
    {
        if (kinematicsProcessor == null)
            return;

        float rawX = -kinematicsProcessor.x;
        float rawY = kinematicsProcessor.y;

        raw_x = rawX;
        raw_y = rawY;

        float x01 = Mathf.InverseLerp(kinematicXMin, kinematicXMax, rawX);
        float y01 = Mathf.InverseLerp(kinematicYMin, kinematicYMax, rawY);

        float targetX = Mathf.Lerp(worldXMin, worldXMax, x01);
        float targetY = Mathf.Lerp(worldYMin, worldYMax, y01);

        normalized_x = targetX;
        normalized_y = targetY;

        // Add manual offset (WASD influence)
        targetX += manualOffset.x;
        targetY += manualOffset.y;

        // Clamp final position
        targetX = Mathf.Clamp(targetX, worldXMin, worldXMax);
        targetY = Mathf.Clamp(targetY, worldYMin, worldYMax);

        Vector3 desiredPosition = new Vector3(targetX, targetY, startPosition.z);
        targetPosition = desiredPosition;
    }

    void ApplyWristTilt()
    {
        if (serialReader == null)
            return;

        float wrist = serialReader.wristAngle;

        wrist = Mathf.Clamp(wrist, -90f, 90f);

        if (invertWristTilt)
            wrist = -wrist;

        wrist += wristTiltOffset;

        Vector3 euler = transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(wrist, euler.y, euler.z);
    }

    void LateUpdate()
    {
        // Smooth final movement
        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            Time.deltaTime * positionSmoothing
        );
    }
}