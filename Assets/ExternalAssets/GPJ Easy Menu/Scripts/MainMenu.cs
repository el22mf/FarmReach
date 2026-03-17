using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class MainMenu : MonoBehaviour
{

    public GameObject optionsScreen;
    public GameObject instructionsScreen;
    public GameObject quitConfirmMenu;

    public GameObject mainMenuPanel;
    //public GameObject gameUIPanel;     // HUD
    public GameObject metricsPanel;    // Your minigame metrics canvas
    public GameObject pausePanel;
    public GameObject loginPanel;

    public CinemachineVirtualCamera[] menuVCams;   // 5 menu cameras
    public CinemachineVirtualCamera overviewVCam;  // gameplay camera

    public centralGameManager centralGameManager;
    public GameObject gameCompleteCanvas;
    public GameObject overviewMenuCanvas;

    public static MainMenu Instance;
    public CameraManager cameraManager; // assign via inspector


    // Use this for initialization
    void Awake()
	{
        Instance = this;
        ActivateMenuCamera();
        metricsPanel.SetActive(false);
        overviewMenuCanvas.SetActive(false); 
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


    //Start New Game
    public void PlayAsGuest()
    {
        Debug.Log("Starting session as Guest");

        mainMenuPanel.SetActive(false);
        //gameUIPanel.SetActive(true);

        GameState.sessionActive = true;
        Time.timeScale = 1f;

        if (pausePanel != null)
        {
            var pauseMenu = pausePanel.GetComponent<PauseMenu>();
            if (pauseMenu != null)
                pauseMenu.ResetPause();
        }

        if (cameraManager != null)
            cameraManager.SwitchToOverview();
    }

    public static class GameState
    {
        public static bool sessionActive = false;
    }

    //Continue Game (Ideally used for loading to a level Select)
    public void OpenLogin()
    {
        loginPanel.SetActive(true);
    }

    public void CloseLogin()
    {
        loginPanel.SetActive(false);
    }

    public void LoginConfirmed(string username)
    {
        Debug.Log("Logged in as: " + username);

        loginPanel.SetActive(false);
        mainMenuPanel.SetActive(false);

        //gameUIPanel.SetActive(true);
        overviewMenuCanvas.SetActive(true);


        GameState.sessionActive = true;

        if (pausePanel != null)
        {
            var pauseMenu = pausePanel.GetComponent<PauseMenu>();
            if (pauseMenu != null)
                pauseMenu.ResetPause();
        }

        if (cameraManager != null)
            cameraManager.SwitchToOverview();
    }

    public void OpenPause()
    {
        pausePanel.SetActive(true);
        GameState.sessionActive = false;
        Time.timeScale = 0f;
    }

    public void ClosePause()
    {
        pausePanel.SetActive(false);
        GameState.sessionActive = true;
        Time.timeScale = 1f;
    }
    public void BackToMainMenu()
    {
        // Hide all gameplay UI
        //gameUIPanel.SetActive(false);
        metricsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
        gameCompleteCanvas.SetActive(false);
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

    //Open Panel for Options
    public void OpenOptions()
    {
        optionsScreen.SetActive(true);
    }

    //Close options Panel
    public void CloseOptions()
    {
        optionsScreen.SetActive(false);
    }

    //Open Instructions Panel
    public void ShowInstructions()
    {
        instructionsScreen.SetActive(true);
    }

    //Close Instructions Panel
    public void CloseInstructions()
    {
        instructionsScreen.SetActive(false);
    }

    //Open Quit Confirm Dialog
    public void PressQuit()
    {
        quitConfirmMenu.SetActive(true);
    }

    //Exit the game
    public void ConfirmQuit()
    {
        Application.Quit();
    }

    //Cancel quitting the game
    public void CancelQuit()
    {
        quitConfirmMenu.SetActive(false);
    }
}