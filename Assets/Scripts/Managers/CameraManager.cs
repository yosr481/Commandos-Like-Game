using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour {

    [Header("Camera Movment")]
    public GameObject cameraGameObject;
    public float cameraSpeed = 0.3f;
    public bool canMove = true;

    float mouseX;
    float mouseY;

    [Header("Camera Rotation")]
    public bool verticalRotationEnabled = true;
    public float verticalRotationMin = 0;
    public float verticalRotationMax = 65;

    void LateUpdate()
    {
        HandleMouseRotation();
        if(canMove)
            HandleCameraMovment();

        mouseX = Input.mousePosition.x;
        mouseY = Input.mousePosition.y;
    }

    public void MoveToPosition(Vector3 pos)
    {
        transform.position = pos;
    }

    public void HandleMouseRotation()
    {
        var easeFactor = 10f;

        if (Input.GetMouseButton(1) && Input.GetKey(KeyCode.LeftAlt))
        {
            if(Input.mousePosition.x != mouseX)
            {
                var camRotationY = (Input.mousePosition.x - mouseX) * easeFactor * Time.deltaTime;
                transform.Rotate(0, camRotationY, 0);
            }

            if (verticalRotationEnabled && Input.mousePosition.y != mouseY)
            {
                var cameraRotationX = (mouseY - Input.mousePosition.y) * easeFactor * Time.deltaTime;
                var desiredRotationX = cameraGameObject.transform.eulerAngles.x * cameraRotationX;

                if (desiredRotationX >= verticalRotationMin || desiredRotationX <= verticalRotationMax)
                {
                    cameraGameObject.transform.Rotate(cameraRotationX, 0, 0);
                }
            }
        }

       
    }

    void HandleCameraMovment()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 newPos = new Vector3(horizontal, 0, vertical) * cameraSpeed;
        transform.position += newPos;
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
