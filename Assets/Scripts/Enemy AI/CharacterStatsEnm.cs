using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyControl))]
public class CharacterStatsEnm : MonoBehaviour {

    public float health = 100;
    public int morale;
    public float viewAngleLimit = 50;
    public int alertLevel;
    public int team;
    public bool selected = false;
    public bool run;
    public bool dead;
    public bool crouch;
    public bool alert = true;
    public bool aim;
    public bool shooting;
    public GameObject alertCube;
    public GameObject viewIllustration;
    EnemyControl enControl;
    EnemyAI enAI;

	void Start () {
        health = 100;

        enControl = GetComponent<EnemyControl>();
        enAI = GetComponent<EnemyAI>();
	}
	
	// Update is called once per frame
	void Update () {
        if (run) crouch = false;

        if (alertCube)
        {
            float scale = alertLevel * 0.05f;
            alertCube.transform.localScale = new Vector3(scale, scale, scale);
            if(alertLevel >= 5)
            {
                alertCube.GetComponent<MeshRenderer>().material.color = Color.red;
            }
        }

        if (viewIllustration) viewIllustration.SetActive(selected);
	}

    public void StopMoving()
    {
        enControl.moveToPosition = false;
    }

    public void MoveToPosition(Vector3 pos)
    {
        enControl.moveToPosition = true;
        enControl.destPosition = pos;
    }

    public void ChangeToNormal()
    {
        enAI.ChangeAIBehaviour("AI_State_Normal", 0);
        alert = false;
        crouch = false;
        run = false;
    }

    public void ChangeToAlert(Vector3 poi)
    {
        alert = true;
        enControl.moveToPosition = false;

        enAI.GoOnAlert(poi);
    }

    public void CallFunctionWithStrings(string functionIdentifier, float delay)
    {
        Invoke(functionIdentifier, delay);
    }

    void ChangeStance()
    {
        crouch = !crouch;
    }

    void AlertPhase()
    {
        alert = !alert;
    }

    void ChangeRunState()
    {
        run = !run;
    }
}