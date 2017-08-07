using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BossIA : MonoBehaviour {

    private FieldOfView fov;
    [SerializeField] private bool hasHeardPlayer;
    private TurnManager turnManager;
    public Transform player;
    Vector3 targetDir;
    public bool secondState;
    public bool thirdState;
    public Transform[] bombPoints;
    public GameObject bomb;
    public GameObject explosion;
    public float distToBombPoints;
    [SerializeField]
    private int finalAttackCounter = 3;
    bool canKill = true;
    public Material fovMaterial;
    public SpriteRenderer sprite;
    public Animator anim;
    private AudioSource bossAudioPlayer;
    public AudioClip[] bossSounds;
    private Animator playerAnim;
    private AILerp playerMovement;
    private bool allarmTrigger = true, playerDead;
    private PlayerActions playerActions;
    private float turnDelay = 0;
    public GameObject thunder;
    public GameObject charging;
    public GameObject final;
    private GameObject clown;
    
   
  
    


    // Use this for initialization
    void Start ()
    {
        
        turnManager = GameObject.Find("TurnManager").GetComponent<TurnManager>();
        player = GameObject.Find("Player").transform;
        playerAnim = GameObject.Find("WakandaSprite").GetComponent<Animator>();
        fov = GetComponent<FieldOfView>();
        fovMaterial.SetColor("_EmissionColor", Color.white);
        bossAudioPlayer = GetComponent<AudioSource>();
        playerMovement = GameObject.Find("Player").GetComponent<AILerp>();
        playerActions = GameObject.Find("Player").GetComponent<PlayerActions>();
        
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
    void Update ()
    {

        if (AngleToPositive(transform.rotation.eulerAngles.z) > 260 && AngleToPositive(transform.rotation.eulerAngles.z) < 280)
        {
            sprite.flipX = true;
        }
        else if (transform.rotation.eulerAngles.z > 80 && transform.rotation.eulerAngles.z < 100)
        {
            sprite.flipX = true;
        }
        else
        {
            sprite.flipX = false;
        }
        

        anim.SetFloat("Angle", transform.rotation.eulerAngles.z);

        if (fov.FindVisibleTarget())
        {
            if(canKill)
            {
                Debug.Log("TI VEDO");
                KillPlayer();
            }
           
        }
        else
        {
            Debug.Log("NON TI VEDO");
        }

        if (fov.AmIHearingPlayer())
        { 
            hasHeardPlayer = true;
            
        }
        else
        {
            hasHeardPlayer = false;
            allarmTrigger = true;
        }


        if (!playerMovement.canMove && hasHeardPlayer && allarmTrigger)
        {
             BossSound(bossSounds[0]);
             allarmTrigger = false;
        }
    }

    public void StartTurn()
    {
        turnDelay = 1;

        if(secondState)
        {
            List<Transform> nearestPoints = new List<Transform>();
            for (int i = 0; i < bombPoints.Length; i++)
            {
                if (Vector3.Distance(bombPoints[i].position, player.position) < distToBombPoints)
                {
                    nearestPoints.Add ( bombPoints[i]);
                }
            }
            if (nearestPoints.Count > 0)
            {
                int randomPoint = Random.Range(0, nearestPoints.Count);

                ThrowBomb(nearestPoints[randomPoint].position);
            }           
        }
        if (thirdState)
        {
            finalAttackCounter--;
            if(finalAttackCounter==1)
            {
                StartCoroutine(FinalAttackPreview());
            }
            if (finalAttackCounter <= 0)
            {
                StartCoroutine(FinalAttack());
                finalAttackCounter = 3;
            }
        }

        ChangeAngleIfHeardPlayer();
        StartCoroutine(ChangeTurnWithDelay(turnDelay));

    }

    void ChangeAngleIfHeardPlayer()
    {
        if (hasHeardPlayer)
        {
            targetDir = player.position - transform.position;
            transform.up = player.position - transform.position;
            Quaternion rot = new Quaternion();
            rot.eulerAngles = new Vector3(0, 0, RoundAngleToNinety(transform.eulerAngles.z));
            transform.rotation = rot;
        }
        else
        {
            Quaternion rot = new Quaternion();
            rot.eulerAngles = new Vector3(0, 0, -90);
            transform.rotation *= rot;
        }
    }

    IEnumerator FinalAttackPreview()
    {
        anim.SetBool("isAttacking", true);
        Quaternion rot = new Quaternion();
        rot.eulerAngles = new Vector3(-35, -45, 60);
        clown = Instantiate(charging, new Vector3(-10,7,0), rot);
        turnDelay += 5;
        fov.viewAngle = 360;
        fov.viewRadius = 0;
        float timer = 0;
        float lerpSeconds = 3;
        canKill = false;
        fovMaterial.SetColor("_EmissionColor", Color.red);
        while (timer<lerpSeconds)
        {
            timer += Time.deltaTime;
            fov.viewRadius = Mathf.Lerp(0, 20, timer / lerpSeconds);
            yield return null;
        }
        int flashCounter = 0;
        while(flashCounter < 3)
        {
            fov.viewAngle = 0;
            yield return new WaitForSeconds(0.5f);
            fov.viewAngle = 360;
            flashCounter++;
            yield return new WaitForSeconds(0.5f);
        }
        fov.viewAngle = 90.1f;
        fovMaterial.SetColor("_EmissionColor", Color.white);
        canKill = true;
    }

    IEnumerator FinalAttack()
    {
        Destroy(clown);
        Quaternion rot = new Quaternion();
        rot.eulerAngles = new Vector3(-35, -45, 60);
        GameObject clone = Instantiate(final, transform.position, rot);
        turnDelay += 2;
        fov.viewAngle = 360;
        BossSound(bossSounds[3]);
        yield return new WaitForSeconds(2);
        fov.viewAngle = 90.1f;
        Destroy(clone);
        anim.SetBool("isAttacking", false);
    }

    public void KillPlayer()
    {
        
        if (!playerDead)
        {
            Quaternion rot = new Quaternion();
            rot.eulerAngles = new Vector3(-35, -45, 60);
            GameObject clone = Instantiate(thunder, player.position, rot);
            playerActions.Die();
            BossSound(bossSounds[1]);
            playerDead = true;
        }
    }

    public void ThrowBomb(Vector3 position)
    {
        turnDelay += 1;
        Debug.Log("Lancio una bomba in posizione: " + position.ToString());
        GameObject b = Instantiate(bomb, transform.position, Quaternion.identity);
        StartCoroutine(BombLerp(b, position));
    }

    IEnumerator BombLerp(GameObject b, Vector3 position)
    {
        float timer = 0;
        float timeToLerp = 1;
        while (timer < timeToLerp)
        {
            timer += Time.deltaTime;
            b.transform.position = Vector3.Lerp(b.transform.position, position, timer / timeToLerp);
            yield return null;
        }
        Quaternion rot = new Quaternion();
        rot.eulerAngles = new Vector3(-35, -45, 60);
        GameObject expl = Instantiate(explosion, b.transform.position, rot);
        Destroy(b);
        Destroy(expl, 1);
        BossSound(bossSounds[2]);
        yield return null;
    }

    IEnumerator ChangeTurnWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        turnManager.changeTurn();
    }

    float RoundAngleToNinety(float angleToRound)
    {
        float result = Mathf.Round(angleToRound / 90);
        Debug.Log(angleToRound+"  " + result);
        return result *= 90;
        
    }
    
    public void Die ()
    {
       
        Debug.Log("Sono morto" + this.gameObject.tag);
        //inserire animazione porcodio
        StartCoroutine(LoadingLastCutScene(5f)); // inserire al posto di 5 la durata dell'animazione del boss
    }

    IEnumerator LoadingLastCutScene(float bossDeathAnimation)
    {
        yield return new WaitForSeconds(bossDeathAnimation);
        SceneManager.LoadScene("EndingVideo");

    }
     void BossSound (AudioClip soundEffect)
     {
        bossAudioPlayer.PlayOneShot(soundEffect);

     }

        

}
