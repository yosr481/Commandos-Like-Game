using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Horse : MonoBehaviour {
    public float speed = 20;
    [HideInInspector]
    public bool playerAtGetUpPoint = false;
    public Transform getDownPoint;

    Camera cam;
    CameraManager camManager;
    Cart cart;
    Animator anim;
    Rigidbody rigid;

    float vertical;

	// Use this for initialization
	void Start () {
        cam = FindObjectOfType<Camera>();
        camManager = cam.gameObject.GetComponentInParent<CameraManager>();
        cart = FindObjectOfType<Cart>();
        anim = GetComponent<Animator>();
        rigid = cart.gameObject.GetComponent<Rigidbody>();

    }
	
	// Update is called once per frame
	void FixedUpdate () {
        if (playerAtGetUpPoint)
        {
            camManager.canMove = false;
            camManager.MoveToPosition(transform.position);
            cam.orthographicSize = 16;
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            if(vertical > 0)
            {
                transform.Translate(Vector3.forward * vertical * speed * Time.deltaTime);
                anim.SetBool("RidingMode", true);
                cart.Ride();
            }           
            else
            {
                cart.Brake();
                anim.SetBool("RidingMode", false);
            }   

            if (Input.GetKey(KeyCode.Escape))
            {
                CharacterStats[] allPlayers = FindObjectsOfType<CharacterStats>();
                foreach(CharacterStats cs in allPlayers)
                {
                    if (cs.isRiding)
                    {
                        cs.isRiding = false;
                    }
                }

                camManager.MoveToPosition(new Vector3(0, 0, 0));
                playerAtGetUpPoint = false;
            }
        }
        else
        {
            camManager.canMove = true;
            anim.SetBool("RidingMode", false);
        }
	}
}
