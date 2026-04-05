using UnityEngine;

public class Apple : MonoBehaviour
{
    [HideInInspector] public Vector3 startPos;
    [HideInInspector] public Quaternion startRot;

    [HideInInspector] public bool isPicked; // Optional: you can remove if not used

    void Awake()
    {
        // Save initial position and rotation
        startPos = transform.position;
        startRot = transform.rotation;
    }

    // Helper to reset the apple
    public void ResetApple()
    {
        transform.position = startPos;
        transform.rotation = startRot;
        gameObject.SetActive(true); // Make sure it's visible
        isPicked = false;           // optional
    }
}