using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movimento : MonoBehaviour
{

	public GameObject Player;
	public Vector3 nuovo;
	private float minimum = 3f, maximum =8f;

	void LateUpdate ()
	{
		nuovo = (Camera.main.ScreenToWorldPoint (Input.mousePosition) + Player.transform.position) / 2;

		//Camera.main.transform.position = nuovo;
		Camera.main.transform.position = Vector3.Lerp (Camera.main.transform.position, nuovo, 0.9f *Time.deltaTime) ;

		if (Input.GetAxis("Mouse ScrollWheel") != 0f) // forward
		{
			Camera.main.orthographicSize = Mathf.Clamp (Camera.main.orthographicSize-Input.GetAxis("Mouse ScrollWheel"), minimum, maximum);
		}
	}
}