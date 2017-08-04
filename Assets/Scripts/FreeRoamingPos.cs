using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FreeRoamingPos : MonoBehaviour {

    private PlayerActions playerPos;
    public GameObject[] newPos;
    public static int i = 0;
    public static int staticFuochiFatui = 0;

	// Use this for initialization
	void Awake ()
    {
        playerPos = GameObject.Find("Player").GetComponent<PlayerActions>();
	}


    public void ChangeFreeroamingPos ()
    {
        
        playerPos.transform.position = newPos[i].transform.position;
        i++;
        if (i > 1)
        {
            staticFuochiFatui++;
        }
        
    }
}
