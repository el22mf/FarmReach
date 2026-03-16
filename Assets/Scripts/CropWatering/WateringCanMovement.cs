using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WateringCanMovement : MonoBehaviour
{
    public float moveSpeed = 3f;

    public float tiltSpeed = 50f; // how fast it tilts when you press Q/E

    void Update()
    {
        HandleMovement();
        HandleTilt();
    }

    void HandleMovement()
    {
        float h = -(Input.GetAxis("Horizontal")); // A / D keys
        float v = -(Input.GetAxis("Vertical")); // W / S keys

        Vector3 move = new Vector3(h, 0f, v) * moveSpeed * Time.deltaTime;

        // Move in world space so it doesn't rotate weirdly
        transform.Translate(move, Space.World);
    }

    void HandleTilt()
    {
        // Tilt forward/back with Q/E (this will control pouring)
        if (Input.GetKey(KeyCode.Q))
        {
            transform.Rotate(Vector3.right, tiltSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.E))
        {
            transform.Rotate(Vector3.right, -tiltSpeed * Time.deltaTime);
        }
    }
}
