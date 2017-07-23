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
    private int fakePlayerActions;
    public bool canHide = false;
    public bool isHidden = false;
    [HideInInspector]
    public Transform armadioFrontTransform;

    
    private int numerOfPathNode;
    private Vector3[] nodeArray;
    private SpriteRenderer sprite;
    private LineRenderer lineOfMovement;
   

    // Use this for initialization
    void Start ()
    {

        clickableSpriteList = new List<GameObject>();
        grid = AstarPath.active.data.gridGraph;
        seeker = GetComponent<Seeker>();
        aiLerp = GetComponent<AILerp>();
        sprite = GetComponent<SpriteRenderer>();
        lineOfMovement = GetComponent<LineRenderer>();
        playerActions = playerActionsPerTurn;

    }
	
	// Update is called once per frame
	void Update ()
    {
        

        if (isMyTurn)
        {
            if (canCreateGrid)
            {
                if(!isHidden)
                {
                    if (!isCrouched)
                    {
                        CreateClickableGrid(playerActions);
                    }
                    else if (isCrouched)
                    {
                        CreateClickableGrid((int)Mathf.Floor(playerActions / 2));
                    }
                }
                else if(isHidden)
                {
                    GameObject clone = Instantiate(clickableSprite, armadioFrontTransform.position, Quaternion.identity);
                    clone.tag = "HideSprite";
                    clickableSpriteList.Add(clone);
                    clone.GetComponent<SpriteRenderer>().color = Color.green;
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

            RaycastHit hit;
            if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit) )
            {
                if (hit.collider != null)
                {

                    if (hit.collider.tag == "ClickableSprite")
                    {
                        Path p = seeker.StartPath(this.transform.position, hit.transform.position);
                        p.BlockUntilCalculated();
                        List<Vector3> pathNodeList = p.vectorPath;
                        lineOfMovement.positionCount = pathNodeList.Count;
                        //  lineOfMovement.SetPosition(0, this.transform.position);
                        //  lineOfMovement.SetPosition(1, hit.transform.position);

                        
                       

                        for (int i = 0; i < pathNodeList.Count - 1; i++)
                        {
                            lineOfMovement.SetPosition(i, pathNodeList[i]);
                            lineOfMovement.SetPosition(i + 1, pathNodeList[i+1]);
                          
                            //Debug.DrawLine(pathNodeList[i], pathNodeList[i + 1]);
                            
                            
                        }

                        fakePlayerActions = Mathf.RoundToInt(p.GetTotalLength());

                        if (Input.GetMouseButton(0))
                        {
                            aiLerp.canMove = true;
                            SubtractMovementActions(hit.transform.position);
                            DestroyClickableGrid();

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

                    if(hit.collider.tag =="Armadio" && canHide && !isHidden)
                    {
                        if (Input.GetMouseButton(0))
                        {
                            if (playerActions >= 3)
                            {
                                DestroyClickableGrid();
                                isHidden = true;
                                Path p = seeker.StartPath(this.transform.position, hit.transform.position);
                                p.BlockUntilCalculated();
                                aiLerp.canMove = true;
                                playerActions -= 3;
                                canBeHeard = false;
                                GetComponent<Collider>().enabled = false;
                            }
                        }
                    }

                    if (hit.collider.tag == "HideSprite" && isHidden)
                    {
                        if (Input.GetMouseButton(0))
                        {
                            DestroyClickableGrid();
                            isHidden = false;
                            Path p = seeker.StartPath(this.transform.position, hit.transform.position);
                            p.BlockUntilCalculated();
                            aiLerp.canMove = true;
                            playerActions -= 3;
                            canBeHeard = false;
                            GetComponent<Collider>().enabled = true;
                        }
                    }

         /*       if (Input.GetMouseButtonDown(0))
                {

                    if (hit.collider != null)
                    {

                        if (hit.collider.tag == "ClickableSprite")
                        {
                            SubtractMovementActions(hit.collider.transform.position);
                            //aiLerp.target.position = hit.transform.position;
                            DestroyClickableGrid();
                           // aiLerp.canMove = true;
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

                } */
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
           
           /* if (Input.GetMouseButtonDown(0))
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
                 
                } */
            }
            
        }
        
		
	}


    public void TargetReached()
    {

        if (isHidden)
        {
            if(playerActions>=3)
            {
                GameObject clone = Instantiate(clickableSprite, armadioFrontTransform.position, Quaternion.identity);
                clone.tag = "HideSprite";
                clickableSpriteList.Add(clone);
                clone.GetComponent<SpriteRenderer>().color = Color.green;
            }
        }
        else
        {
            transform.position = new Vector3(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y), 0);
            canCreateGrid = true;
        }

        aiLerp.canMove = false;

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
     
       
        if (isCrouched)
        {
            playerActions -= fakePlayerActions * 2;
        }
        else
        {
            playerActions -= fakePlayerActions ;
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
