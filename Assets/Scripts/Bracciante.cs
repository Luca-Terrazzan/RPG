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
    private int waypointsCounter = 0;

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
            if (isPatroling)
            {

                if (hasToSetPath)
                {
                    GetWayPointPath();
                    settingNodePath = true;
                    hasToSetPath = false;
                }

                else if(settingNodePath)
                {
                    if(vectorNodesArray[nodesCounter]!=Vector3.zero)
                    {
                        CreateVisionSprites();
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
                Debug.Log("direction position: " + direction.position.ToString());
                


            }

        }	
    }

    void CreateVisionSprites()
    {
        int i = 2;
        GameObject clone = Instantiate(visionSprite, transform.position + transform.up, Quaternion.identity);
    }

    void GetWayPointPath()
    {
        Path p = seeker.StartPath(transform.position, waypoints[waypointsCounter].position);
        if (waypointsCounter < waypoints.Length-1)
        {
            waypointsCounter++;
        }else
        {
            waypointsCounter = 0;
        }

        p.BlockUntilCalculated();
        pathNodesList = p.vectorPath;

        vectorNodesArray = new Vector3[15];

        for (int j=0; j<pathNodesList.Count;j++)
        {
            vectorNodesArray[j] = pathNodesList[j];
        }     
    }

    public void TargetReached()
    {
       // aiLerp.canMove = false;
        settingNodePath = true;
    }

    void LateUpdate()
    {
        sprite.position = transform.position;
    }

}
