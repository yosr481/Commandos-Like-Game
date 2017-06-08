using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    [Header("Camera Movment")]
    public GameObject cameraMover;
    public float cameraSpeed = 0.3f;

    void Update()
    {
        HandleCameraMovment();
    }

    void HandleCameraMovment()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 newPos = new Vector3(horizontal, 0, vertical) * cameraSpeed;
        cameraMover.transform.position += newPos;
    }

    public void ChangeStance()
    {
        //if (selectedUnit)
        //{
        //    selectedUnit.run = false;
        //    selectedUnit.crouch = !selectedUnit.crouch;
        //}
    }
}
