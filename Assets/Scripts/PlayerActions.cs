using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using Pathfinding.Util;

public class PlayerActions : MonoBehaviour{

    public int playerActions;
    public int playerActionsPerTurn;
    private int playerActionsTemp;
    public GameObject clickableSprite;
    public Camera cam;
    public TurnManager turnManager;
    public int attackActions;
    public LayerMask enemyMask;
    public LayerMask obstacle;


    private GridGraph grid;
    private Seeker seeker;
    private AILerp aiLerp;
    private List<GameObject> clickableSpriteList;

    public bool isMyTurn = false;
    private bool canCreateGrid = true;
    public bool isCrouched = false;
    public bool canBeHeard = false;
    public bool isFreeRoaming = false;
    public bool hasKey = false;

    private int numerOfPathNode;
    private Vector3[] nodeArray;
    private SpriteRenderer sprite;
    private int i;

    // Use this for initialization
    void Start () {

        clickableSpriteList = new List<GameObject>();
        grid = AstarPath.active.data.gridGraph;
        seeker = GetComponent<Seeker>();
        aiLerp = GetComponent<AILerp>();
        sprite = GetComponent<SpriteRenderer>();
        playerActions = playerActionsPerTurn;

    }
	
	// Update is called once per frame
	void Update ()
    {
        

        if (isMyTurn)
        {
            //non cancellare mi serve da finire

           /* RaycastHit colpito;
            if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out colpito))
            {
               
                Path p = seeker.StartPath(this.transform.position, colpito.transform.position);
                p.BlockUntilCalculated();
                List<Vector3> pathNodeList = p.vectorPath;
                Debug.Log(p.vectorPath.Count);

                nodeArray = new Vector3[15];

                for ( i = 0; i < pathNodeList.Count -1; i++)
                {
                    
                    Debug.DrawLine(pathNodeList[i], pathNodeList[i+1] );
                }

                }
                
                */
                

               
            
            
            if (canCreateGrid)
            {
                if (!isCrouched)
                {
                    CreateClickableGrid(playerActions);
                }
                else if (isCrouched)
                {
                    CreateClickableGrid((int)Mathf.Floor(playerActions / 2));
                }
                canCreateGrid = false;
            }

            if (Input.GetKeyDown(KeyCode.LeftControl)&&aiLerp.canMove==false)
            {
                
                DestroyClickableGrid();
                if (!isCrouched)
                {
                    CreateClickableGrid((int)Mathf.Floor(playerActions / 2));
                    isCrouched = true;
                    
                    sprite.color = Color.magenta;
                }
                else if (isCrouched)
                {
                    CreateClickableGrid(playerActions);
                    isCrouched = false;
                    
                    sprite.color = Color.red;
                }
            }

            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit))
                {

                    if (hit.collider != null)
                    {

                        if (hit.collider.tag == "ClickableSprite")
                        {
                            SubtractMovementActions(hit.collider.transform.position);
                            //aiLerp.target.position = hit.transform.position;
                            DestroyClickableGrid();
                            aiLerp.canMove = true;
                            if (isCrouched)
                            {
                                canBeHeard = false;
                            }
                            else
                            {
                                canBeHeard = true;
                            }
                        }

                        
                    }

                }
            }

            if (Input.GetKeyDown(KeyCode.KeypadEnter)&&!aiLerp.canMove)
            {
                isMyTurn = false;
                DestroyClickableGrid();
                turnManager.changeTurn();
                playerActions = playerActionsPerTurn;
                canCreateGrid = true;
                canBeHeard = false;
            }
        }

        if (isFreeRoaming)

        {
            DestroyClickableGrid();
           
            if (Input.GetMouseButtonDown(0))
            {
              //  Debug.Log("Ciao");

                RaycastHit hit;

                if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit))
                {
                    Debug.Log(hit.collider.gameObject.tag);
                    if ( hit.collider.tag != "Obstacle")
                    {
                        Debug.Log("yes");
                        aiLerp.canMove = true;

                        GraphNode node = AstarPath.active.GetNearest(hit.point).node;

                        if (node.Walkable)
                        {
                            Path p = seeker.StartPath(transform.position, (Vector3)node.position);
                            p.BlockUntilCalculated();
                            Debug.Log("hit point" + hit.point);
                            Debug.Log("node " + (Vector3)node.position);
                        }

                        
                    }
                 
                }
            }
            
        }
        
		
	}

   
    public void TargetReached()
    {
        
        transform.position = new Vector3(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y), 0);
        aiLerp.canMove = false;

            canCreateGrid = true;
    }

    void CreateClickableGrid(int numberOfMovements)
    {
        grid.GetNodes(node =>
        {
        Path p = seeker.StartPath(transform.position, (Vector3)node.position);
        p.BlockUntilCalculated();
            if (p.GetTotalLength() <= numberOfMovements + 0.1f)
            {
                if (p.GetTotalLength() > 0.9f && node.Walkable)
                {
                    Vector3 nodePos = (Vector3)node.position;
                    if (!Physics.Raycast(nodePos+ new Vector3(0,0,-0.1f), Vector3.forward, Mathf.Infinity, enemyMask))
                    {
                        GameObject clone = Instantiate(clickableSprite, nodePos, Quaternion.identity);
                        clickableSpriteList.Add(clone);
                    }
                }

                //Debug.Log("" + (Vector3)node.position); <<<---- utile 

            }
        }); 
    }

    public void DestroyClickableGrid()
    {
        foreach (GameObject c in clickableSpriteList)
        {
            Destroy(c);
        }
    }

    void SubtractMovementActions(Vector3 target)
    {
        Path p = seeker.StartPath(transform.position, target);
        p.BlockUntilCalculated();
        if (isCrouched)
        {
            playerActions -= Mathf.RoundToInt(p.GetTotalLength()) * 2;
        }
        else
        {
            playerActions -= Mathf.RoundToInt(p.GetTotalLength());
        }
    }
    /// <summary>
    /// Git Gud Casual
    /// </summary>
    /// <param name="enemy"> The Casual to kill</param>
    public void BackStabEnemy(GameObject enemy)
    {
        Vector3 lastEnemyPos = new Vector3 (enemy.transform.position.x, enemy.transform.position.y, 0);
        if (enemy.tag == "Bracciante")
        {
            enemy.GetComponent<Bracciante>().Die();
            Debug.Log("muori merda");
        }
       else if (enemy.tag == "CowBoy")
        {
            enemy.GetComponent<RagazzoMucca>().Die();
        }
       else if (enemy.tag == "Puttana")
        {
            enemy.GetComponent<RagazzaAmbiziosa>().Die();
        }
        if (playerActions > 0)
        {
            GameObject clone = Instantiate(clickableSprite, new Vector3(lastEnemyPos.x, lastEnemyPos.y, 0), Quaternion.identity);
            clickableSpriteList.Add(clone);
        }

    }


    private void LateUpdate()
    {
        transform.position = new Vector3(transform.position.x,transform.position.y,0);
    }

   
}
