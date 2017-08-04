using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagazzoMucca : MonoBehaviour {

    private TurnManager turnManager;
    private PlayerActions player; //da mettere se si vuole gestire la morte del player tramite un metodo
    private FieldOfView fieldOfView;

    public bool isMyTurn;
    public bool isSleeping;
    public bool hasSeenPlayer;
    private bool imDead;
    private float waitTimer = 1f;
    private float sleepingView;
    private float originalViewAngle;
    public Transform enemyRear;
    public Transform spriteTransform;
    public Animator anim;
    private SpriteRenderer sprite;

    private bool canKillPlayer = true;

    private void Start()
    {
        turnManager = GameObject.Find("TurnManager").GetComponent<TurnManager>();
        player = GameObject.Find("Player").GetComponent<PlayerActions>();
        fieldOfView = GetComponent<FieldOfView>();
        originalViewAngle = fieldOfView.viewAngle;
     //   sprite = anim.GetComponent<SpriteRenderer>();
    }

    float AngleToPositive(float angle)
    {
        if (angle > 359)
        {
            return angle - 360;
        }
        else if (angle < 0)
        {
            return 360 - angle;
        }
        else return angle;
    }

    void Update ()
    {
    /*    if (AngleToPositive(transform.rotation.eulerAngles.z) > 45 && AngleToPositive(transform.rotation.eulerAngles.z) < 225)
        {
            sprite.flipX = true;
        }
        else
        {
            sprite.flipX = false;
        }
        anim.SetBool("isSleeping", isSleeping);
        anim.SetFloat("angle", transform.rotation.eulerAngles.z); */
        hasSeenPlayer = fieldOfView.FindVisibleTarget();

	    if (player.isMyTurn && !imDead)
        {
             if (!isSleeping && hasSeenPlayer)
             {
                KillThePlayer();
             }
        }
        if ( isSleeping)
        {
            fieldOfView.viewAngle = 0;
        }
        else
        {
            fieldOfView.viewAngle = originalViewAngle;
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
            StartCoroutine("ChangeTurnDelay", waitTimer);
            return;
        }

       // isMyTurn = true;
        SleepingManager();
        

        if (isSleeping)
        {
            // feeback snore
           isMyTurn = false;
           StartCoroutine("ChangeTurnDelay", waitTimer);
            
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
                StartCoroutine("ChangeTurnDelay", waitTimer);
                

            }
        }
    }

    public void KillThePlayer()
    {
        if (canKillPlayer)
        {
            // animazione sparo 
            // bool player morto
            player.Die();
            Debug.Log("Pew pew pew. Git Gud sei morto casual");
            canKillPlayer = false;
        }       
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
        this.transform.position = new Vector3(100, 100, 100);
        Debug.Log(this.transform.position);
        imDead = true;
        Debug.Log("Sono morto" + this.gameObject.tag);
        //anim dead
    }

    private void LateUpdate()
    {
        enemyRear.position = transform.position - transform.up;
       // spriteTransform.position = transform.position;
    }
}
