using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TutorialManagment : MonoBehaviour {

    public GameObject tutorialObjects;
    private Image tutorialImage;
    private Button si, no;

   
	// Use this for initialization
	void Start ()
    {
        tutorialObjects = GameObject.Find("TutorialObjects");
        tutorialObjects.SetActive(false);

        tutorialImage = transform.GetChild(0).GetComponentInChildren<Image>();
        si = transform.GetChild(0).GetComponentInChildren<Button>();
        no = transform.GetChild(1).GetComponentInChildren<Button>();

        si.onClick.AddListener(OpenTutorial);
        no.onClick.AddListener(GoToGame);
        
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void OpenTutorial()
    {
        gameObject.SetActive(false);
        tutorialObjects.SetActive(true);

       

    }
    void GoToGame()
    {
        SceneManager.LoadScene("GhostTown");
    }
}
