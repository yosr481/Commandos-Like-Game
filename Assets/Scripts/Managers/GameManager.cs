using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    [Header("Choosing Variables")]
    public CharacterStats selectedUnit;
    public static ArrayList selectedUnits = new ArrayList();
    public int playerTeam;
    public GameObject unitControl;
    public bool doubleClick;
    public bool overUIElement;

    [Header("Camera Movment")]
    public GameObject cameraMover;
    public float cameraSpeed = 0.3f;

    MouseManager mouseManager;

    void Start()
    {
        mouseManager = GetComponent<MouseManager>();
    }

    void Update()
    {
        if (!overUIElement)
            HandleSelection();

        bool hasUnit = selectedUnit;
        unitControl.SetActive(hasUnit);

        HandleCameraMovment();
    }

    void HandleSelection()
    {
        if (Input.GetMouseButtonUp(0))
        {
            if (!mouseManager.userIsDragging)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, 100))
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

    void HandleCameraMovment()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 newPos = new Vector3(horizontal, 0, vertical) * cameraSpeed;
        cameraMover.transform.position += newPos;
    }

    void CheckHit(RaycastHit hit)
    {
        if (hit.transform.GetComponent<CharacterStats>())
        {
            CharacterStats hitStats = hit.transform.GetComponent<CharacterStats>();
            HandleSelectedUnitsArray(hitStats);

            //if(selectedUnit == null)
            //{
            //    selectedUnit = hitStats;
            //    selectedUnit.selected = true;
            //}
            //else
            //{
            //    selectedUnit.selected = false;
            //    selectedUnit = hitStats;
            //    selectedUnit.selected = true;
            //}
        }
        else
        {
            if (selectedUnits.Count > 0)
            {
                if (doubleClick)
                {
                    foreach(CharacterStats charStat in selectedUnits)
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
                //selectedUnit.MoveToPosition(hit.point);
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
            if(Input.GetKey(KeyCode.LeftControl))
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
        if(selectedUnits.Count > 0)
        {
            for (int i = 0; i < selectedUnits.Count; i++)
            {
                CharacterStats charStat = selectedUnits[i] as CharacterStats;
                charStat.selected = false;
                selectedUnits.Remove(charStat);
            }
        }

        //if(selectedUnit != null)
        //{
        //    selectedUnit.selected = false;
        //    selectedUnit = null;
        //}
    }

    bool UnitIsAlreadyInSelectedUnits(CharacterStats charStat)
    {
        foreach (CharacterStats currentCharStat in selectedUnits)
        {
            if(currentCharStat == charStat)
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

    public void EnterUIElement()
    {
        overUIElement = true;
    }

    public void ExitUIElement()
    {
        overUIElement = false;
    }

    public void ChangeStance()
    {
        if (selectedUnit)
        {
            selectedUnit.run = false;
            selectedUnit.crouch = !selectedUnit.crouch;
        }
    }
}
