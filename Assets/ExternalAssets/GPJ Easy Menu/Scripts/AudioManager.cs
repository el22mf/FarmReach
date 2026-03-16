using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


using System.Xml;
using System.Xml.Serialization;

public class AudioManager : MonoBehaviour
{
    private float musicVolume;
    private float sfxVolume;
    private float UIVolume;

    private bool audioLoaded;

    public AudioSaveData theSave;

    public AudioSource[] musicSources;
    private float[] musicStartVol;
    public AudioSource[] sfxSources;
    private float[] sfxStartVol;
    public AudioSource[] uiSources;
    private float[] uiStartVol;

	// Use this for initialization
	void Start()
	{
        CheckAudioLoaded();

        
	}

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    //Update is called once per frame
    void Update()
	{
		
	}

    public void CheckAudioLoaded()
    {
        if(!audioLoaded)
        {
            //set default volume levels
            musicStartVol = new float[musicSources.Length];
            for(int i = 0; i < musicSources.Length; i++)
            {
                musicStartVol[i] = musicSources[i].volume;
            }
            sfxStartVol = new float[sfxSources.Length];
            for (int i = 0; i < sfxSources.Length; i++)
            {
                sfxStartVol[i] = sfxSources[i].volume;
            }
            uiStartVol = new float[uiSources.Length];
            for (int i = 0; i < uiSources.Length; i++)
            {
                uiStartVol[i] = uiSources[i].volume;
            }


            LoadData();
            audioLoaded = true;
        }
    }

    public float GetMusicVol()
    {
        return musicVolume;
    }

    public float GetSFXVol()
    {
        return sfxVolume;
    }

    public float GetUIVol()
    {
        return UIVolume;
    }

    public void SetMusicVol(float newVol)
    {
        musicVolume = newVol;
        ApplyVolumes();
    }

    public void SetSFXVol(float newVol)
    {
        sfxVolume = newVol;
        ApplyVolumes();
    }

    public void SetUIVol(float newVol)
    {
        UIVolume = newVol;
        ApplyVolumes();
    }

    public void ApplyVolumes()
    {
        for(int i = 0; i < musicSources.Length; i++)
        {
            musicSources[i].volume = musicStartVol[i] * musicVolume;
        }
        for (int i = 0; i < sfxSources.Length; i++)
        {
            sfxSources[i].volume = sfxStartVol[i] * sfxVolume;
        }
        for (int i = 0; i < uiSources.Length; i++)
        {
            uiSources[i].volume = uiStartVol[i] * UIVolume;
        }
    }

    public void PlayMusic(int trackNumber)
    {
        if (trackNumber < musicSources.Length && trackNumber >= 0)
        {

            for (int i = 0; i < musicSources.Length; i++)
            {
                musicSources[i].Stop();
            }
            musicSources[trackNumber].Play();
        } else
        {
            Debug.LogError("Selected a music number outside music source size");
        }
    }

    public void PlaySFX(int sfxNumber)
    {
        if (sfxNumber < sfxSources.Length && sfxNumber >= 0)
        {
            sfxSources[sfxNumber].Play();
            sfxSources[sfxNumber].Play();
        }
        else
        {
            Debug.LogError("Selected a sfx number outside sfx source size");
        }
    }

    public void PlayUI(int uiNumber)
    {
        if (uiNumber < uiSources.Length && uiNumber >= 0)
        {
            uiSources[uiNumber].Stop();
            uiSources[uiNumber].Play();
        }
        else
        {
            Debug.LogError("Selected a ui audio number outside ui audio source size");
        }
    }

    public void LoadData()
    {
        string dataPath = Application.persistentDataPath;

        if (System.IO.File.Exists(dataPath + "/audio.data"))
        {
            var serializer = new XmlSerializer(typeof(AudioSaveData));
            var stream = new FileStream(dataPath + "/audio.data", FileMode.Open);
            theSave = serializer.Deserialize(stream) as AudioSaveData;
            stream.Close();


            musicVolume = theSave.musicVol;
            sfxVolume = theSave.sfxVol;
            UIVolume = theSave.uiVol;

            ApplyVolumes();
        }
    }

    public void SaveData()
    {
        theSave.musicVol = musicVolume;
        theSave.sfxVol = sfxVolume;
        theSave.uiVol = UIVolume;

        Debug.Log("Saving Data");


        string dataPath = Application.persistentDataPath;

        var serializer = new XmlSerializer(typeof(AudioSaveData));
        var stream = new FileStream(dataPath + "/audio.data", FileMode.Create);
        serializer.Serialize(stream, theSave);
        stream.Close();
    }
}