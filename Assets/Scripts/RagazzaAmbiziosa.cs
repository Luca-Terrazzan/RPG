using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagazzaAmbiziosa : MonoBehaviour {

    private GridGraph grid;
    private Seeker seeker;
    private AILerp aiLerp;
    public TurnManager turnManager;
    private FieldOfView fov;
    private bool attackTrigger = true;

    public int actionsAmount;
    public int maxActionsAmount;
    public bool isMyTurn;
    public Transform[] waypoints;
    public Transform spriteTransform;
    public Transform soundSprite;
    private Transform direction;

    private Vector3[] vectorNodesArray;
    private int nodesCounter = 0;
    private int waypointsCounter = 0;
    public bool hasSeenPlayer;
    public bool hasHeardPlayer;
    private bool hasToSetPlayerPath = true;
    private Vector3 lastPositionHeard;
    public bool imDead;

    private int numberOfPathNodes;
    Quaternion[] nextTurnAngle = new Quaternion[3];

    public Transform playerTransform;
    public Transform enemyRear;

    public GameObject bomb;
    public GameObject explosion;

    private bool canThrowBomb = true;

    public Animator anim;
    public SpriteRenderer sprite;

    private AudioSource prostititutaSoundPlayer;
    public AudioClip prostitutaAttackSound;

    // Use this for initialization
    void Start()
    {
        turnManager = GameObject.Find("TurnManager").GetComponent<TurnManager>();
        playerTransform = GameObject.Find("Player").transform;
        prostititutaSoundPlayer = GetComponent<AudioSource>();
        grid = AstarPath.active.data.gridGraph;
        seeker = GetComponent<Seeker>();
        aiLerp = GetComponent<AILerp>();
        direction = GetComponentInChildren<Transform>();
        fov = GetComponent<FieldOfView>();
        actionsAmount = maxActionsAmount;
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


    private void Update()
    {
        
        if (AngleToPositive(transform.rotation.eulerAngles.z) > 45 && AngleToPositive(transform.rotation.eulerAngles.z) < 225)
        {
            sprite.flipX = true;
        }
        else
        {
            sprite.flipX = false;
        }

        anim.SetBool("isMoving", aiLerp.canMove);
        anim.SetFloat("angle", transform.rotation.eulerAngles.z);

        // Debug.Log(transform.up.ToString());
        if (isMyTurn)
        {
            // this.gameObject.layer = 8;
            if (fov.FindVisibleTarget())
            {
                if(canThrowBomb)
                {
                    ThrowBomb(playerTransform.position);
                    canThrowBomb = false;
                    aiLerp.canMove = false;
                    playerTransform.GetComponent<AILerp>().canMove = false;
                }
            }
        }
        else
        {
            if (fov.FindVisibleTarget())
            {
                if (canThrowBomb)
                {
                    ThrowBomb(playerTransform.position);
                    canThrowBomb = false;
                    aiLerp.canMove = false;
                    playerTransform.GetComponent<AILerp>().canMove = false;
                }
            }
            else if (fov.AmIHearingPlayer())
            {
                hasHeardPlayer = true;
                lastPositionHeard = new Vector3(playerTransform.position.x, playerTransform.position.y, 0);
            }
            else
            {
                this.gameObject.layer = 8;
            }
        }
    }

    IEnumerator ChangeTurnDelay(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        turnManager.changeTurn();

    }

    public void StartTurn()                   //chiamato all'inizio del mio turno
    {
        if (imDead)
        {
            StartCoroutine("ChangeTurnDelay", 1);
            return;

        }
        isMyTurn = true;
        this.gameObject.layer = 12;
        AstarPath.active.Scan();
        actionsAmount = maxActionsAmount;
        nodesCounter = 0;
        hasSeenPlayer = false;

        Vector3 target = new Vector3();
 
        if (hasHeardPlayer)                    //se ho sentito il player ma quel nigga se l'è svignata
        {
            hasHeardPlayer = false;
            Path p = seeker.StartPath(transform.position,lastPositionHeard);
            p.BlockUntilCalculated();
            lastPositionHeard = p.vectorPath[p.vectorPath.Count - 1];
            ThrowBomb((Vector3)grid.GetNearest(lastPositionHeard).node.position);
        }
        else
        {
            aiLerp.canMove = true;
        }
        
        target = waypoints[waypointsCounter].position;
        GetPathNodes(target);                            //prendo tutti i nodi del path verso il target scelto
        GoToNode(vectorNodesArray[nodesCounter]);              //vado al primo nodo del path
        nodesCounter += 1;
    }

    public void EndTurn()
    {
        this.gameObject.layer = 8;
        AstarPath.active.Scan();
        isMyTurn = false;
        aiLerp.canMove = false;
        turnManager.changeTurn();
    }

    public void TargetReached()     //chiamato quando ho raggiunto il nodo
    {
        actionsAmount -= 1;

        if (nodesCounter < numberOfPathNodes)    //se non ho raggiunto ancora l'ultimo nodo del path verso il waypoint
        {
            if (actionsAmount > 0)  //se ho ancora azioni disponibili
            {
                Debug.Log(nodesCounter + " " + numberOfPathNodes);

                GoToNode(vectorNodesArray[nodesCounter]);
                nodesCounter++;
            }
            else                   //se non ho più azioni disponibili finisco il mio porco dio di turno
            {
                EndTurn();
            }
        }
        else
        {
            ChooseNextWaypoint();      //scelgo il prossimo waypoint

            if (actionsAmount > 0)
            {
                Debug.Log("Arrivo al waypoint: " + waypoints[waypointsCounter].position.ToString());
                GetPathNodes(waypoints[waypointsCounter].position);         //estrapolo i nodi del path verso il player
                nodesCounter = 0;
                GoToNode(vectorNodesArray[nodesCounter]);                  //vado al primo nodo del path
                nodesCounter++;
            }
            else        //se non ho più azioni disponibili FINISCO IL MIO DIO CANE DI TURNO
            {
                EndTurn();
            }
        }
        
    }


    
    public void ThrowBomb(Vector3 position)
    {
        Debug.Log("Lancio una bomba in posizione: " + position.ToString());
        StartCoroutine(BombLerp(position));
    }

    IEnumerator BombLerp(Vector3 position)
    {

        anim.SetTrigger("attack");
        aiLerp.canMove = false;
        yield return new WaitForSeconds(0.5f);
        Quaternion bombRot = new Quaternion();
        bombRot.eulerAngles = new Vector3(-35, -45, 60);
        GameObject b = Instantiate(bomb, transform.position, bombRot);
        float timer = 0;
        float timeToLerp = 1;
        while (timer < timeToLerp)
        {
            timer += Time.deltaTime;
            b.transform.position = Vector3.Lerp(b.transform.position, position, timer / timeToLerp);
            yield return null;
        }
        GameObject expl = Instantiate(explosion, b.transform.position, Quaternion.identity);
        Destroy(b);
        Destroy(expl, 0.9f);
        aiLerp.canMove = true;
        prostititutaSoundPlayer.clip = prostitutaAttackSound;
        prostititutaSoundPlayer.Play();

        yield return null;
    }


    void GetPathNodes(Vector3 target)       //estrapola i nodi del path verso il target in un array, eccetto il nodo della nostra posizione (vectorNodesArray)
    {
        Path p = seeker.StartPath(transform.position, target);
        p.BlockUntilCalculated();
        List<Vector3> pathNodesList = p.vectorPath;
        numberOfPathNodes = pathNodesList.Count - 1;

        vectorNodesArray = new Vector3[20];

        for (int j = 0; j < pathNodesList.Count - 1; j++)
        {

            vectorNodesArray[j] = pathNodesList[j + 1];
        }
    }

    void ChooseNextWaypoint()
    {
        if (waypointsCounter < waypoints.Length - 1)        //scelgo quale waypoint prendere per il path da settare
        {
            waypointsCounter++;
        }
        else
        {
            waypointsCounter = 0;
        }


    }

    public void Die()
    {
        aiLerp.canMove = false;
        imDead = true;
        anim.SetTrigger("Die");
        sprite.transform.localScale *= 4;
        StartCoroutine(DieDelay());
    }

    IEnumerator DieDelay()
    {
        yield return new WaitForSeconds(0.5f);
        this.transform.position = new Vector3(100, 100, 100);
    }
    public void GoToNode(Vector3 targetPos)     //vai al nodo scelto
    {
        Path p = seeker.StartPath(transform.position, targetPos);
        p.BlockUntilCalculated();
    }

    void LateUpdate()
    {
        spriteTransform.position = transform.position;
        soundSprite.position = new Vector3(transform.position.x, transform.position.y, 0);
        enemyRear.position = transform.position - transform.up;
    }
}
