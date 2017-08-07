using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FreeRoamingPos : MonoBehaviour {

    private PlayerActions playerPos;
    public GameObject[] newPos;
    public static int i = 0;
    public static int staticFuochiFatui = 0;
    public static int[] karmaLevel = new int [5];
    public static int j;
   
    public static bool triggerOnceEvent = true;

	// Use this for initialization
	void Awake ()
    {
        playerPos = GameObject.Find("Player").GetComponent<PlayerActions>();
        
	}

/*
        int goodCounter=0, badCounter=0;
        

        for ( int i=0; i < goodKarma.Length; i++)
        {
           if (goodKarma[i]==0)
           {
                goodCounter++;
           }
           else if  (goodKarma[i] == 1)
           {
                badCounter++;
           }
           
        }
        if (goodCounter == goodKarma.Length)
        {
            //finale bello
        }
        else if (badCounter == goodKarma.Length)
        {
            //finale cattivo!!! non si fa!!!
        }
        else
        {
            //finale boh, schifo

        }
        */
    
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
