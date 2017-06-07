using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public CharacterStats selectedUnit;
    public static ArrayList selectedUnits = new ArrayList();
    public int playerTeam;
    public GameObject unitControl;
    public bool doubleClick;
    public bool overUIElement;
    public GameObject cameraMover;
    public float cameraSpeed = 0.3f;

    void Start()
    {

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
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if(Physics.Raycast(ray, out hit, 100))
            {
                CheckHit(hit);
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

            if(hitStats.team == playerTeam)
            {
                if(selectedUnit == null)
                {
                    selectedUnit = hitStats;
                    selectedUnit.selected = true;
                }
                else
                {
                    selectedUnit.selected = false;
                    selectedUnit = hitStats;
                    selectedUnit.selected = true;
                }
            }
            else
            {
                if(selectedUnit == null)
                {
                    //Add logic here for enemies FOV
                }
            }
        }
        else
        {
            if (selectedUnit)
            {
                if (doubleClick)
                {
                    selectedUnit.run = true;
                }
                else
                {
                    doubleClick = true;
                    StartCoroutine("closeDoubleClick");
                }

                selectedUnit.MoveToPosition(hit.point);
            }
        }
    }

    IEnumerator closeDoubleClick()
    {
        yield return new WaitForSeconds(1);
        doubleClick = false;
    }

    public void DeselcetUnit()
    {
        selectedUnit.selected = false;
        selectedUnit = null;

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
