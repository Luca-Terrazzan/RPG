﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PickUpItems : MonoBehaviour {

    private PlayerActions player;


	// Use this for initialization
	void Start ()
    {
        player = GetComponent<PlayerActions>();


    }
    // Metodo a caso per la collisione con gli oggetti, amazing.
    private void OnTriggerEnter (Collider collision)
    {
       
       if (collision.gameObject.tag == "Key")
        {
            player.hasKey = true;
            Destroy(collision.gameObject);
            Debug.Log("Hai preso la chiave.Amaze.");
        }
        if (collision.gameObject.tag == "ExitDoor")
         {
           
            if (player.hasKey)
            {
                Debug.Log("Puoi uscire.Wow.");
                SceneManager.LoadScene("GhostTown");

                // go to main scene and set hasKey to false
            }
            else
            {
                Debug.Log("Noob vai a prendere la chiave");
            }
         }

        if (collision.gameObject.tag == "OggettoRaccoglibileAcaso")
        {
            Debug.Log("Bravo hai preso un oggetto inutile.Amaze.");
            // store object somewhere 
        }

       if(collision.gameObject.tag == "EnemyFront")
        {
            player.GetComponent<AILerp>().canMove = false;
            collision.GetComponentInParent<AILerp>().canMove = false;
            collision.GetComponentInParent<Bracciante>().KillPlayer();
        }
        if (collision.gameObject.tag == "ArmadioFront")
        {
            player.canHide = true;
            player.armadioFrontTransform = collision.transform;
        }
        if (collision.gameObject.CompareTag("EnterDoor"))
        {
            Debug.Log("Passa alla scena successiva");
           
        }

        if (collision.tag == "InnerExplosion")
        {
            player.Die();
        }
        if (collision.tag == "OuterExplosion")
        {
            player.playerActions -= 5;
        }
        
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "ArmadioFront")
        {
            player.canHide = false;
        }
        if (other.tag == "LowBox")
        {
            player.lowInvisible = false;
        }
    }

    public void OnTriggerStay(Collider other)
    {
        
        if (other.gameObject.tag == "EnemyRear")
        {
            float distToEnemy = Vector3.Distance(player.transform.position,other.transform.position);
            
                RaycastHit hit;
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
                {
                if (hit.collider != null)
                {

                    if ((hit.collider.gameObject.tag == "Bracciante" || hit.collider.tag == "CowBoy" || hit.collider.tag == "Prostituta"))
                    {
                        if (player.playerActions >= 6)
                        {
                            player.fakePlayerActions = 6;

                            if (Input.GetMouseButtonUp(0))
                            {
                                player.BackStabEnemy(hit.collider.gameObject); // git gud

                            }
                        }
                    }
                }

            }           
        }

        if (other.tag == "LowBox")
        {
            player.lowInvisible = true;
        }






    }


}
