using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using Pathfinding.Util;

public class Bracciante : MonoBehaviour
{

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
    public GameObject visionSprite;
    public Transform soundSprite;
    public Transform enemyRear;
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

    public GameObject boom;
    




    public Transform playerTransform;

    // Use this for initialization
    void Start()
    {

        grid = AstarPath.active.data.gridGraph;
        seeker = GetComponent<Seeker>();
        aiLerp = GetComponent<AILerp>();
        direction = GetComponentInChildren<Transform>();
        fov = GetComponent<FieldOfView>();
        actionsAmount = maxActionsAmount;
    }

    private void Update()
    {
        if (isMyTurn)
        {
            if (fov.FindVisibleTarget())
            {
                hasSeenPlayer = true;
            }
        }
        else
        {
            if (fov.FindVisibleTarget())
            {
                hasHeardPlayer = true;
                lastPositionHeard = new Vector3(playerTransform.position.x, playerTransform.position.y, 0);
            }
            else if (fov.AmIHearingPlayer())
            {
                hasHeardPlayer = true;
                lastPositionHeard = new Vector3(playerTransform.position.x, playerTransform.position.y, 0);
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
        actionsAmount = maxActionsAmount;
        nodesCounter = 0;
        hasSeenPlayer = false;
        isMyTurn = true;
        if (fov.FindVisibleTarget())
        {
            hasSeenPlayer = true;
        }
        Vector3 target = new Vector3();
        if (hasSeenPlayer)                           //se ho visto il player
        {
            target = playerTransform.position;            //setto il target alla posizione del player
            hasToSetPlayerPath = false;
        }
        else if (hasHeardPlayer)                    //se ho sentito o visto il player ma quel nigga se l'è svignata
        {
            target = (Vector3)grid.GetNearest(lastPositionHeard).node.position;    //setto come target l'ultimo punto in cui il player è passato
            hasHeardPlayer = true;
        }
        else                                       //se non ho visto ne sentito il player
        {
            target = waypoints[waypointsCounter].position;
        }
        GetPathNodes(target);                            //prendo tutti i nodi del path verso il target scelto
        GoToNode(vectorNodesArray[nodesCounter]);              //vado al primo nodo del path
        nodesCounter += 1;
    }

    public void TargetReached()     //chiamato quando ho raggiunto il nodo
    {
        actionsAmount -= 1;

        if (hasSeenPlayer)       //se ho visto il player
        {
            if (hasToSetPlayerPath)      //se devo ancora settare il path verso il player
            {
                if (actionsAmount > 0)         //se ho ancora azioni disponibili
                {
                    nodesCounter = 0;
                    GetPathNodes(playerTransform.position);         //estrapolo i nodi del path verso il player
                    GoToNode(vectorNodesArray[nodesCounter]);       //vado al primo nodo del path
                    nodesCounter++;
                    hasToSetPlayerPath = false;
                }
                else       //se non ho azioni finisco il mio turno
                {
                    isMyTurn = false;
                    turnManager.changeTurn();
                }

            }
            else if (!hasToSetPlayerPath)        //se ho già settato il path verso il player
            {
                if (actionsAmount > 0)          //se ho ancora azioni disponibili
                {
                    if (nodesCounter < numberOfPathNodes - 1)        //se non sono ancora arrivato al penultimo nodo del path verso il player
                    {
                        GoToNode(vectorNodesArray[nodesCounter]);       //vado al nodo successivo
                        nodesCounter++;
                    }
                    else
                    {
                        /////////////////////////////se i nodi del path sono finiti mi trovo nella casella adiacente al player quindi lo killo quel bastardo e gli dico git gud casual
                        Debug.Log("sei morto porcoddio");
                    }
                }
                else       //se non ho azioni finisco il mio cazzo di turno
                {
                    isMyTurn = false;
                    turnManager.changeTurn();
                }

            }
        }
        else if (hasHeardPlayer)     //se ho sentito il player
        {
            if (actionsAmount > 0)
            {
                if (nodesCounter < numberOfPathNodes - 1)         //se non sono ancora arrivato al penultimo nodo del path verso dove ho sentito il player
                {
                    GoToNode(vectorNodesArray[nodesCounter]);       //vado al nodo successivo
                    nodesCounter++;
                }
                else                //se sono arrivato al penultimo nodo del path
                {
                    //Sto negro fa un controllo di tot gradi in giro per cercare il player
                    aiLerp.enableRotation = false;
                    transform.up = new Vector3(vectorNodesArray[nodesCounter].x, vectorNodesArray[nodesCounter].y,0) - transform.position;
                    nextTurnAngle[0].eulerAngles = transform.rotation.eulerAngles + new Vector3(0, 0, 45);
                    nextTurnAngle[1].eulerAngles = transform.rotation.eulerAngles + new Vector3(0, 0, -45);
                    nextTurnAngle[2].eulerAngles = transform.rotation.eulerAngles + new Vector3(0, 0, 0);
                    StartCoroutine("LookAround");
                    hasHeardPlayer = false;
                }
            }
            else       //se non ho azioni finisco il mio cazzo di turno
            {
                isMyTurn = false;
                turnManager.changeTurn();
            }
        }
        else         //se non ho ne visto ne sentito il player
        {
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
                else        //se non ho più azioni disponibili FINISCO IL MIO DIO CANE DI TURNO MADONNA LADRA
                {
                    isMyTurn = false;
                    turnManager.changeTurn();
                }
            }
        }
    }

    IEnumerator LookAtTargetLocation()
    {
        yield return new WaitForSeconds(0.5f);
        aiLerp.enableRotation = true;
        turnManager.changeTurn();

    }
    IEnumerator LookAround()
    {
        float timer = 0;
        float turnRate = 2;
        int whichAngle = 0;
        Quaternion startingRotation = new Quaternion();
        startingRotation.eulerAngles = new Vector3(0,0,transform.rotation.eulerAngles.z);
        while (whichAngle<3)
        {
            if (fov.FindVisibleTarget())
            {
                break;
            }
            transform.rotation = Quaternion.Lerp(startingRotation, nextTurnAngle[whichAngle], timer/turnRate);
            timer += Time.deltaTime;
            if (timer > turnRate)
            {
                whichAngle++;
                timer = 0;
                yield return null;
                startingRotation.eulerAngles = new Vector3(0, 0, transform.rotation.eulerAngles.z);
            }
            yield return null;
        }
        if (hasSeenPlayer)
        {
            TargetReached();
        }
        else
        {
            GetPathNodes(waypoints[waypointsCounter].position);         //estrapolo i nodi del path verso il waypoint corrente
            nodesCounter = 0;
            GoToNode(vectorNodesArray[nodesCounter]);                  //vado al primo nodo del path
            nodesCounter++;
        }
        aiLerp.enableRotation = true;
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
        //imDead = true;
        Debug.Log("Sono morto" + this.gameObject.tag);
        //GameObject clone = Instantiate(boom, transform.position+new Vector3(0,0,-1f), boom.transform.rotation);
        Debug.Log("Ciao");
        
        Debug.Log("Sono una bestemmia bella immaginala");
        
        
       // Destroy(clone, 3);
        //anim dead
    }
    public void GoToNode(Vector3 targetPos)     //vai al nodo scelto
    {
        Path p = seeker.StartPath(transform.position, targetPos);
        p.BlockUntilCalculated();
    }

    void LateUpdate()
    {
        sprite.position = transform.position;
        soundSprite.position = new Vector3(transform.position.x,transform.position.y,0);
        enemyRear.position = transform.position - transform.up;
    }

}
