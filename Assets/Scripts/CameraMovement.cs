using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{

	public GameObject Player;
	private Vector3 offset;
	private float minimum = 3f, maximum =8f;

    public float horizontalClamp;
    public float verticalClamp;
    public float moveSpeed;


    void Start ()
	{
		offset = new Vector3 (0, 0, -5);
	}

	void LateUpdate ()
	{

		if (Input.GetAxis("Mouse ScrollWheel") != 0f) // forward
		{
			Camera.main.orthographicSize = Mathf.Clamp (Camera.main.orthographicSize-Input.GetAxis("Mouse ScrollWheel"), minimum, maximum);
		}

	}
}