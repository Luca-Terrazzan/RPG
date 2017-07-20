﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{

	public GameObject Player;
	private Vector3 offset;
	private float minimum = 3f, maximum =6f;

	void Start ()
	{
		offset = new Vector3 (5f, -5f, -5f);
	}

	void LateUpdate ()
	{
		transform.position = Player.transform.position + offset;

		if (Input.GetAxis("Mouse ScrollWheel") != 0f) // forward
		{
			Camera.main.orthographicSize = Mathf.Clamp (Camera.main.orthographicSize-Input.GetAxis("Mouse ScrollWheel"), minimum, maximum);
		}
	}
}