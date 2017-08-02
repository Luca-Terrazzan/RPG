using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movimento : MonoBehaviour
{

	public GameObject Player;
	private Vector3 nuovo;
	private float minimum = 4f, maximum =8f;

	void LateUpdate ()
	{
		nuovo = (Camera.main.ScreenToWorldPoint (Input.mousePosition) + Player.transform.position) / 4;

		//Camera.main.transform.position = nuovo;
		Camera.main.transform.position = Vector3.Lerp (Camera.main.transform.position, nuovo, 0.9f *Time.deltaTime) ;

		if (Input.GetAxis("Mouse ScrollWheel") != 0f) // forward
		{
			Camera.main.orthographicSize = Mathf.Clamp (Mathf.Lerp(Camera.main.orthographicSize, Camera.main.orthographicSize-Input.GetAxis("Mouse ScrollWheel"), 0.5f), minimum, maximum);
		}
	}
}