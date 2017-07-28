using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeRoamingPos : MonoBehaviour {

    private PlayerActions playerPos;
    public GameObject[] newPos;
    [SerializeField] private int i = 0;

	// Use this for initialization
	void Start ()
    {
        playerPos = GameObject.Find("Player").GetComponent<PlayerActions>();
	}


    public void ChangeFreeroamingPos ()
    {
        playerPos.transform.position = newPos[i].transform.position;
        i++;
        
    }
}
