using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{

    public bool isPaused;
    private float storeTimeScale;

    public GameObject pauseScreen;
    public GameObject optionsScreen;
    public GameObject instructionsScreen;

	// Use this for initialization
	void Start()
	{
        storeTimeScale = Time.timeScale;
	}

	//Update is called once per frame
	void Update()
	{
		if(Input.GetKeyDown(KeyCode.Escape))
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

    public void OpenOptionsScreen()
    {
        optionsScreen.SetActive(true);
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