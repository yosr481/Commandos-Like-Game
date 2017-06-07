using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zoom : MonoBehaviour {
    public float speed = .5f;

    float minZoom = 4;
    float maxZoom = 50;
    Camera camera;
	// Use this for initialization
	void Start () {
        camera = GetComponent<Camera>();
	}
	
	// Update is called once per frame
	void Update () {
        float zoom = Input.GetAxis("Mouse ScrollWheel");

        if(camera.orthographicSize > minZoom && camera.orthographicSize < maxZoom)
            camera.orthographicSize -= zoom * speed * Time.deltaTime;

        if (camera.orthographicSize < minZoom)
            camera.orthographicSize = 4.2f;

        if (camera.orthographicSize > maxZoom)
            camera.orthographicSize = 49.8f;
    }
}
