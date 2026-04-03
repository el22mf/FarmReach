using UnityEngine;

public class FullScreenHandler : MonoBehaviour
{
    // This is the function you hook to On Value Changed (bool)
    public void SetFullScreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
        Debug.Log("Full-screen set to: " + Screen.fullScreen);

    }
}