﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script is for all the controllable units in the game, whether they are walkable or not.
/// </summary>
public class ControllableUnit : MonoBehaviour {

    public Vector2 screenPos;
    public bool onScreen;
    public bool selected = false;
	
	void Update () {
        if (!selected)
        {
            screenPos = Camera.main.WorldToScreenPoint(transform.position);
            if (MouseManager.UnitsWithinScreenSpace(screenPos))
            {
                if (!onScreen)
                {
                    MouseManager.unitsOnScreen.Add(gameObject.GetComponent<CharacterStats>());
                    onScreen = true;
                }
            }
            else
            {
                if (onScreen)
                {
                    MouseManager.RemoveFromOnScreenUnit(gameObject.GetComponent<CharacterStats>());
                    onScreen = false;
                }
            }
        }
        else
        {
            gameObject.GetComponent<CharacterStats>().selected = true;
        }
	}
}
