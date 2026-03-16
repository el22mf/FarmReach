using System.Collections;
using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    public CinemachineVirtualCamera overviewCamera;
    public CinemachineVirtualCamera[] zoneCams;

    private CinemachineBrain brain;

    private void Start()
    {
        // Get CinemachineBrain once
        brain = FindAnyObjectByType<CinemachineBrain>();
        if (MainMenu.GameState.sessionActive)
        {
            SwitchToOverview();
        }
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Escape))
        //{
        //    ExitToOverview();
        //}
    }

    public void SwitchToZone(int index)
    {
        Time.timeScale = 1f; // ensure gameplay running

        overviewCamera.Priority = 0;

        for (int i = 0; i < zoneCams.Length; i++)
        {
            zoneCams[i].Priority = (i == index) ? 10 : 0;
        }
    }

    public void SwitchToOverview()
    {
        overviewCamera.Priority = 10;

        foreach (var cam in zoneCams)
            cam.Priority = 0;
    }

    public void ExitToOverview()
    {
        SwitchToOverview();

        // Deactivate all minigames in the scene
        GameObject[] activeGames = GameObject.FindGameObjectsWithTag("Minigame");
        foreach (var g in activeGames)
        {
            g.SetActive(false);
        }

        // Pause after blend
        StartCoroutine(PauseAfterBlend());
    }

    private IEnumerator PauseAfterBlend()
    {
        float blendTime = 1f; // default if brain missing
        if (brain != null)
            blendTime = brain.m_DefaultBlend.m_Time;

        yield return new WaitForSecondsRealtime(blendTime + 0.05f);
        Time.timeScale = 0f;
    }
}