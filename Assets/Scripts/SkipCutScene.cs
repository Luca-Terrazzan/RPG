using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class SkipCutScene : MonoBehaviour
{
    VideoPlayer videoIntro;
    
    

    private void Start()
    {
        
        videoIntro = GetComponent<VideoPlayer>();
        StartCoroutine(VideoTimer((float)videoIntro.clip.length));

       
    }
    private void Update()
    {

        if (Input.GetMouseButton(0)||Input.GetKeyDown("escape") || Input.GetKeyDown("space"))
        {
            SceneManager.LoadScene("Tutorial"); 
        }

        
    }

    IEnumerator VideoTimer(float clipLenght)
    {
        yield return new WaitForSeconds(clipLenght);
        SceneManager.LoadScene("Tutorial");
    }
}
