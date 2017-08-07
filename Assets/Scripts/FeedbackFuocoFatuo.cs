using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeedbackFuocoFatuo : MonoBehaviour
{

    [SerializeField] SpriteRenderer feedBackFatuo, fuocoFatuo;
	// Use this for initialization
	void Start ()
    {
        feedBackFatuo = transform.GetChild(1).GetComponentInChildren<SpriteRenderer>();
        fuocoFatuo = transform.GetChild(0).GetComponentInChildren<SpriteRenderer>();
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    private void OnMouseEnter()
    {
        if (!fuocoFatuo.enabled)
        { 
            feedBackFatuo.enabled = true;
        }
    }
    private void OnMouseExit()
    {
        feedBackFatuo.enabled = false;
    }
}
