using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using Pathfinding.Util;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

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
    public Transform wakandaSpriteTransform;
    


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
    public int fakePlayerActions;
    public bool canHide = false;
    public bool isHidden = false;
    private bool isMenuOpen = false;
    //[HideInInspector]
    public bool lowInvisible;
    [HideInInspector]
    public Transform armadioFrontTransform;

    
    private int numerOfPathNode;
    private Vector3[] nodeArray;
    private SpriteRenderer sprite;
    private LineRenderer lineOfMovement;
   

    // private Button crouchButton;
    // private Button endTurnButton;
    // private Button menuButton;
    // private Button exit;
    // private Button backToGame;
    private Button crouchButton, endTurnButton, menuButton, goToExitInterface, backToGame, exitTheGame, backToGameTwo, resetScene ;

    private Image exitInterface, menuInterface, actionsBar, fakeActionsBar, backgroundBar, menuImage, deathInterface;

    private Animator anim;

    private FreeRoamingPos newPos;


    private SpriteRenderer wakandaSprite;

   
    // Use this for initialization
    void Start ()
    {
        #region FindObjects

        if (isFreeRoaming)
        {
            newPos = GameObject.Find("FreeRoamingManager").GetComponent<FreeRoamingPos>();
            newPos.ChangeFreeroamingPos();
        }
        else
        {
            turnManager = GameObject.Find("TurnManager").GetComponent<TurnManager>();
        }
        menuInterface = GameObject.Find("MenuInterface").GetComponent<Image>();
        backgroundBar = GameObject.Find("BackgroundBar").GetComponent<Image>();
        fakeActionsBar = GameObject.Find("FakeActionsBar").GetComponent<Image>();
        crouchButton = GameObject.Find("Crouch").GetComponent<Button>();
        endTurnButton = GameObject.Find("EndTurn").GetComponent<Button>();
        menuButton = GameObject.Find("Menu").GetComponent<Button>();
        backToGame = GameObject.Find("Riprendi").GetComponent<Button>();
        goToExitInterface = GameObject.Find("Esci").GetComponent<Button>();
        wakandaSpriteTransform = GameObject.Find("WakandaSprite").transform;
        actionsBar = GameObject.Find("ActionsBar").GetComponent<Image>();
        clickableSprite = GameObject.Find("clickableSprite");
        cam = GameObject.Find("Camera").GetComponent<Camera>();
        exitInterface = GameObject.Find("ExitInterface").GetComponent<Image>();
        exitTheGame = GameObject.Find("ConfermaUscita").GetComponent<Button>();
        backToGameTwo = GameObject.Find("TornaAlGioco").GetComponent<Button>();
        deathInterface = GameObject.Find("DeathCartel").GetComponent<Image>();
        resetScene = GameObject.Find("GoBackToGame").GetComponent<Button>();
        #endregion

        #region Click Buttons 

        crouchButton.onClick.AddListener(CrouchMethod);
        endTurnButton.onClick.AddListener(EndTurn);
        if (!isMenuOpen)
        { }
        menuButton.onClick.AddListener(Menu);
         backToGame.onClick.AddListener(CloseMenu); 
        goToExitInterface.onClick.AddListener(ExitGame);
        backToGameTwo.onClick.AddListener(CloseMenu);
        resetScene.onClick.AddListener(LoadCurrentScene);


        #endregion

        clickableSpriteList = new List<GameObject>();
        grid = AstarPath.active.data.gridGraph;
        seeker = GetComponent<Seeker>();
        aiLerp = GetComponent<AILerp>();
        sprite = GetComponent<SpriteRenderer>();
        lineOfMovement = GetComponent<LineRenderer>();
        playerActions = playerActionsPerTurn;
        lineOfMovement.sortingLayerName = "SoundRange";
        anim = wakandaSpriteTransform.GetComponent<Animator>();
        wakandaSprite = wakandaSpriteTransform.GetComponent<SpriteRenderer>();

        SetInterface();
    }
  
    float AngleToPositive(float angle)
    {
        if (angle > 359)
        {
            return angle - 360;
        }
        else if (angle < 0)
        {
            return 360 - angle;
        }
        else return angle;
    }

    // Update is called once per frame
    void Update()
    {
        

        if (AngleToPositive(transform.rotation.eulerAngles.z) > 45 && AngleToPositive(transform.rotation.eulerAngles.z) < 225)
        {
            wakandaSprite.flipX = true;
        }
        else
        {
            wakandaSprite.flipX = false;
        }

        anim.SetBool("isCrouched", isCrouched);
        anim.SetBool("isMoving", aiLerp.canMove);
        anim.SetFloat("Angle", AngleToPositive(transform.rotation.eulerAngles.z));

        #region Is My Turn
        if (isMyTurn && !isMenuOpen)
        {
            AstarPath.active.Scan();
            actionsBar.fillAmount = (float)(playerActions - fakePlayerActions) / playerActionsPerTurn;
            fakeActionsBar.fillAmount = (float)playerActions / playerActionsPerTurn;
            if (canCreateGrid)
            {
                if (!isHidden)
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
                else if (isHidden)
                {
                    GameObject clone = Instantiate(clickableSprite, armadioFrontTransform.position, Quaternion.identity);
                    clone.tag = "HideSprite";
                    clickableSpriteList.Add(clone);
                    clone.GetComponent<SpriteRenderer>().color = Color.green;
                }
                canCreateGrid = false;
            }

            RaycastHit hit;
            if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit))
            {
                if (hit.collider != null)
                {

                    if (hit.collider.tag == "ClickableSprite")
                    {
                        lineOfMovement.enabled = true;
                        Path p = seeker.StartPath(this.transform.position, hit.transform.position);
                        p.BlockUntilCalculated();
                        List<Vector3> pathNodeList = p.vectorPath;
                        lineOfMovement.positionCount = pathNodeList.Count;
                       


                        for (int i = 0; i < clickableSpriteList.Count; i++)
                        {
                            if (clickableSpriteList[i] != null)
                                clickableSpriteList[i].GetComponent<SpriteRenderer>().color = Color.cyan;
                        }



                        for (int i = 0; i < pathNodeList.Count - 1; i++)
                        {
                            Collider[] changeColor = Physics.OverlapBox(new Vector3(pathNodeList[i + 1].x, pathNodeList[i + 1].y, 0), new Vector3(this.transform.localScale.x / 2, this.transform.localScale.y / 2, 6f));
                            lineOfMovement.SetPosition(i, new Vector3(pathNodeList[i].x, pathNodeList[i].y, 0));
                            lineOfMovement.SetPosition(i + 1, new Vector3(pathNodeList[i + 1].x, pathNodeList[i + 1].y, 0));

                            GameObject clickCollider = gameObject;
                            GameObject hearCollider = gameObject;
                            for (int j = 0; j < changeColor.Length; j++)
                            {
                                if (changeColor[j].tag == "HearRange")
                                {
                                    hearCollider = changeColor[j].gameObject;
                                }
                                if (changeColor[j].tag == "ClickableSprite")
                                {
                                    clickCollider = changeColor[j].gameObject;
                                }


                            }
                            if (clickCollider.tag == "ClickableSprite" && hearCollider.tag == "HearRange")
                            {
                                Vector3 dirFromAtoB = (hearCollider.transform.position - clickCollider.transform.position).normalized;


                                GameObject enemy = hearCollider.transform.parent.parent.GetChild(0).gameObject;

                                float dotProd = Vector3.Dot(dirFromAtoB, enemy.transform.forward);
                                if (dotProd > 0.7)
                                {
                                    clickCollider.GetComponent<SpriteRenderer>().color = Color.red;
                                }
                                else clickCollider.GetComponent<SpriteRenderer>().color = Color.yellow;
                            }

                        }


                        fakePlayerActions = Mathf.RoundToInt(p.GetTotalLength());

                        if (Input.GetMouseButtonDown(0))
                        {
                            aiLerp.canMove = true;
                            SubtractMovementActions(hit.transform.position);
                            DestroyClickableGrid();
                            fakePlayerActions = 0;
                            if (isCrouched)
                            {
                                canBeHeard = false;
                            }
                            else
                            {
                                canBeHeard = true;
                                StartCoroutine("ChangeCanBeHeardWithDelay");
                            }
                        }
                    }

                    if (hit.collider.tag == "Armadio" && canHide && !isHidden && armadioFrontTransform.transform.parent.position == hit.transform.position)
                    {
                        if (Input.GetMouseButton(0))
                        {
                            if (playerActions >= 3)
                            {
                                armadioFrontTransform.parent.gameObject.layer = 0;
                                AstarPath.active.Scan();
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
                            armadioFrontTransform.parent.gameObject.layer = 8;
                            AstarPath.active.Scan();
                        }
                    }


                }


            }

        }
        #endregion

        #region Free Roaming

        if (isFreeRoaming && !isMenuOpen) // variabile da attivare quando ci si trova in un'area esterna alle zone pericolose
        {
            lineOfMovement.enabled = false;  
            DestroyClickableGrid(); // distruzione delle griglie per liberare il giocatore dal vincolo dei punti azione 

            if (Input.GetMouseButtonDown(0))
            {
                if (!EventSystem.current.IsPointerOverGameObject())
                {
                    RaycastHit hit;

                    if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit))
                    {
                        Debug.Log(hit.collider.gameObject.tag);
                        if (hit.collider.tag != "Obstacle")
                        {

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
        #endregion
 
    }

    IEnumerator ChangeCanBeHeardWithDelay()
    {
        yield return new WaitForSeconds(0.1f);
        canBeHeard = false;
    }

       #region Methods

    void EndTurn()
    {
        if (!aiLerp.canMove && isMyTurn)
        {
            isMyTurn = false;
            DestroyClickableGrid();
            turnManager.changeTurn();
            playerActions = playerActionsPerTurn;
            canCreateGrid = true;
            canBeHeard = false;
            actionsBar.fillAmount = 1;
            fakeActionsBar.fillAmount = 1;
        }

    }

    void CrouchMethod()
    {
        if(!aiLerp.canMove && !isHidden && isMyTurn)
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
    }

    void Menu()
    {
        
        isMenuOpen = true;
       
        menuInterface.gameObject.SetActive(true);
        Time.timeScale = 0;

    }

    void CloseMenu()
    {
        Time.timeScale = 1;
        menuInterface.gameObject.SetActive(false);
        exitInterface.gameObject.SetActive(false);
        isMenuOpen = false;

    }

    void ExitGame ()
    {
        menuInterface.gameObject.SetActive(false);
        Time.timeScale = 0;
        exitInterface.gameObject.SetActive(true);
    }

    public void Die()
    {
        deathInterface.gameObject.SetActive(true);

    }

    void LoadCurrentScene()
    {
     SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void SetInterface ()
    {
        menuInterface.gameObject.SetActive(false);
        exitInterface.gameObject.SetActive(false);
        deathInterface.gameObject.SetActive(false);
    }

    public void TargetReached()
    {
        if (isCrouched)
        {
            canBeHeard = false;
        }
        else if(!isCrouched)
        {
            canBeHeard = true;
        }

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
        lineOfMovement.enabled = false;
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
        if (playerActions >= 6)
        {
            Vector3 lastEnemyPos = new Vector3(enemy.transform.position.x, enemy.transform.position.y, 0);
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
            GameObject clone = Instantiate(clickableSprite, new Vector3(lastEnemyPos.x, lastEnemyPos.y, 0), Quaternion.identity);
            clickableSpriteList.Add(clone);
            playerActions -= 6;
        }
        

    }
    #endregion

    private void LateUpdate()
    {
        transform.position = new Vector3(transform.position.x,transform.position.y,0);
        wakandaSpriteTransform.position = transform.position + new Vector3(-0.5f, 0.5f, 0);
    }

   
}
