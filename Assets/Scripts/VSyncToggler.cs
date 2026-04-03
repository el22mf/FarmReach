using UnityEngine;

public class VSyncHandler : MonoBehaviour
{
    public void SetVSync(bool enabled)
    {
        QualitySettings.vSyncCount = enabled ? 1 : 0;
    }
}