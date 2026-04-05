using UnityEngine;

public class TargetCabbageGlow : MonoBehaviour
{
    public GameObject glowPrefab;
    private GameObject activeGlow;

    public void EnableGlow()
    {
        if (activeGlow == null)
        {
            activeGlow = Instantiate(
                glowPrefab,
                transform.position,
                Quaternion.identity,
                transform
            );
        }
    }

    public void DisableGlow()
    {
        if (activeGlow != null)
        {
            Destroy(activeGlow);
        }
    }
}
