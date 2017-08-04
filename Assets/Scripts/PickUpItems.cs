using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PickUpItems : MonoBehaviour {

    private PlayerActions player;
    private Collider myCol;
    private AudioSource attackSound;
    public LayerMask lowBoxMask;
    private Text fuocoFatuo;


	// Use this for initialization
	void Start ()
    {
        player = GetComponent<PlayerActions>();
        myCol = GetComponent<Collider>();
        fuocoFatuo = GameObject.Find("FuochiFatui").GetComponent<Text>();
        attackSound = GetComponent<AudioSource>();

    }

    private void Update()
    {
        fuocoFatuo.text = (player.fuocoFatuo + FreeRoamingPos.staticFuochiFatui).ToString();
        if (Physics.BoxCast(transform.position,myCol.bounds.extents/2,Vector3.forward,Quaternion.identity,5,lowBoxMask))
        {
            player.lowInvisible = true;
        }
        else
        {
            player.lowInvisible = false;
        }
    }
    // Metodo a caso per la collisione con gli oggetti, amazing.
    private void OnTriggerEnter (Collider collision)
    {
       
       if (collision.gameObject.tag == "Key")
        {
            collision.gameObject.GetComponent<AudioSource>().Play();
            player.fuocoFatuo++;
            player.hasKey = true;
            // Destroy(collision.gameObject);
            collision.gameObject.GetComponent<BoxCollider>().enabled = false;
            collision.gameObject.GetComponent<SpriteRenderer>().enabled = false;
            Debug.Log("Hai preso la chiave.Amaze.");
        }
        if (collision.gameObject.tag == "ExitDoor")
         {
           
            if (player.hasKey)
            {
                if (SceneManager.GetActiveScene().name == "Bordello") 
                {
                    SceneManager.LoadScene("Saloon");
                }
                else
                {
                    SceneManager.LoadScene("GhostTown");
                }

               
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
            if (collision.transform.parent.gameObject.tag == "Bracciante")
            {
                player.GetComponent<AILerp>().canMove = false;
                collision.GetComponentInParent<AILerp>().canMove = false;
                collision.GetComponentInParent<Bracciante>().KillPlayer();
            }
            else if (collision.transform.parent.gameObject.tag == "CowBoy")
            {
                player.GetComponent<AILerp>().canMove = false;
                collision.GetComponentInParent<RagazzoMucca>().KillThePlayer();
            }
            else if (collision.gameObject.transform.parent.tag == "Boss")
            {
                player.GetComponent<AILerp>().canMove = false;
                collision.GetComponentInParent<BossIA>().KillPlayer();
            }
            
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
                                player.BackStabEnemy(hit.collider.gameObject);
                                attackSound.Play();

                            }
                        }
                    }
                }

            }           
        }

       



    }


}
