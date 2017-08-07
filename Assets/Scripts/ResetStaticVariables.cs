using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetStaticVariables : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
        FreeRoamingPos.i = 0;
        FreeRoamingPos.staticFuochiFatui = 0;
        FreeRoamingPos.j = 0;
        FreeRoamingPos.triggerOnceEvent = true;

    }
	
}
