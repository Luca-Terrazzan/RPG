using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransparencyOnOver : MonoBehaviour {
    private SpriteRenderer sprite;
    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    private void OnMouseEnter()
    {
        sprite.color = new Color(1, 1, 1, 0.5f);
    }

    private void OnMouseExit()
    {
        sprite.color = new Color(1, 1, 1, 1);
    }
}
