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

    public int actionsAmount;
    public int maxActionsAmount;
    public bool isMyTurn;
    public Transform[] waypoints;
    public Transform sprite;
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


    // Use this for initialization
    void Start()
    {
        turnManager = GameObject.Find("TurnManager").GetComponent<TurnManager>();
        playerTransform = GameObject.Find("Player").transform;

        grid = AstarPath.active.data.gridGraph;
        seeker = GetComponent<Seeker>();
        aiLerp = GetComponent<AILerp>();
        direction = GetComponentInChildren<Transform>();
        fov = GetComponent<FieldOfView>();
        actionsAmount = maxActionsAmount;
    }

    private void Update()
    {

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
        this.gameObject.layer = 12;
        AstarPath.active.Scan();
        actionsAmount = maxActionsAmount;
        nodesCounter = 0;
        hasSeenPlayer = false;
        isMyTurn = true;

        Vector3 target = new Vector3();
 
        if (hasHeardPlayer)                    //se ho sentito il player ma quel nigga se l'è svignata
        {
            hasHeardPlayer = false;
            Path p = seeker.StartPath(transform.position,lastPositionHeard);
            p.BlockUntilCalculated();
            lastPositionHeard = p.vectorPath[p.vectorPath.Count - 1];
            ThrowBomb((Vector3)grid.GetNearest(lastPositionHeard).node.position);
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
        turnManager.changeTurn();
        hasSeenPlayer = false;
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
                isMyTurn = false;
                turnManager.changeTurn();
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
                isMyTurn = false;
                turnManager.changeTurn();
            }
        }
        
    }

    
    public void ThrowBomb(Vector3 position)
    {
        Debug.Log("Lancio una bomba in posizione: " + position.ToString());
        GameObject b = Instantiate(bomb, transform.position, Quaternion.identity);
        StartCoroutine(BombLerp(b,position));
    }

    IEnumerator BombLerp(GameObject b,Vector3 position)
    {
        float timer = 0;
        float timeToLerp = 1;
        while (timer < timeToLerp)
        {
            timer += Time.deltaTime;
            b.transform.position = Vector3.Lerp(b.transform.position, position,timer/timeToLerp);
            yield return null;
        }
        GameObject expl = Instantiate(explosion, b.transform.position,Quaternion.identity);
        Destroy(b);
        Destroy(expl, 1);
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
        this.transform.position = new Vector3(100, 100, 100);
        Debug.Log(this.transform.position);
        imDead = true;
        Debug.Log("Sono morto" + this.gameObject.tag);

    }

    public void GoToNode(Vector3 targetPos)     //vai al nodo scelto
    {
        Path p = seeker.StartPath(transform.position, targetPos);
        p.BlockUntilCalculated();
    }

    void LateUpdate()
    {
        sprite.position = transform.position;
        soundSprite.position = new Vector3(transform.position.x, transform.position.y, 0);
        enemyRear.position = transform.position - transform.up;
    }
}
