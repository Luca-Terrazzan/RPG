using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{

	public GameObject Player;
	private Vector3 offset;

	void Start ()
	{
		offset = new Vector3 (3f, -5f, -5f);
	}

	void LateUpdate ()
	{
		transform.position = Player.transform.position + offset;
	}
}