using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnObstacle : MonoBehaviour {

	
    public void OnCollisionEnter2D(Collision2D other)
    {
        Debug.Log("sono");
        if(other.gameObject.tag=="Obstacle")
        {
            Debug.Log("mannaggadd");
            Destroy(this.gameObject);
        }
    }
}
