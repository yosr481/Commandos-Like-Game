using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseManager : MonoBehaviour {

    public bool userIsDragging = false;
    public GUIStyle mouseDragSkin;

    static float timeLimitBeforDeclareDrag = 1;
    static float timeLeftBeforeDeclareDrag;
    static Vector2 mouseDragStart;
    static Vector3 mouseUpPoint;
    static Vector3 mouseDownPoint;
    static Vector3 currentMousePoint;
    static float clickDragZone = 1.3f;
    RaycastHit hit;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            currentMousePoint = hit.point;
        }

        // Mouse drag.
        if (Input.GetMouseButtonDown(0))
        {
            mouseDownPoint = hit.point;
            timeLeftBeforeDeclareDrag = timeLimitBeforDeclareDrag;
            mouseDragStart = Input.mousePosition;
        }
        else if (Input.GetMouseButton(0))
        {
            if (!userIsDragging)
            {
                timeLeftBeforeDeclareDrag -= Time.deltaTime;
                if(timeLeftBeforeDeclareDrag <= 0 || UserDraggingByPosition(mouseDragStart, Input.mousePosition))
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

    void OnGUI()
    {
        if (userIsDragging)
        {
            float boxWidth = Camera.main.WorldToScreenPoint(mouseDownPoint).x - Camera.main.WorldToScreenPoint(currentMousePoint).x;
            float boxHeight = Camera.main.WorldToScreenPoint(mouseDownPoint).y - Camera.main.WorldToScreenPoint(currentMousePoint).y;

            float boxLeft = Input.mousePosition.x;
            float boxTop = (Screen.height - Input.mousePosition.y) - boxHeight;

            GUI.Box(new Rect(boxLeft, boxTop, boxWidth, boxHeight), "", mouseDragSkin);
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
