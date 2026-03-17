using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MainMenu;

public class PauseMenu : MonoBehaviour
{

    public bool isPaused;
    private float storeTimeScale;

    public GameObject pauseScreen;
    public GameObject optionsScreen;
    public GameObject instructionsScreen;

    public GameObject mainMenuPanel;
    public GameObject metricsPanel;    // Your minigame metrics canvas
    public GameObject pausePanel;

    public CinemachineVirtualCamera[] menuVCams;   // 5 menu cameras
    public CinemachineVirtualCamera overviewVCam;  // gameplay camera

    public centralGameManager centralGameManager;
    public GameCompleteManager gameCompleteManager;

    public MainMenu mainMenu;
    public GameObject gameCompleteCanvas;
    public GameObject overviewMenuCanvas;
    public CameraManager cameraManager;

    // Use this for initialization
    void Start()
	{
        storeTimeScale = Time.timeScale;
	}

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseUnpause();
        }

    }

    public void PauseUnpause()
    {
        isPaused = !isPaused;
        pauseScreen.SetActive(isPaused);

        if(isPaused)
        {
            storeTimeScale = Time.timeScale;
            Time.timeScale = 0f;
        } else
        {
            Time.timeScale = storeTimeScale;
            optionsScreen.SetActive(false);
            instructionsScreen.SetActive(false);
        }
    }

    public void LevelSelect()
    {
        Debug.Log("Back to level select");

        metricsPanel.SetActive(false);
        mainMenuPanel.SetActive(false);
        //gameUIPanel.SetActive(true);
        //overviewMenuCanvas.SetActive(true);
        gameCompleteCanvas.SetActive(false);

        centralGameManager.ResetAllMinigames();

        // Reset minigames
        GameObject[] activeGames = GameObject.FindGameObjectsWithTag("Minigame");
        foreach (var g in activeGames)
            g.SetActive(false);

        // Reset session state
        GameState.sessionActive = true;
        Time.timeScale = 1f;

        if (pausePanel != null)
        {
            var pauseMenu = pausePanel.GetComponent<PauseMenu>();
            if (pauseMenu != null)
                pauseMenu.ResetPause();
        }

        if (cameraManager != null)
        {
            cameraManager.SwitchToOverview();

        }
        else
        {
            Debug.LogWarning("MainMenu reference not set in PauseMenu");
        }
    }


    public void OpenOptions()
    {
        optionsScreen.SetActive(true);
    }

    public void CloseOptions()
    {
        optionsScreen.SetActive(false);
    }

    public void OpenInstructionsScreen()
    {
        instructionsScreen.SetActive(true);
    }

    public void CloseInstructionsScreen()
    {
        instructionsScreen.SetActive(false);
    }

    public void ResetPause()
    {
        isPaused = false;
        Time.timeScale = 1f;

        pauseScreen.SetActive(false);
        optionsScreen.SetActive(false);
        instructionsScreen.SetActive(false);
    }

}