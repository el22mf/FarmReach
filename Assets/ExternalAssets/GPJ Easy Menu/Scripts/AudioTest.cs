using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioTest : MonoBehaviour
{
    public InputField trackInput;
    public AudioManager theAM;

	// Use this for initialization
	void Start()
	{
		
	}

	//Update is called once per frame
	void Update()
	{
		
	}

    public void PlayMusic()
    {
        theAM.PlayMusic(int.Parse(trackInput.text));
    }

    public void PlaySFX()
    {
        theAM.PlaySFX(int.Parse(trackInput.text));
    }

    public void PlayUI()
    {
        theAM.PlayUI(int.Parse(trackInput.text));
    }
}