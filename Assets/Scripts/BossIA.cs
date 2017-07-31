using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossIA : MonoBehaviour {

    private FieldOfView fov;
    private bool hasHeardPlayer;
    private TurnManager turnManager;
    public Transform player;
    Vector3 targetDir;

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
	}

    public void StartTurn()
    {
        if (hasHeardPlayer)
        {
            targetDir = player.position - transform.position;
            transform.up = player.position - transform.position;
            Quaternion rot = new Quaternion();
            rot.eulerAngles = new Vector3(0,0,RoundAngleToNinety(transform.eulerAngles.z));
            transform.rotation = rot;
            StartCoroutine(ChangeTurnWithDelay(1));
        }
        else
        {
            Quaternion rot = new Quaternion();
            rot.eulerAngles = new Vector3(0, 0, 90);
            transform.rotation*= rot;
            StartCoroutine(ChangeTurnWithDelay(1));
        }
    }

    public void KillPlayer()
    {
        
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
