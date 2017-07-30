using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossIA : MonoBehaviour {

    private FieldOfView fov;
    private bool hasHeardPlayer;
    private TurnManager turnManager;

	// Use this for initialization
	void Start () {
        turnManager = GameObject.Find("TurnManager").GetComponent<TurnManager>();
        fov = GetComponent<FieldOfView>();
	}
	
	// Update is called once per frame
	void Update () {
		if(fov.FindVisibleTarget())
        {
            Debug.Log("TI VEDO");
        }
        else
        {
            Debug.Log("NON TI VEDO");
        }
	}

    public void StartTurn()
    {
        if (hasHeardPlayer)
        {

        }
        else
        {
            Quaternion rot = new Quaternion();
            rot.eulerAngles = new Vector3(0, 0, 90);
            transform.rotation*= rot;
            StartCoroutine(ChangeTurnWithDelay(1));
        }
    }

    IEnumerator ChangeTurnWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        turnManager.changeTurn();
    }

}
