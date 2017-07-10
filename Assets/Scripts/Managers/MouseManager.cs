using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseManager : MonoBehaviour {

    #region Variables
    public bool userIsDragging = false;
    public GUIStyle mouseDragSkin;
    public static CharacterStatsEnm[] enemies;
    public static ArrayList selectedUnits = new ArrayList();
    public static ArrayList unitsOnScreen = new ArrayList();
    public static ArrayList unitsInDragBox = new ArrayList();

    public bool doubleClick;
    public bool overUIElement;
    bool finishDragOnThisFrame;
    bool startedDrag;

    static float timeLimitBeforDeclareDrag = .12f;
    static float timeLeftBeforeDeclareDrag;
    static Vector2 mouseDragStart;
    static Vector3 mouseDownPoint;
    static Vector3 currentMousePoint;
    static float clickDragZone = 1.3f;
    RaycastHit hit;

    float boxWidth;
    float boxHeight;
    float boxLeft;
    float boxTop;
    static Vector2 boxStart;
    static Vector2 boxFinish;
    #endregion

    void Start()
    {
        enemies = GameObject.FindObjectsOfType<CharacterStatsEnm>();
    }


    void Update () {

        if (!overUIElement)
            HandleSelection();

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            currentMousePoint = hit.point;
            CheckControllableObjects(hit);
        }

        #region Mouse drag.
        if (Input.GetMouseButtonDown(0))
        {
            mouseDownPoint = hit.point;
            timeLeftBeforeDeclareDrag = timeLimitBeforDeclareDrag;
            mouseDragStart = Input.mousePosition;
            startedDrag = true;
        }
        else if (Input.GetMouseButton(0))
        {
            if (!userIsDragging)
            {
                timeLeftBeforeDeclareDrag -= Time.deltaTime;
                if(timeLeftBeforeDeclareDrag <= 0 /*|| UserDraggingByPosition(mouseDragStart, Input.mousePosition)*/)
                {
                    userIsDragging = true;
                }
            }

            if (userIsDragging)
            {
                boxWidth = Camera.main.WorldToScreenPoint(mouseDownPoint).x - Camera.main.WorldToScreenPoint(currentMousePoint).x;
                boxHeight = Camera.main.WorldToScreenPoint(mouseDownPoint).y - Camera.main.WorldToScreenPoint(currentMousePoint).y;

                boxLeft = Input.mousePosition.x;
                boxTop = (Screen.height - Input.mousePosition.y) - boxHeight;

                #region Calculate BoxStart and Finish
                if (FloatToBool(boxWidth))
                {
                    if (FloatToBool(boxHeight))
                    {
                        boxStart = new Vector2(Input.mousePosition.x, Input.mousePosition.y + boxHeight);
                    }
                    else
                    {
                        boxStart = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                    }
                }
                else
                {
                    if (FloatToBool(boxHeight)){
                        boxStart = new Vector2(Input.mousePosition.x + boxWidth, Input.mousePosition.y + boxHeight);
                    }
                    else
                    {
                        boxStart = new Vector2(Input.mousePosition.x + boxWidth, Input.mousePosition.y);
                    }
                }
                boxFinish = new Vector2(boxStart.x + Unsigned(boxWidth), boxStart.y - Unsigned(boxHeight));
                #endregion
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (userIsDragging)
            {
                finishDragOnThisFrame = true;
                userIsDragging = false;
            }
            
        }


        if (!Input.GetKey(KeyCode.LeftControl) && startedDrag && userIsDragging)
        {
            DeselcetUnit();
            startedDrag = false;
        }
        #endregion
    }

    void LateUpdate()
    {
        unitsInDragBox.Clear();

        if((userIsDragging || finishDragOnThisFrame) && unitsOnScreen.Count > 0)
        {
            for (int i = 0; i < unitsOnScreen.Count; i++)
            {
                CharacterStats unitObj = unitsOnScreen[i] as CharacterStats;
                ControllableUnit unitScript = unitObj.GetComponent<ControllableUnit>();

                if (!UnitAlreadyInDragBoxUnits(unitObj))
                {
                    if (UnitInsideDragBox(unitScript.screenPos))
                    {
                        unitObj.selected = true;
                        unitsInDragBox.Add(unitObj);
                        Debug.Log(unitsInDragBox.Count);
                    }
                    else
                    {
                        if (!UnitIsAlreadyInSelectedUnits(unitObj))
                        {
                            unitObj.selected = false;
                        }
                        unitScript.selected = false;
                    }
                }
            }
        }

        if (finishDragOnThisFrame)
        {
            finishDragOnThisFrame = false;
            PutDraggedUnitsInSelectedUnits();
        }
    }

    void OnGUI()
    {
        if (userIsDragging)
        {
            GUI.Box(new Rect(boxLeft, boxTop, boxWidth, boxHeight), "", mouseDragSkin);
        }
    }

    #region Helper Functions

    void HandleSelection()
    {
        if (Input.GetMouseButtonUp(0))
        {
            if (!userIsDragging)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                {
                    CheckHit(hit);
                }
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            DeselcetUnit();
        }
    }

    void CheckControllableObjects(RaycastHit hit)
    {
        if (hit.transform.gameObject.GetComponent<Cart>())
        {
            Cart currentCart = hit.transform.gameObject.GetComponent<Cart>();
            currentCart.selected = true;
            if (Input.GetMouseButton(0))
            {
                currentCart.pressed = true;
            }
        }
        else
        {
            Cart[] allCarts = GameObject.FindObjectsOfType<Cart>();

            foreach(Cart c in allCarts)
            {
                if(!c.pressed)
                    c.selected = false;
            }
            if (Input.GetMouseButton(0))
            {
                foreach (Cart c in allCarts)
                {
                    c.pressed = false;
                }
            }
        }
    }

    void CheckHit(RaycastHit hit)
    {
        if (hit.transform.GetComponent<CharacterStats>())
        {
            CharacterStats hitStats = hit.transform.GetComponent<CharacterStats>();
            /*if(!hitStats.isBeenCaught)*/
                HandleSelectedUnitsArray(hitStats);
        }
        else if (hit.transform.GetComponent<CharacterStatsEnm>())
        {
            foreach(CharacterStatsEnm charStatEnm in enemies)
            {
                charStatEnm.selected = false;
            }
            hit.transform.GetComponent<CharacterStatsEnm>().selected = true;

        }
        else if (hit.transform.GetComponent<Cart>())
        {
            if(selectedUnits.Count == 1)
            {
                CharacterStats selectedUnit = selectedUnits[0] as CharacterStats;
                Cart currentCart = hit.transform.gameObject.GetComponent<Cart>();

                if (doubleClick)
                {
                    selectedUnit.run = true;
                }
                else
                {
                    doubleClick = true;
                    StartCoroutine("closeDoubleClick");
                }

                selectedUnit.MoveToPosition(currentCart.pointToGetUp.position);
            }  
        }
        else
        {
            if (selectedUnits.Count > 0)
            {
                if (doubleClick)
                {
                    foreach (CharacterStats charStat in selectedUnits)
                    {
                        charStat.run = true;
                    }
                }
                else
                {
                    doubleClick = true;
                    StartCoroutine("closeDoubleClick");
                }

                foreach (CharacterStats charStat in selectedUnits)
                {
                    charStat.MoveToPosition(hit.point);
                }
            }
        }
    }

    IEnumerator closeDoubleClick()
    {
        yield return new WaitForSeconds(1);
        doubleClick = false;
    }

    void HandleSelectedUnitsArray(CharacterStats charStat)
    {
        if (!UnitIsAlreadyInSelectedUnits(charStat))
        {
            if (!Input.GetKey(KeyCode.LeftControl))
            {
                DeselcetUnit();
            }

            charStat.selected = true;
            selectedUnits.Add(charStat);
        }
        else
        {
            if (Input.GetKey(KeyCode.LeftControl))
                RemoveUnitFromSelectedUnit(charStat);
            else
            {
                DeselcetUnit();
                charStat.selected = true;
                selectedUnits.Add(charStat);
            }
        }
    }

    public void DeselcetUnit()
    {
        if (selectedUnits.Count > 0)
        {
            for (int i = 0; i < selectedUnits.Count; i++)
            {
                CharacterStats charStat = selectedUnits[i] as CharacterStats;
                charStat.selected = false;
                charStat.gameObject.GetComponentInParent<ControllableUnit>().selected = false;
            }
            selectedUnits.Clear();
        }
    }

    bool UnitIsAlreadyInSelectedUnits(CharacterStats charStat)
    {
        foreach (CharacterStats currentCharStat in selectedUnits)
        {
            if (currentCharStat == charStat)
            {
                return true;
            }
            return false;
        }
        return false;
    }

    void RemoveUnitFromSelectedUnit(CharacterStats charStat)
    {
        if (selectedUnits.Count > 0)
        {
            foreach (CharacterStats currentCharStat in selectedUnits)
            {
                if (currentCharStat == charStat)
                {
                    selectedUnits.Remove(currentCharStat);
                }
                return;
            }
        }
        return;
    }

    float Unsigned(float var)
    {
        if (var < 0) var *= -1;
        return var;
    }

    bool FloatToBool(float f)
    {
        if (f < 0) return false; else return true;
    }

    public static bool UnitsWithinScreenSpace(Vector2 unitScreenPos)
    {
        if ((unitScreenPos.x < Screen.width && unitScreenPos.y < Screen.height) && (unitScreenPos.x > 0 && unitScreenPos.y  > 0))
            return true;
        else
            return false;
    }

    public static void RemoveFromOnScreenUnit(CharacterStats unit)
    {
        for (int i = 0; i < unitsOnScreen.Count; i++)
        {
            CharacterStats unitObj = unitsOnScreen[i] as CharacterStats;
            if(unit == unitObj)
            {
                unitsOnScreen.RemoveAt(i);
                return;
            }
        }
        return;
    }

    public static bool UnitInsideDragBox(Vector2 unitScreenPos)
    {
        if ((unitScreenPos.x > boxStart.x && unitScreenPos.y < boxStart.y) && (unitScreenPos.x < boxFinish.x && unitScreenPos.y > boxFinish.y))
            return true;
        else
            return false;
    }

    public static bool UnitAlreadyInDragBoxUnits(CharacterStats unit)
    {
            foreach (CharacterStats currentCharStat in selectedUnits)
            {
                if (currentCharStat == unit)
                {
                    return true;
                }
                return false;
            }
        return false;
    }

    public void PutDraggedUnitsInSelectedUnits()
    {
        if (unitsInDragBox.Count > 0)
        {
            for (int i = 0; i < unitsInDragBox.Count; i++)
            {
                CharacterStats unitObj = unitsInDragBox[i] as CharacterStats;

                if (!UnitIsAlreadyInSelectedUnits(unitObj))
                {
                    selectedUnits.Add(unitObj);
                    unitObj.GetComponent<ControllableUnit>().selected = true;
                }
            }

            unitsInDragBox.Clear();
        }
    }

    public bool UserDraggingByPosition(Vector2 dragStartPoint, Vector2 newPoint)
    {
        if (newPoint.x > dragStartPoint.x + clickDragZone || newPoint.x < dragStartPoint.x + clickDragZone ||
            newPoint.y > dragStartPoint.y + clickDragZone || newPoint.y < dragStartPoint.y + clickDragZone)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    #endregion

    #region UI over mouse
    public void EnterUIElement()
    {
        overUIElement = true;
    }

    public void ExitUIElement()
    {
        overUIElement = false;
    }
    #endregion
}