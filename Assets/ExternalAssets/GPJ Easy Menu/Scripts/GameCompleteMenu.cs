using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class GameCompleteMenu : MonoBehaviour
{


    public GameObject mainMenuPanel;
    //public GameObject gameUIPanel;     // HUD
    public GameObject metricsPanel;    // Your minigame metrics canvas
    public GameObject pausePanel;

    public CinemachineVirtualCamera[] menuVCams;   // 5 menu cameras
    public CinemachineVirtualCamera overviewVCam;  // gameplay camera

    public centralGameManager centralGameManager;

    public GameObject overviewMenuCanvas;


    // Use this for initialization
    void Awake()
	{
        metricsPanel.SetActive(false);
    }

    //Update is called once per frame
    void Update()
	{
		
	}

    void ActivateMenuCamera()
    {
        // Lower all priorities first
        foreach (var vcam in menuVCams)
            vcam.Priority = 0;

        if (overviewVCam != null)
            overviewVCam.Priority = 0;

        // Pick one random menu camera
        int index = Random.Range(0, menuVCams.Length);
        menuVCams[index].Priority = 10;
    }

    public void ActivateOverviewCamera()
    {
        // Disable all menu cameras
        foreach (var vcam in menuVCams)
            vcam.Priority = 0;

        // Enable overview
        overviewVCam.Priority = 10;
    }

    //Start New Game
    public void PlayAsGuest()
    {
        Debug.Log("Starting session as Guest");

        mainMenuPanel.SetActive(false);
        //gameUIPanel.SetActive(true);
        overviewMenuCanvas.SetActive(true);

        GameState.sessionActive = true;
        Time.timeScale = 1f;

        if (pausePanel != null)
        {
            var pauseMenu = pausePanel.GetComponent<PauseMenu>();
            if (pauseMenu != null)
                pauseMenu.ResetPause();
        }

        ActivateOverviewCamera();
    }

    public static class GameState
    {
        public static bool sessionActive = false;
    }

    public void BackToMainMenu()
    {
        // Hide all gameplay UI
        //gameUIPanel.SetActive(false);
        metricsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
        overviewMenuCanvas.SetActive(false);


        centralGameManager.ResetAllMinigames();

        // Reset minigames
        GameObject[] activeGames = GameObject.FindGameObjectsWithTag("Minigame");
        foreach (var g in activeGames)
            g.SetActive(false);

        // Reset session state
        GameState.sessionActive = false;
        Time.timeScale = 1f;

        // Reset pause menu
        if (pausePanel != null)
        {
            var pauseMenu = pausePanel.GetComponent<PauseMenu>();
            if (pauseMenu != null)
                pauseMenu.ResetPause();
        }

        ActivateMenuCamera();
    }

}