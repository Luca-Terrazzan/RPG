﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour {

    public GameObject[] charactersArray;
    [SerializeField] private int i = 0;
    


    // Use this for initialization
    void Start()
    {
        changeTurn();
	}
	// Update is called once per frame
	void Update ()
    {
       
    }
    /// <summary>
    /// Cambio turno delle singole istnze dell'array 
    /// </summary>
    public void changeTurn()
    {
        if (charactersArray[i].gameObject.tag == "Bracciante")
        {
            charactersArray[i].GetComponent<Bracciante>().StartTurn();
        }
        else if (charactersArray[i].gameObject.tag == "Prostituta")
        {
            charactersArray[i].GetComponent<RagazzaAmbiziosa>().isMyTurn = true;
        }
        else if (charactersArray[i].gameObject.tag == "CowBoy")
        {
            charactersArray[i].GetComponent<RagazzoMucca>().StartTurn();
            Debug.Log("sonodentro");
        }
        else if (charactersArray[i].gameObject.tag == "Player")
        {
            charactersArray[i].GetComponent<PlayerActions>().isMyTurn = true;
        }
        if (i < charactersArray.Length-1)
        {
            i++;
        }
        else
        {
            i = 0;
        }
     
    }
}
