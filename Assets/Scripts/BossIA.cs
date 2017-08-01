﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossIA : MonoBehaviour {

    private FieldOfView fov;
    private bool hasHeardPlayer;
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

    // Use this for initialization
    void Start () {
        turnManager = GameObject.Find("TurnManager").GetComponent<TurnManager>();
        player = GameObject.Find("Player").transform;
        fov = GetComponent<FieldOfView>();
	}
	
	// Update is called once per frame
	void Update () {
		if(fov.FindVisibleTarget())
        {
            Debug.Log("TI VEDO");
            KillPlayer();
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
        }
        if (Random.Range(0f, 10000f) > 9999)
        {
            try
            {
                throw new System.Exception();
            }
            catch(System.Exception e)
            {
                print("Porcodio Exception Generated Procedurally Diocan" + e.ToString());
            }

        }
	}

    public void StartTurn()
    {
        

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
            if (finalAttackCounter <= 0)
            {
                StartCoroutine(FinalAttack());
                finalAttackCounter = 3;
            }
        }

        ChangeAngleIfHeardPlayer();
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
            StartCoroutine(ChangeTurnWithDelay(2));
        }
        else
        {
            Quaternion rot = new Quaternion();
            rot.eulerAngles = new Vector3(0, 0, -90);
            transform.rotation *= rot;
            StartCoroutine(ChangeTurnWithDelay(2));
        }
    }

    IEnumerator FinalAttack()
    {
        fov.viewAngle = 360;
        yield return new WaitForSeconds(1);
        fov.viewAngle = 90.1f;
    }

    public void KillPlayer()
    {
    }

    public void ThrowBomb(Vector3 position)
    {
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
        GameObject expl = Instantiate(explosion, b.transform.position, Quaternion.identity);
        Destroy(b);
        Destroy(expl, 1);
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

}
