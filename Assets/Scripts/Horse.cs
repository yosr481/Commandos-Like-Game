using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Horse : MonoBehaviour {
    public float speed = 5;

    Camera camera;
    CameraManager camManager;
    Cart cart;
    Animator anim;
    Rigidbody rigid;

	// Use this for initialization
	void Start () {
        camera = FindObjectOfType<Camera>();
        camManager = camera.gameObject.GetComponentInParent<CameraManager>();
        cart = GetComponentInParent<Cart>();
        anim = GetComponent<Animator>();
        rigid = cart.gameObject.GetComponent<Rigidbody>();

    }
	
	// Update is called once per frame
	void FixedUpdate () {
        if (cart.pressed)
        {
            camManager.canMove = false;
            camManager.MoveToPosition(transform.position + Vector3.up * 25 - Vector3.forward * 10);
            camera.orthographic = false;
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            rigid.velocity = Vector3.forward * vertical * speed;
            anim.SetBool("RidingMode", true);
        }
        else
        {
            camManager.canMove = true;
            anim.SetBool("RidingMode", false);
        }
	}
}
