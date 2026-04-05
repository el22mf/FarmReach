using UnityEngine;

public class SystemConfig : MonoBehaviour
{
    // The singleton instance
    public static SystemConfig Instance { get; private set; }

    [Header("Timing")]
    public float physicsHz = 60f;
    public float metricsSamplingHz = 5f;

    void Awake()
    {
        // Enforce singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // Optional for future multi-scene games
        // DontDestroyOnLoad(gameObject);

        // Set physics frequency
        Time.fixedDeltaTime = 1f / physicsHz;
    }

    // Convenience property
    public float SamplingInterval => 1f / metricsSamplingHz;
}
