using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cart : MonoBehaviour {

    public Transform pointToGetUp;
    public Transform centerOfMass;
    public GameObject rider;

    [Header("Selection")]
    public GameObject projector;
    public Vector2 screenPos;
    public bool onScreen;
    public bool selected = false;
    public bool pressed = false;

    [Header("Wheels")]
    public WheelCollider[] wheels = new WheelCollider[4];

    void Start()
    {
        Brake();
    }

    void Update()
    {
        if (!selected)
        {
            screenPos = Camera.main.WorldToScreenPoint(transform.position);
            if (MouseManager.UnitsWithinScreenSpace(screenPos))
            {
                if (!onScreen)
                {
                    onScreen = true;
                }
            }
            else
            {
                if (onScreen)
                {
                    onScreen = false;
                }
            }
        }

        projector.SetActive(selected);
    }

    public void Ride()
    {
        foreach(WheelCollider wc in wheels)
        {
            wc.motorTorque = 0.5f;
        }
    }

    public void Brake()
    {
        foreach (WheelCollider wc in wheels)
        {
            wc.motorTorque = 0;
            wc.brakeTorque = 2;
        }
    }
}