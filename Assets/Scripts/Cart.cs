using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cart : MonoBehaviour {

    public Transform pointToGetUp;
    public GameObject projector;
    public Vector2 screenPos;
    public bool onScreen;
    public bool selected = false;
    public bool pressed = false;

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
}