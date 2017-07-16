using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour {

    public PlayerActions player;
    public Bracciante[] braccianti;

	// Use this for initialization
	void Start () {
        player.isMyTurn = true;
        for(int i=0;i<braccianti.Length;i++)
        {
            braccianti[i].isMyTurn = false;
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
