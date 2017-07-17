using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using Pathfinding.Util;

public class Bracciante : MonoBehaviour {

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
    private Transform direction;

    private Vector3[] vectorNodesArray;
    private int nodesCounter = 0;
    private int waypointsCounter = 0;
    public bool hasSeenPlayer;
    public bool hasHeardPlayer;
    private bool hasToSetPlayerPath = true;

    private int numberOfPathNodes;
    private float timer = 0;
    public float turnRate = 2;
    private int whichAngle = 0;
    private int angleIndex = 0;
    Quaternion[] nextTurnAngle = new Quaternion[3];




    public Transform playerTransform;

    // Use this for initialization
    void Start () {

        grid = AstarPath.active.data.gridGraph;
        seeker = GetComponent<Seeker>();
        aiLerp = GetComponent<AILerp>();
        direction = GetComponentInChildren<Transform>();
        fov = GetComponent<FieldOfView>();
        actionsAmount = maxActionsAmount;
    }

    public void StartTurn()                   //chiamato all'inizio del mio turno
    {
        actionsAmount = maxActionsAmount;
        nodesCounter = 0;
        isMyTurn = true;
        fov.FindVisibleTarget();
        Vector3 target = new Vector3();
        if (hasSeenPlayer)                           //se ho visto il player
        {
            target = playerTransform.position;            //setto il target alla posizione del player
            hasToSetPlayerPath = false;
        }
        else if (hasHeardPlayer)                    //se ho sentito o visto il player ma quel nigga se l'è svignata
        {
            target = (Vector3)grid.GetNearest(new Vector3(fov.lastPlayerSeenPoint.x,fov.lastPlayerSeenPoint.y,0)).node.position;              //settare il target nell'ultimo punto in cui il player è passato sul range uditivo di sto negro
            hasHeardPlayer = true;
        }
        else                                       //se non ho visto ne sentito il player
        {
            target = waypoints[waypointsCounter].position;
        }
        GetPathNodes(target);                            //prendo tutti i nodi del path verso il target scelto
        GoToNode(vectorNodesArray[nodesCounter]);              //vado al primo nodo del path
        nodesCounter+=1;
    }

    public void TargetReached()     //chiamato quando ho raggiunto il nodo
    {
        actionsAmount -= 1;

        if(hasSeenPlayer)       //se ho visto il player
        {
            if(hasToSetPlayerPath)      //se devo ancora settare il path verso il player
            {
                if(actionsAmount>0)         //se ho ancora azioni disponibili
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
            else if(!hasToSetPlayerPath)        //se ho già settato il path verso il player
            {
                if (actionsAmount > 0)          //se ho ancora azioni disponibili
                {
                    if(nodesCounter<numberOfPathNodes-1)        //se non sono ancora arrivato al penultimo nodo del path verso il player
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
        }else if (hasHeardPlayer)     //se ho sentito il player
        {
            if(actionsAmount>0)
            {
                if(nodesCounter<numberOfPathNodes-1)         //se non sono ancora arrivato al penultimo nodo del path verso dove ho sentito il player
                {
                    GoToNode(vectorNodesArray[nodesCounter]);       //vado al nodo successivo
                    nodesCounter++;
                }
                else                //se sono arrivato al penultimo nodo del path
                {
                    //Sto negro fa un controllo di tot gradi in giro per cercare il player
                    nextTurnAngle[0].eulerAngles = transform.rotation.eulerAngles + new Vector3(0, 0, 90);
                    nextTurnAngle[1].eulerAngles = transform.rotation.eulerAngles + new Vector3(0, 0, -180);
                    nextTurnAngle[2].eulerAngles = transform.rotation.eulerAngles + new Vector3(0, 0, 90);
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
            if(nodesCounter<numberOfPathNodes)    //se non ho raggiunto ancora l'ultimo nodo del path verso il waypoint
            {
                if(actionsAmount>0)  //se ho ancora azioni disponibili
                {
                    Debug.Log(nodesCounter+" "+numberOfPathNodes);

                    GoToNode(vectorNodesArray[nodesCounter]);
                    nodesCounter++;
                }
                else                   //se non ho più azioni disponibili finisco il mio porco dio di turno
                {
                    isMyTurn = false;
                    turnManager.changeTurn();
                }
            }else
            {
                ChooseNextWaypoint();      //scelgo il prossimo waypoint

                if(actionsAmount>0)
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

    IEnumerator LookAround()
    {
        timer = 0;
        turnRate = 2;
        while (whichAngle < 3)
        {
            timer += Time.deltaTime;
            Quaternion.Lerp(transform.rotation, nextTurnAngle[whichAngle],timer/turnRate);
            if (timer > turnRate)
            {
                whichAngle++;
                timer = 0;
            }
            yield return null;
        }
        yield return new WaitForSeconds(2);
        TargetReached();

    }


    void GetPathNodes(Vector3 target)       //estrapola i nodi del path verso il target in un array, eccetto il nodo della nostra posizione (vectorNodesArray)
    {
        Path p = seeker.StartPath(transform.position, target);
        p.BlockUntilCalculated();
        List<Vector3> pathNodesList =  p.vectorPath;
        numberOfPathNodes = pathNodesList.Count-1;

        vectorNodesArray = new Vector3[20];

        for (int j=0; j<pathNodesList.Count-1;j++)
        {

            vectorNodesArray[j] = pathNodesList[j+1];
        }     
    }

    void ChooseNextWaypoint()
    {
        if (waypointsCounter < waypoints.Length-1)        //scelgo quale waypoint prendere per il path da settare
        {
            waypointsCounter++;
        }
        else
        {
            waypointsCounter = 0 ;
        }


    }


    public void GoToNode(Vector3 targetPos)     //vai al nodo scelto
    {
        Path p = seeker.StartPath(transform.position, targetPos);
        p.BlockUntilCalculated();
    } 

    void LateUpdate()
    {
        sprite.position = transform.position;
    }

}
