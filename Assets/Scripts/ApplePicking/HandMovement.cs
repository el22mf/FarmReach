using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandMovement : MonoBehaviour
{
    public float moveSpeed = 3f;

    public float tiltSpeed = 150f; // how fast it tilts when you press Q/E

    private HandToggle handToggle;

    void Start()
    {
        handToggle = GetComponent<HandToggle>();
    }

    void Update()
    {
        HandleTilt();
        if (handToggle != null && !handToggle.CanTranslate)
            return;
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

    void HandleTilt()
    {
        // Tilt forward/back with Q/E (this will control pouring)
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

