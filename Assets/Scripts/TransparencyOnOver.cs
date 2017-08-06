using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransparencyOnOver : MonoBehaviour {
    private SpriteRenderer sprite;
    private float distanceForTransparency = 80;

    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
    }
    void Update()
    {
        RaycastHit hit;
        if (Camera.main.WorldToScreenPoint(transform.position).y < Input.mousePosition.y && Vector3.Distance(new Vector3(Input.mousePosition.x,Input.mousePosition.y,0),new Vector3(Camera.main.WorldToScreenPoint(transform.position).x, Camera.main.WorldToScreenPoint(transform.position).y,0))< distanceForTransparency)
        {
            sprite.color = new Color(1, 1, 1, 0.5f);
        }
        else
        {
            sprite.color = new Color(1, 1, 1, 1);
        }
        
    }
}
