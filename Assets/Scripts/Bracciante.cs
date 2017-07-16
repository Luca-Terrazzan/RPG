using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using Pathfinding.Util;

public class Bracciante : MonoBehaviour {

    private GridGraph grid;
    private Seeker seeker;
    private AILerp aiLerp;

    public int actionsAmount;
    public bool isMyTurn;
    public bool isPatroling=true;
    public Transform[] waypoints;
    public Transform sprite;
    public GameObject visionSprite;
    private Transform direction;

    public bool hasToSetPath=true;
    private Vector3[] vectorNodesArray;
    private int nodesCounter = 0;
    [SerializeField] private int waypointsCounter = -1;
    public bool hasSeenPlayer;
    private int numberOfPathNodes;

    
       

    public Transform playerTransform;

    // Use this for initialization
    void Start () {

        grid = AstarPath.active.data.gridGraph;
        seeker = GetComponent<Seeker>();
        aiLerp = GetComponent<AILerp>();
        direction = GetComponentInChildren<Transform>();



    }

    // Update is called once per frame
    void Update () {

        
        if (isMyTurn)
        {
            if (isPatroling)       //se sto pattugliando
            {
                if (hasToSetPath)       //se devo settare un path
                {
                    ChooseNextWaypoint();          //scelgo qual è il prossimo waypoint   
                    GetPathNodes(waypoints[waypointsCounter].position);    //prendo tutti i nodi del path verso il waypoint scelto
                    GoToNode(vectorNodesArray[nodesCounter]);              //vado al primo nodo del path
                    nodesCounter++;                                       
                    hasToSetPath = false;
                }
            }
            else if (!isPatroling)            //se ho visto il player
            {
                if (hasToSetPath)       //se devo settare un path
                {
                    GetPathNodes(playerTransform.position);         //prendo tutti i nodi del path verso il player
                    GoToNode(vectorNodesArray[nodesCounter]);       //vado al primo nodo del path
                    nodesCounter++;
                    hasToSetPath = false;
                }
            }

        }
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
        if (waypointsCounter < waypoints.Length - 1)        //scelgo quale waypoint prendere per il path da settare
        {
            waypointsCounter++;
        }
        else
        {
            waypointsCounter = 0 ;
        }


    }



    public void TargetReached()     //chiamato quando ho raggiunto il nodo
    {
        actionsAmount -= 1;
        Debug.Log(nodesCounter);
        if (actionsAmount > 0)      //se ho ancora azioni disponibili
        {
            if (isPatroling)        //se sto pattugliando
            {
                if (hasSeenPlayer)  //se ho visto il player smetto di pattugliare
                {
                    hasToSetPath = true;
                    isPatroling = false;
                    nodesCounter = 0;
                    hasSeenPlayer = false;
                }
                else                //se non ho visto il player
                {
                    if (nodesCounter < numberOfPathNodes)       //se i nodi del path non sono finiti, vai al prossimo nodo
                    {
                        GoToNode(vectorNodesArray[nodesCounter]);
                        nodesCounter++;
                    }
                    else                                        //se i nodi del path sono finiti, devo settare un altro path
                    {
                        hasToSetPath = true;
                        nodesCounter = 0;
                    }
                }
            }
            else if(!isPatroling)           //se ho visto il player
            {
                if (nodesCounter < numberOfPathNodes - 1)       //se i nodi del path non sono finiti, vai al prossimo nodo
                {
                    GoToNode(vectorNodesArray[nodesCounter]);
                    nodesCounter++;
                }
                else                              //se i nodi del path sono finiti mi trovo nella casella adiacente al player quindi lo killo quel bastardo e gli dico git gud casual
                {

                    //uccidi il player
                }                
            }            
        }
        else
        {
            isMyTurn = false;
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
