using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    //audio options references
    public Slider musicSlider;
    public Text musicVolText;
    public Slider sfxSlider;
    public Text sfxVolText;
    public Slider uiSlider;
    public Text uiVolText;

    public AudioManager theAM;

    //graphic options references
    public Text resText;
    public Toggle fullScreenToggle;
    public Toggle vsyncToggle;

    public List<ResolutionItem> resolutions = new List<ResolutionItem>();
    public int selectedRes;


	// Use this for initialization
	void Start()
	{
        if (theAM == null)
        {
            theAM = FindObjectOfType<AudioManager>();
        }

        //Set starting audio values
        theAM.CheckAudioLoaded();

        musicSlider.value = theAM.GetMusicVol() * 100f;
        sfxSlider.value = theAM.GetSFXVol() * 100f;
        uiSlider.value = theAM.GetSFXVol() * 100f;
        UpdateVolLabels();

        //set starting graphics values
        fullScreenToggle.isOn = Screen.fullScreen;
        vsyncToggle.isOn = QualitySettings.vSyncCount == 0 ? false : true;

        bool foundRes = false;
        for(int i = 0; i < resolutions.Count; i++)
        {
            if(Screen.width == resolutions[i].width && Screen.height == resolutions[i].height)
            {
                selectedRes = i;
                foundRes = true;
            }
        }
        if(!foundRes)
        {
            resolutions.Add(new ResolutionItem(Screen.width, Screen.height));
            selectedRes = resolutions.Count - 1;
        }

        resText.text = resolutions[selectedRes].width + " x " + resolutions[selectedRes].height;
    }

    //Update is called once per frame
    void Update()
	{
		
	}

    public void UpdateVolLabels()
    {
        musicVolText.text = Mathf.Round(musicSlider.value).ToString();
        sfxVolText.text = Mathf.Round(sfxSlider.value).ToString();
        uiVolText.text = Mathf.Round(uiSlider.value).ToString();
    }

    public void AdjustMusicVol()
    {
        theAM.SetMusicVol(musicSlider.value / 100f);
        UpdateVolLabels();
    }

    public void AdjustSFXVol()
    {
        theAM.SetSFXVol(sfxSlider.value / 100f);
        UpdateVolLabels();
    }

    public void AdjustUIVol()
    {
        theAM.SetUIVol(sfxSlider.value / 100f);
        UpdateVolLabels();
    }


    public void ResSelectLeft()
    {
        if (selectedRes > 0)
        {
            selectedRes--;
        }

        resText.text = resolutions[selectedRes].width + " x " + resolutions[selectedRes].height;
    }

    public void ResSelectRight()
    {
        if (selectedRes < resolutions.Count - 1)
        {
            selectedRes++;
        }

        resText.text = resolutions[selectedRes].width + " x " + resolutions[selectedRes].height;
    }

    public void ApplyResolution()
    {
        Screen.SetResolution(resolutions[selectedRes].width, resolutions[selectedRes].height, fullScreenToggle.isOn);
    }

    public void SetFullScreen()
    {
        if (fullScreenToggle.isOn)
        {
            Screen.fullScreen = true;
        }
        else
        {
            Screen.fullScreen = false;
        }
    }

    public void SetVsync()
    {
        if(vsyncToggle.isOn)
        {
            QualitySettings.vSyncCount = 1;
        } else
        {
            QualitySettings.vSyncCount = 0;
        }
    }

    public void ApplyChanges()
    {
        ApplyResolution();
        SetFullScreen();
        SetVsync();


        //make sure audio changes will be applied on exit
        theAM.SaveData();
    }

    public void CloseMenu()
    {
        //reset audioLevels based saved data. This will ensure if changes aren't applied in menu, audio will revert to last setting
        theAM.LoadData();


        gameObject.SetActive(false);
    }
}