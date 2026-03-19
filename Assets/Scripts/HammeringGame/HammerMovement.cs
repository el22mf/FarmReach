using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HammerMovement : MonoBehaviour
{

    public float tiltSpeed = 50f; // how fast it tilts when you press Q/E

    void Update()
    {
        HandleTilt();
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
