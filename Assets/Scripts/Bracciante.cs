﻿using System.Collections;
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

    private Vector3[] vectorNodesArray;
    private int nodesCounter = 0;
    private int waypointsCounter = 0;
    public bool hasSeenPlayer;
    public bool hasHeardPlayer;
    private bool hasToSetPlayerPath = true;

    private int numberOfPathNodes;

    
       

    public Transform playerTransform;

    // Use this for initialization
    void Start () {

        grid = AstarPath.active.data.gridGraph;
        seeker = GetComponent<Seeker>();
        aiLerp = GetComponent<AILerp>();
        direction = GetComponentInChildren<Transform>();

    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            StartTurn();
        }
    }

    public void StartTurn()                   //chiamato all'inizio del mio turno
    {
        nodesCounter = 0;
        isMyTurn = true;
        Vector3 target = new Vector3();
        if (hasSeenPlayer)                           //se ho visto il player
        {
            target = playerTransform.position;            //setto il target alla posizione del player
            hasToSetPlayerPath = false;
        }
        else if (hasHeardPlayer)                    //se ho sentito il player
        {
                            //settare il target nell'ultimo punto in cui il player è passato sul range uditivo di sto negro
        }
        else                                       //se non ho visto ne sentito il player
        {
            target = waypoints[waypointsCounter].position;
        }
        Debug.Log("Inizio turno vado a waypoint: " + waypoints[waypointsCounter].position.ToString());
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
                    hasHeardPlayer = false;
                }
            }
            else       //se non ho azioni finisco il mio cazzo di turno
            {
                isMyTurn = false;
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
