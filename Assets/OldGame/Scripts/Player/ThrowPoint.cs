using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowPoint : MonoBehaviour
{

    static Vector3 currentMousePoint;
    RaycastHit hit;

    // Update is called once per frame
    void FixedUpdate()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            currentMousePoint = hit.point;
            transform.position = new Vector3(currentMousePoint.x, -2.5f, currentMousePoint.z);
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            Destroy(this);
        }
    }
}