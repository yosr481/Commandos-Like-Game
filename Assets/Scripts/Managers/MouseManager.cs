using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseManager : MonoBehaviour {

    public bool userIsDragging = false;
    static float timeLimitBeforDeclareDrag = 1;
    static float timeLeftBeforeDeclareDrag;
    static Vector2 mouseDragStart;
    static float clickDragZone = 1.3f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        // Mouse drag.
        if (Input.GetMouseButtonDown(0))
        {
            timeLeftBeforeDeclareDrag = timeLimitBeforDeclareDrag;
            mouseDragStart = Input.mousePosition;
        }
        else if (Input.GetMouseButton(0))
        {
            if (!userIsDragging)
            {
                timeLeftBeforeDeclareDrag -= Time.deltaTime;
                if(/*timeLeftBeforeDeclareDrag <= 0 || */UserDraggingByPosition(mouseDragStart, Input.mousePosition))
                {
                    userIsDragging = true;
                }
            }

            if (userIsDragging)
            {
                Debug.Log("User is dragging");
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            timeLeftBeforeDeclareDrag = 0;
            userIsDragging = false;
        }
	}

    public bool UserDraggingByPosition(Vector2 dragStartPoint, Vector2 newPoint)
    {
        if ((newPoint.x > dragStartPoint.x + clickDragZone || newPoint.x < dragStartPoint.x + clickDragZone) ||
            (newPoint.y > dragStartPoint.y + clickDragZone || newPoint.y < dragStartPoint.y + clickDragZone))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
