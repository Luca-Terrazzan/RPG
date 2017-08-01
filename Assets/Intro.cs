﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Intro : MonoBehaviour {

    //public bool stop = true;

    private Image wakandaLogo, backgorundLogo;
    private Button startGame, toDesktop;


    // Use this for initialization
    void Start ()
    {
        wakandaLogo = transform.GetChild(1).GetComponentInChildren<Image>();
        backgorundLogo = transform.GetChild(0).GetComponent<Image>();
        startGame = transform.GetChild(2).GetComponentInChildren<Button>();
        toDesktop = transform.GetChild(3).GetComponentInChildren<Button>();

    }
	


	// Update is called once per frame
	void Update ()
    {
        
        if (wakandaLogo.gameObject.transform.position.y <755 )
        {
            wakandaLogo.gameObject.transform.position = Vector3.Lerp(wakandaLogo.gameObject.transform.position,
              new Vector3(wakandaLogo.transform.position.x, wakandaLogo.transform.position.y + 200F, wakandaLogo.transform.position.z)
              , Time.deltaTime);
        }
        else
        {
            backgorundLogo.gameObject.SetActive(true);
            startGame.gameObject.SetActive(true);
            toDesktop.gameObject.SetActive(true);
            
        }
        
    }
}
