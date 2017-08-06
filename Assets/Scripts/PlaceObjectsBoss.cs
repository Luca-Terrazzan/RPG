using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceObjectsBoss : MonoBehaviour {

    public SpriteRenderer botRightSprite, topRightSprite, topLeftSprite, botLeftSprite;
    private PlayerActions player;
    private BossIA boss;
    public AudioClip pickUpSound;
	// Use this for initialization
	void Start () {
        player = GetComponent<PlayerActions>();
        boss = GameObject.Find("Boss").GetComponent<BossIA>();
	}
	
	// Update is called once per frame
	void Update () {
        if(player.isMyTurn)
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
            {
                if (Input.GetMouseButtonDown(0) && Vector3.Distance(hit.transform.position, transform.position) < 1.1f)
                {
                    Debug.Log("ho cliccato e distanza corretta");
                    if (hit.collider.tag == "PlaceObject" && player.playerActions > 0 && !hit.collider.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled)
                    {
                        Debug.Log("porcodio");
                        player.playerActions -= 1;
                        player.DestroyClickableGrid();
                        player.canCreateGrid = true;
                        hit.collider.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = true;
                        player.WakandaSounds(pickUpSound);
                    }
                }
            }

            if (botRightSprite.enabled && topRightSprite.enabled && topLeftSprite.enabled && botLeftSprite.enabled)
            {
                Debug.Log("Hai vinto bravo wow");
            }
            if (botRightSprite.enabled && topRightSprite.enabled)
            {
                boss.secondState = true;
            }
            if (botRightSprite.enabled && topRightSprite.enabled && topLeftSprite.enabled)
            {
                boss.thirdState = true;
            }
        }
    }
}
