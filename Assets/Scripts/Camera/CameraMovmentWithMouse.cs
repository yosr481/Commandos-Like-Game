using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovmentWithMouse : MonoBehaviour {
    public Transform target;
    public float speed = 1;
    public bool startMoving;
    Vector3 direction;

    void Update()
    {
        if (startMoving)
        {
            target.position += direction;
        }
    }
    
    public void StartMovingForward(bool opposite)
    {
        startMoving = transform;

        direction = (!opposite) ? target.forward : -target.forward;
        direction *= speed;
    }

    public void StartMovingSides(bool opposite)
    {
        startMoving = transform;

        direction = (!opposite) ? target.right : -target.right;
        direction *= speed;
    }

    public void StopMoving()
    {
        startMoving = false;
    }
}