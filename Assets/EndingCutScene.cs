using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class EndingCutScene : MonoBehaviour
{
    
    AudioSource audio;
    VideoPlayer video;
    public VideoClip[] videoToPlay;
	// Use this for initialization
	void Start ()
    {
        video = GetComponent<VideoPlayer>();
        audio = GetComponent<AudioSource>();
        video.Stop();
        video.clip = ClipToPlay();
        video.SetTargetAudioSource(0,audio);
        video.Play();
        StartCoroutine(VideoTimer((float) video.clip.length));

	}

     void Update()
     {
        if (Input.GetMouseButton(0) || Input.GetKeyDown("escape") || Input.GetKeyDown("space"))
        {
            SceneManager.LoadScene("MenuIniziale");
        }
     }


    VideoClip ClipToPlay()
    {
        int goodCounter = 0, badCounter = 0;


        for (int i = 0; i < FreeRoamingPos.karmaLevel.Length; i++)
        {
            if (FreeRoamingPos.karmaLevel[i] == 0)
            {
                goodCounter++;
            }
            else if (FreeRoamingPos.karmaLevel[i] == 1)
            {
                badCounter++;
            }

        }
        if (goodCounter == FreeRoamingPos.karmaLevel.Length)
        {
            //finale bello
            return videoToPlay[0];
        }
        else if (badCounter == FreeRoamingPos.karmaLevel.Length)
        {
            //finale cattivo!!! non si fa!!!
            return videoToPlay[1];
        }
        else
        {
            return videoToPlay[2];
            //finale boh, schifo

        }
    }

    IEnumerator VideoTimer(float videoLenght)
    {
        yield return new WaitForSeconds(videoLenght);
        SceneManager.LoadScene("MenuIniziale");

    }
   
}

