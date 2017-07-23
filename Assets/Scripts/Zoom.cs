using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zoom : MonoBehaviour {

    public float zoomIn = 4;
    public float normal = 8;
    public float zoomOut = 16;
    public float smooth = 5;

    Camera cam;
    CameraManager camManager;
	void Start () {
        cam = GetComponent<Camera>();
        camManager = GetComponentInParent<CameraManager>();
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
                camManager.cameraSpeed = 0.3f;
            }
        }
        else if(Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            if (cam.orthographicSize == zoomIn)
            {
                cam.orthographicSize = normal;
                camManager.cameraSpeed = 0.3f;
            }
            else if (cam.orthographicSize == normal)
            {
                cam.orthographicSize = zoomOut;
                camManager.cameraSpeed = 0.6f;
            }
        }
	}
}
