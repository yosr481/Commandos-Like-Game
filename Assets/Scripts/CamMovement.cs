using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CamMovement : MonoBehaviour {

    public Transform mainCamera;
    public float movingAmount = 5;

    bool moveRight = false;
    bool moveLeft = false;
    bool moveUp = false;
    bool moveDown = false;

	// Update is called once per frame
	void Update () {
		if(moveRight)
            mainCamera.Translate(new Vector3(movingAmount * Time.deltaTime, 0, 0));
        if (moveLeft)
            mainCamera.Translate(new Vector3(-movingAmount * Time.deltaTime, 0, 0));
        if (moveUp)
            mainCamera.Translate(new Vector3(0, movingAmount * Time.deltaTime, 0));
        if (moveDown)
            mainCamera.Translate(new Vector3(0, -movingAmount * Time.deltaTime, 0));
    }

    public void MoveingRight()
    {
        moveRight = true;
    }

    public void MoveingLeft()
    {
        moveLeft = true;
    }

    public void MoveingUp()
    {
        moveUp = true;
    }

    public void MoveingDown()
    {
        moveDown = true;
    }

    public void StopMove()
    {
        moveRight = false;
        moveLeft = false;
        moveUp = false;
        moveDown = false;
    }
}
