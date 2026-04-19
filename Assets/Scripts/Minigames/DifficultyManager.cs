using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    public static DifficultyManager Instance;
    public float CurrentDifficulty = 1f;

    [Header("Game References")]
    public CropWateringGameManager cropWateringManager;
    public HammeringGameManager hammeringManager;

    public int difficulty = 1;

    private float hitboxMin = 0.25f;
    private float hitboxMax = 0.75f;

    private float loadAngleMin = 10f;
    private float loadAngleMax = 75f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void SetDifficulty(float value)
    {
        difficulty = Mathf.RoundToInt(value);
        ApplyDifficulty();
    }

    private void ApplyDifficulty()
    {
        ApplyCropDifficulty();
        ApplyHammerDifficulty();
    }

    private void ApplyCropDifficulty()
    {
        if (cropWateringManager == null || cropWateringManager.allCabbages == null)
            return;

        float t = (difficulty - 1) / 9f;
        float size = Mathf.Lerp(hitboxMax, hitboxMin, t);

        foreach (var cabbage in cropWateringManager.allCabbages)
        {
            if (cabbage == null) continue;

            BoxCollider col = cabbage.GetComponent<BoxCollider>();
            if (col != null)
                col.size = new Vector3(size, size, size);
        }
    }

    private void ApplyHammerDifficulty()
    {
        if (hammeringManager == null) return;

        float t = (difficulty - 1) / 9f;
        hammeringManager.loadAngle = Mathf.Lerp(loadAngleMin, loadAngleMax, t);
        Debug.Log("Load angle now: " + hammeringManager.loadAngle);
    }

    private void Start()
    {
        ApplyDifficulty();
    }
}