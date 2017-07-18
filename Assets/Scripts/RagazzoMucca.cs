using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagazzoMucca : MonoBehaviour {

    public TurnManager turnManager;
    public PlayerActions player; //da mettere se si vuole gestire la morte del player tramite un metodo
    private FieldOfView fieldOfView;

    public bool isMyTurn;
    public bool isSleeping;
    public bool hasSeenPlayer;

    private void Start()
    {
       
        fieldOfView = GetComponent<FieldOfView>();
    }

    void Update ()
    {
       hasSeenPlayer = fieldOfView.FindVisibleTarget();

	    if (player.isMyTurn)
        {
             if (!isSleeping && hasSeenPlayer)
             {
                KillThePlayer();
             }
        }
	}
	
	public void StartTurn()
    {
        SleepingManager();
        isMyTurn = true;

        if (isSleeping)
        {
            // feeback snore
            isMyTurn = false;
            turnManager.changeTurn();
        }
        else
        {
            // remove snore feedback
            if (hasSeenPlayer)
            {
                KillThePlayer();
                
            }
            else
            {
                isMyTurn = false;
                turnManager.changeTurn();
                
            }
        }
    }

    public void KillThePlayer()
    {
        // animazione sparo 
        // bool player morto
        Debug.Log("Pew pew pew. Git Gud sei morto casual");
    }

    private void SleepingManager()
    {
        if (isSleeping)
        {
            isSleeping = false;
        }
        else
        {
            isSleeping = true;
        }
    }
	
}
