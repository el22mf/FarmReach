using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioGenerator : MonoBehaviour
{
    public GameObject audioPrefab;

	// Use this for initialization
	void Start()
	{
		if(!FindObjectOfType<AudioManager>())
        {
            Instantiate(audioPrefab, transform.position, transform.rotation);
        }
	}

	//Update is called once per frame
	void Update()
	{
		
	}
}