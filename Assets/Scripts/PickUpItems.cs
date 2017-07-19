using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        else if (collision.gameObject.tag == "ExitDoor")
         {
            if (player.hasKey)
            {
                Debug.Log("Puoi uscire.Wow.");
                // go to main scene and set hasKey to false
            }
            else
            {
                Debug.Log("Noob vai a prendere la chiave");
            }
         }

       else if (collision.gameObject.tag == "OggettoRaccoglibileAcaso")
        {
            Debug.Log("Bravo hai preso un oggetto inutile.Amaze.");
            // store object somewhere 
        }

       
    }

    public void OnTriggerStay(Collider other)
    {
        
        if (other.gameObject.tag == "EnemyRear")
        {
            float distToEnemy = Vector3.Distance(player.transform.position,other.transform.position);
            if(Input.GetMouseButtonUp(0))
            {
                RaycastHit hit;
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
                {
                    if (hit.collider != null)
                    {
                        
                       if ((hit.collider.gameObject.tag == "Bracciante" || hit.collider.tag == "CowBoy" || hit.collider.tag == "Puttana"))
                       {
                           
                         player.BackStabEnemy(hit.collider.gameObject); // git gud
                       }
                    }
                }

            }           
        }
    }


}
