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

    public bool hasToCalculateLength;
    public bool hasToSetPath=false;
    private List<Vector3> pathNodesList;
    private int i = 0;

    // Use this for initialization
    void Start () {

        grid = AstarPath.active.data.gridGraph;
        seeker = GetComponent<Seeker>();
        aiLerp = GetComponent<AILerp>();

    }

    // Update is called once per frame
    void Update () {
        
        if (isMyTurn)
        {
            if (isPatroling)
            {
                if (hasToSetPath)
                {
                    Path p = seeker.StartPath(transform.position, waypoints[0].position);
                    p.BlockUntilCalculated();
                    pathNodesList = p.vectorPath;
                    pathNodesList.Add(new Vector3(-100, -200));
                    hasToSetPath = false;
                }

                if (Vector2.Distance(transform.position, pathNodesList[i+1]) < 0.1f && pathNodesList.Count>i)
                {
                    
                    i++;
                    actionsAmount -= 1;
                }

                if (actionsAmount<=0)
                {
                    aiLerp.canMove = false;
                    actionsAmount = 0;
                }
                

            }

        }	
    }


}
