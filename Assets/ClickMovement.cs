using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickMovement : MonoBehaviour {


    AILerp ai;
    public Camera cam;
	// Use this for initialization
	void Start () {
        ai = GetComponent<AILerp>();
	}
	
	// Update is called once per frame
	void Update () {
		
        if(Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(cam.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

            if(hit.collider!=null)
            {
                if (hit.collider.tag == "ClickSquare")
                {
                    ai.target.position = hit.transform.position;
                    ai.canSearch = true;
                    ai.canMove = true;
                }
            }
            
        }

	}
}
