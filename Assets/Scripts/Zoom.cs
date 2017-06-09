using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zoom : MonoBehaviour {

    public float zoomIn = 4;
    public float normal = 8;
    public float zoomOut = 16;
    public float smooth = 5;

    Camera cam;
	void Start () {
        cam = GetComponent<Camera>();
	}
	
	void Update () {
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            if(cam.orthographicSize == normal)
            {
                cam.orthographicSize = zoomIn;
            }
            else if(cam.orthographicSize == zoomOut)
            {
                cam.orthographicSize = normal;
            }
        }
        else if(Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            if (cam.orthographicSize == zoomIn)
            {
                cam.orthographicSize = normal;
            }
            else if (cam.orthographicSize == normal)
            {
                cam.orthographicSize = zoomOut;
            }
        }
	}
}
