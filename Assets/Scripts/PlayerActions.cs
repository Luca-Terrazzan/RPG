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
   
    private GridGraph grid;
    private Seeker seeker;
    private AILerp aiLerp;
    private List<GameObject> clickableSpriteList;

    public bool isMyTurn = false;
    private bool canCreateGrid = true;
    public bool isCrouched = false;

    public bool isFreeRoaming = false;

    private SpriteRenderer sprite;

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
                        Debug.Log(hit.collider.tag);

                        if (hit.collider.tag == "ClickableSprite")
                        {
                            Debug.Log("LBLBLBLBDBJNJ FUNZIONA PLZ");
                            SubtractMovementActions(hit.transform.position);
                            aiLerp.target.position = hit.transform.position;
                            DestroyClickableGrid();
                            aiLerp.canMove = true;

                        }
                    }

                }
            }

            if (Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                isMyTurn = false;
                DestroyClickableGrid();
                turnManager.changeTurn();
                playerActions = playerActionsPerTurn;
                canCreateGrid = true;
            }
        }

        if (isFreeRoaming)

        {
            if (Input.GetMouseButtonDown(0))
            {
              //  Debug.Log("Ciao");

                RaycastHit hit;

                if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit))
                {
                    if (hit.collider != null)
                    {
                       
                            GraphNode node = AstarPath.active.GetNearest(hit.point).node;

                            if (node.Walkable)
                            {
                                Path p = seeker.StartPath(transform.position, (Vector3)node.position);
                                p.BlockUntilCalculated();
                                Debug.Log("hit point" +  hit.point);
                                Debug.Log("node " + (Vector3)node.position );
                            }
                               // Debug.Log("ciao");
                            
                        
                    }
                }
            }
            
        }
		
	}

   
    public void TargetReached()
    {
        transform.position = new Vector3(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y), transform.position.z);
        aiLerp.canMove = false;
        canCreateGrid = true;
    }

    void CreateClickableGrid(int numberOfMovements)
    {
        grid.GetNodes(node =>
        {
            Path p = seeker.StartPath(transform.position, (Vector3)node.position);
            p.BlockUntilCalculated();
            if (p.GetTotalLength() <= numberOfMovements+0.1f)
            {
                if (p.GetTotalLength() >0.9f&&node.Walkable)
                {
                    GameObject clone = Instantiate(clickableSprite, (Vector3)node.position, Quaternion.identity);
                    clickableSpriteList.Add(clone);
                   
                }
               
                //Debug.Log("" + (Vector3)node.position); <<<---- utile 

            }
        }); 
    }

    void DestroyClickableGrid()
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
}
