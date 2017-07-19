using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagazzaAmbiziosa : MonoBehaviour {

    public TurnManager turnManager;

    public bool isMyTurn;
    bool imDead;
     
	// Use this for initialization
	void Start ()
    { 
        if (imDead)
        {
            turnManager.changeTurn();
            return;
        }

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Die()
    {
        imDead = true;
        //anim dead
    }
}
