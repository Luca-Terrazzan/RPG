using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{

    public float scrollSpeed;
    public float topBarrier;
    public float botBarrier;
    public float leftBarrier;
    public float rightBarrier;
    public float maxZoom;
    public float minZoom;
    public float zoomSize = 5;

    public float camMaxYUp = 370f;
    public float camMaxYDown = 100f;
    public float camMaxXLeft = 260f;
    public float camMaxXRight = 720f;


    void Update()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            Debug.Log(Input.GetAxis("Mouse ScrollWheel"));
            zoomSize -= 2;
            if (zoomSize > minZoom)
            {
               /* camMaxYUp += 2;
                camMaxYDown -= 2;
                camMaxXRight += 2;
                camMaxXLeft -= 2;*/
            }
            if (zoomSize <= minZoom)
            {
                zoomSize = minZoom;
            }

        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            zoomSize += 2;

            if (zoomSize < maxZoom)
            {
                /*camMaxYUp -= 2;
                camMaxYDown += 2;
                camMaxXRight -= 2;
                camMaxXLeft += 2;*/
            }

            if (zoomSize >= maxZoom)
            {
                zoomSize = maxZoom;
            }
        }


        GetComponent<Camera>().orthographicSize = zoomSize;


        if (Input.mousePosition.y >= Screen.height * topBarrier)
        {
            if (this.transform.position.y <= camMaxYUp)
            {

                transform.Translate(Vector3.up * Time.deltaTime * scrollSpeed, Space.Self);
            }
        }
        if (Input.mousePosition.y <= Screen.height * botBarrier)
        {
            if (this.transform.position.y >= camMaxYDown)
            {
                transform.Translate(Vector3.down * Time.deltaTime * scrollSpeed, Space.Self);
            }
        }
        if (Input.mousePosition.x >= Screen.width * rightBarrier)
        {
            if (this.transform.position.x <= camMaxXRight)
            {
                transform.Translate(Vector3.right * Time.deltaTime * scrollSpeed, Space.World);
                transform.Translate(Vector3.back * Time.deltaTime * scrollSpeed / 2, Space.World);
            }


        }
        if (Input.mousePosition.x <= Screen.width * leftBarrier)
        {
            if (this.transform.position.x >= camMaxXLeft)
            {
                transform.Translate(Vector3.left * Time.deltaTime * scrollSpeed, Space.World);
                transform.Translate(Vector3.forward * Time.deltaTime * scrollSpeed / 2, Space.World);

            }


        }
    }
}