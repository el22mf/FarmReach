using UnityEngine;

public class GrowCabbage : MonoBehaviour
{
    public Transform cabbageVisual; // e.g. Cabbage_01_LOD0

    public float growSpeed = 0.5f;

    public Vector3 fullScale { get; private set; }

    private bool initialised = false;

    void Awake()
    {
        if (cabbageVisual == null)
        {
            // If not set manually, try to grab the first child automatically
            if (transform.childCount > 0)
            {
                cabbageVisual = transform.GetChild(0);
            }
            else
            {
                Debug.LogWarning("GrowCabbage on " + name + " has no cabbageVisual assigned and no children.");
                return;
            }
        }

        fullScale = cabbageVisual.localScale; // full-size mesh
        initialised = true;
    }

    public void AddWater(float amount)
    {
        if (!initialised) return;

        cabbageVisual.localScale = Vector3.MoveTowards(
            cabbageVisual.localScale,
            fullScale,
            growSpeed * amount
        );
    }

    public bool IsFullyGrown()
    {
        return cabbageVisual.localScale == fullScale;
    }

    public void SetScale(float scaleFactor)
    {
        if (!initialised) return;

        cabbageVisual.localScale = fullScale * scaleFactor;
    }
}
