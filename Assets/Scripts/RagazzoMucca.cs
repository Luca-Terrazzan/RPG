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
    private bool imDead;

    private void Start()
    {
       
        fieldOfView = GetComponent<FieldOfView>();
    }

    void Update ()
    {
      

        hasSeenPlayer = fieldOfView.FindVisibleTarget();

	    if (player.isMyTurn && !imDead)
        {
             if (!isSleeping && hasSeenPlayer)
             {
                KillThePlayer();
             }
        }
	}

    IEnumerator ChangeTurnDelay(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        turnManager.changeTurn();
    }
	
	public void StartTurn()
    {
        if (imDead)
        {
            turnManager.changeTurn();
            return;
        }

        isMyTurn = true;
        SleepingManager();
        

        if (isSleeping)
        {
            // feeback snore
            isMyTurn = false;
           StartCoroutine("ChangeTurnDelay",3.14f);
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
                StartCoroutine("ChangeTurnDelay", 3.14f);

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

    public void Die()
    {
        imDead = true;
        //anim dead
    }
}
