using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FreeRoamingPos : MonoBehaviour {

    private PlayerActions playerPos;
    public GameObject[] newPos;
    public static int i = 0;
    public static int staticFuochiFatui = 0;
    public static bool[] goodKarma = new bool [4];
    public static int k;
    public static bool isWakandaNice;

	// Use this for initialization
	void Awake ()
    {
        playerPos = GameObject.Find("Player").GetComponent<PlayerActions>();
        
	}

    private void Update()
    {
        if (i > 1)
        {
            Debug.Log("state " + goodKarma[k] + " index " + k);
            Debug.Log("prima " + goodKarma[k - 1] + " indexprima" + (k-1));
        }
        
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
    public void KarmaSystem()
    {
        if (i > 1)
        {
            goodKarma[k] = isWakandaNice;
            k++;
        }
    }
  
}
