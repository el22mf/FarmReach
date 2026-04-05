using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    public CropWateringGameManager cropWateringManager;

    private int difficulty = 1;       // internal storage
    private float hitboxMin = 0.25f;
    private float hitboxMax = 0.75f;

    // Call this from slider OnValueChanged
    public void SetDifficulty(float value)
    {
        difficulty = Mathf.RoundToInt(value);
        ApplyDifficulty();
    }

    private void ApplyDifficulty()
    {
        if (cropWateringManager == null || cropWateringManager.allCabbages == null) return;

        float size = Mathf.Lerp(hitboxMax, hitboxMin, (difficulty - 1) / 9f);

        foreach (var cabbage in cropWateringManager.allCabbages)
        {
            if (cabbage == null) continue;

            BoxCollider col = cabbage.GetComponent<BoxCollider>();
            if (col != null)
                col.size = new Vector3(size, size, size);
        }
        Debug.Log($"Difficulty {difficulty} applied. Cabbage hitbox size: {size:F2}");
    }

    private void Start()
    {
        ApplyDifficulty(); // apply initial difficulty at start
    }
}