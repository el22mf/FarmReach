using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SawMovement : MonoBehaviour
{
    public float moveSpeed = 3f;

    void Update()
    {
        HandleMovement();
    }

    void HandleMovement()
    {
        float v = -(Input.GetAxis("Vertical")); // W / S keys (Z axis)

        Vector3 pos = transform.position;
        pos.z += v * moveSpeed * Time.deltaTime;

        transform.position = pos;
    }

}
