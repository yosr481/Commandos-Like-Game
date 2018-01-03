using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowPoint : MonoBehaviour
{

    static Vector3 currentMousePoint;
    RaycastHit hit;
    public bool canMove = true;
    ThrowSimulation throwSimulation;

    // Use this for initialization
    void Awake()
    {
        throwSimulation = GetComponentInParent<ThrowSimulation>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity) && canMove)
        {
            currentMousePoint = hit.point;
            transform.position = new Vector3(currentMousePoint.x, -2.5f, currentMousePoint.z);
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            canMove = false;
            StartCoroutine(throwSimulation.SimulateProjectile());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PointOfInterest>())
        {
            StartCoroutine(ReturnToMove());
        }
    }

    IEnumerator ReturnToMove()
    {
        yield return new WaitForSeconds(.5f);

        gameObject.SetActive(false);
    }
}