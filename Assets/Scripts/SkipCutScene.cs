using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SkipCutScene : MonoBehaviour {

    private void Update()
    {
        if (Input.GetMouseButton(0)||Input.GetKeyDown("escape"))
        {
            SceneManager.LoadScene("Tutorial"); 
        }
    }



}
