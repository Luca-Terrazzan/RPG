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
    public bool isPatroling;
    public Transform[] waypoints;
    public Transform sprite;
    public GameObject visionSprite;
    private Transform direction;

    public bool settingNodePath;
    public bool hasToSetPath=false;
    private List<Vector3> pathNodesList;
    private Vector3[] vectorNodesArray;
    private int nodesCounter = 0;
    private int waypointsCounter = -1;
    public bool stopPatroling;
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
            if (actionsAmount <= 0)
            {
                isMyTurn = false;
                
            }

            if (isPatroling)
            {

                if (hasToSetPath)
                {
                    if (waypointsCounter < waypoints.Length - 1)
                    {
                        waypointsCounter++;
                    }
                    else
                    {
                        waypointsCounter = 0;
                    }
                    GetWayPointPath(waypoints[waypointsCounter].position);
                    settingNodePath = true;
                    hasToSetPath = false;
                }

                else if(settingNodePath)
                {
                    if(vectorNodesArray[nodesCounter]!=Vector3.zero)
                    {
                        Path p = seeker.StartPath(transform.position, vectorNodesArray[nodesCounter]);
                        p.BlockUntilCalculated();
                        nodesCounter++;
                        settingNodePath = false;
                        aiLerp.canMove = true;
                    }
                    else
                    {
                        nodesCounter = 0;
                        settingNodePath = false;
                        hasToSetPath = true;
                    }
                   
                }
   

            }
            else
            {
                if (hasToSetPath)
                {
                    GetWayPointPath(playerTransform.position);
                    settingNodePath = true;
                    hasToSetPath = false;
                }
                else if (settingNodePath)
                {
                    if (nodesCounter < numberOfPathNodes-1)
                    {
                        Path p = seeker.StartPath(transform.position, vectorNodesArray[nodesCounter]);
                        p.BlockUntilCalculated();
                        nodesCounter++;
                        settingNodePath = false;
                        aiLerp.canMove = true;
                    }
                    else
                    {
                        nodesCounter = 0;
                        settingNodePath = false;
                        hasToSetPath = true;
                    }

                }

            }

        }
    }
  


    void GetWayPointPath(Vector3 target)
    {
        Path p = seeker.StartPath(transform.position, target);
        

        p.BlockUntilCalculated();
        pathNodesList = p.vectorPath;
        numberOfPathNodes = pathNodesList.Count;

        vectorNodesArray = new Vector3[20];

        for (int j=0; j<pathNodesList.Count;j++)
        {
            vectorNodesArray[j] = pathNodesList[j];
        }     
    }

    public void TargetReached()
    {
        actionsAmount -= 1;
        // aiLerp.canMove = false; <-- utile
        if (stopPatroling)
        {
            isPatroling = false;
            aiLerp.canMove = false;
            hasToSetPath = true;
        }
        else
        {
            settingNodePath = true;
        }

    }

    void LateUpdate()
    {
        sprite.position = transform.position;
    }

}
