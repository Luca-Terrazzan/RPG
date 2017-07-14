using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using Pathfinding.Util;

public class PlayerActions : MonoBehaviour{

    public int playerActionsAmount;
    public GameObject clickableSprite;
    public Camera cam;

    private GridGraph grid;
    private Seeker seeker;
    private AILerp aiLerp;
    private List<GameObject> clickableSpriteList;

    public bool isMyTurn = false;
    public bool canCreateGrid = true;

    // Use this for initialization
    void Start () {

        clickableSpriteList = new List<GameObject>();
        grid = AstarPath.active.data.gridGraph;
        seeker = GetComponent<Seeker>();
        aiLerp = GetComponent<AILerp>();

    }
	
	// Update is called once per frame
	void Update () {

        if (isMyTurn)
        {

            
            if (canCreateGrid)
            {
                CreateClickableGrid();
                canCreateGrid = false;
            }

            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit2D hit = Physics2D.Raycast(cam.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

                if (hit.collider != null)
                {
                    if (hit.collider.tag == "ClickableSprite")
                    {
                        SubtractMovementActions(hit.transform.position);
                        aiLerp.target.position = hit.transform.position;
                        DestroyClickableGrid();
                        aiLerp.canMove = true;

                    }
                }

            }

            if (playerActionsAmount <= 0)
            {
                isMyTurn = false;
            }
        }
		
	}

   
    public void TargetReached()
    {
        transform.position = new Vector3(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y), transform.position.z);
        aiLerp.canMove = false;
        canCreateGrid = true;
    }

    void CreateClickableGrid()
    {
        grid.GetNodes(node =>
        {
            Path p = seeker.StartPath(transform.position, (Vector3)node.position);
            p.BlockUntilCalculated();
            if (p.GetTotalLength() <= playerActionsAmount+0.1f)
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
        playerActionsAmount -= Mathf.RoundToInt(p.GetTotalLength());
    }
}
