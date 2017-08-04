using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TutorialNavigation : MonoBehaviour {

    private Button next, back, exit;
    public Image[] tutorialImages;
    private PlayerActions player;
    private bool isPlayerExisting;

    private int i = 0; 
	// Use this for initialization
	void Start ()
    {
        
        next = transform.GetChild(1).GetComponent<Button>();
        back = transform.GetChild(0).GetComponent<Button>();
        exit = transform.GetChild(2).GetComponent<Button>();
        if (GameObject.Find("Player") != null)
        {
            player = GameObject.Find("Player").GetComponent<PlayerActions>();
            Debug.Log("ciao");
            isPlayerExisting = true;
        }
        next.onClick.AddListener(NextImage);
        back.onClick.AddListener(PreviousImage);
        exit.onClick.AddListener(Exit);

	}

     void NextImage ()
    {

        if (i < tutorialImages.Length -1 )
        {
            i++;
            tutorialImages[i].gameObject.SetActive(true);

            if (i > 0)
            { 
            tutorialImages[i - 1].gameObject.SetActive(false);
            }
           
            
        }
    }

    void PreviousImage()
    {
        if (i <  tutorialImages.Length )
        {
            if (i > 0)
            {
                i--;
                tutorialImages[i].gameObject.SetActive(true);

            }
            tutorialImages[i + 1].gameObject.SetActive(false);

        }
    }
    void Exit()
    {
        if (!isPlayerExisting)
        {
            SceneManager.LoadScene("GhostTown");
        }
        else
        {
            gameObject.SetActive(false);
            GameObject.Find("TutorialObjects").SetActive(false);
            player.CloseMenu();
        }
       
       
    }
}
