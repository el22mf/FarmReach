using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandMovement : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float tiltSpeed = 150f;

    private HandToggle handToggle;

    [Header("References")]
    public KinematicsProcessor kinematicsProcessor;
    public SerialReader serialReader;

    [Header("Kinematic Output Range")]
    public float kinematicXMin = -41f;
    public float kinematicXMax = 41f;
    public float kinematicYMin = 14f;
    public float kinematicYMax = 62f;

    [Header("World Limits")]
    public float worldXMin = 0f;
    public float worldXMax = 5f;
    public float worldYMin = 0f;
    public float worldYMax = 5f;

    public float normalized_x = 0f;
    public float normalized_y = 0f;

    public float all_offset_x = 0f;
    public float all_offset_y = 0f;

    private Vector3 startPosition;


    void Start()
    {
        handToggle = GetComponent<HandToggle>();
        startPosition = transform.position;
    }

    void Update()
    {
        HandleTilt();

        if (handToggle != null && !handToggle.CanTranslate)
            return;

       //ApplyKinematicOffset();
       HandleMovement();

    }

    void HandleMovement()
    {
        float h = -(Input.GetAxis("Horizontal")); // A / D keys
        float v = Input.GetAxis("Vertical"); // W / S keys

        Vector3 move = new Vector3(h, v, 0f) * moveSpeed * Time.deltaTime;

        // Move in world space so it doesn't rotate weirdly
        transform.Translate(move, Space.World);
    }

    void ApplyKinematicOffset()
    {
        if (kinematicsProcessor == null)
            return;

        float rawX = kinematicsProcessor.x;
        float rawY = kinematicsProcessor.y;

        // Normalize to 0-1
        float x01 = Mathf.InverseLerp(kinematicXMin, kinematicXMax, rawX);
        float y01 = Mathf.InverseLerp(kinematicYMin, kinematicYMax, rawY);

        // Map to world space
        float targetX = Mathf.Lerp(worldXMin, worldXMax, x01);
        float targetY = Mathf.Lerp(worldYMin, worldYMax, y01);

        normalized_x = targetX;
        normalized_y = targetY;

        all_offset_x = rawX;
        all_offset_y = rawY;

        // Clamp
        targetX = Mathf.Clamp(targetX, worldXMin, worldXMax);
        targetY = Mathf.Clamp(targetY, worldYMin, worldYMax);

        // Apply ONLY X and Y (leave Z untouched)
        Vector3 pos = transform.position;
        pos.x = targetX;
        pos.y = targetY;
        transform.position = pos;
    }

    void HandleTilt()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            transform.Rotate(Vector3.forward, -tiltSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.E))
        {
            transform.Rotate(Vector3.forward, tiltSpeed * Time.deltaTime);
        }
    }
}