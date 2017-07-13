using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnObstacle : MonoBehaviour {

	
    public void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.tag=="Obstacle")
        {
            Destroy(this.gameObject);
        }
    }   
}
