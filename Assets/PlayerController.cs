using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour {

    public GameObject selectedProjector;
    public float stopDistance = 2;
    public float walkSpeed = 2;

    [HideInInspector]
    public bool selected;
    [HideInInspector]
    public Vector3 hitPoint;

    NavMeshAgent navMesh;
    Animator anim;

    private void Awake()
    {
        navMesh = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
    }

    // Use this for initialization
    void Start () {
        selectedProjector.SetActive(false);
        navMesh.updateRotation = false;
        navMesh.isStopped = true;
    }
	
	// Update is called once per frame
	void Update () {
        // Handle selection vizualization.
        if (selected)
            selectedProjector.SetActive(true);
        else
            selectedProjector.SetActive(false);

        //Handle animations when stop.
        if(hitPoint != null)
        {
            float distanceToStop = Vector3.Distance(transform.position, hitPoint);
            if (distanceToStop <= stopDistance)
            {
                navMesh.updateRotation = false;
                navMesh.isStopped = true;
                anim.SetBool("Walk", false);
            }
                
        }
        
    }

    public void MoveToPosition(Vector3 point)
    {
        anim.SetBool("Walk", true);
        navMesh.speed = walkSpeed;
        navMesh.updateRotation = true;
        navMesh.isStopped = false;
        navMesh.SetDestination(point);
    }
}
