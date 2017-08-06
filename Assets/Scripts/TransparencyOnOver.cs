using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransparencyOnOver : MonoBehaviour {
    private SpriteRenderer sprite;

    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
    }
    void Update()
    {
        RaycastHit hit;
        if (Camera.main.WorldToScreenPoint(transform.position).y < Input.mousePosition.y)
        {
            sprite.color = new Color(1, 1, 1, 0.5f);
        }
        else
        {
            sprite.color = new Color(1, 1, 1, 1);
        }
        
        
    }
}
