using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseBehaviour : MonoBehaviour {

    RaycastHit hit;

    public List<GameObject> selectedPlayers;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                CheckHit(hit);
            }
        }
	}

    void CheckHit(RaycastHit hit)
    {
        if(hit.transform.tag == "Player")
        {
            //selectedPlayers.Clear();
            selectedPlayers.Add(hit.transform.gameObject);
            foreach (GameObject o in selectedPlayers)
            {
                o.GetComponent<PlayerController>().selected = true;
            }
        }
        else
        {
            foreach(GameObject o in selectedPlayers)
            {
                o.GetComponent<PlayerController>().selected = false;
            }
            selectedPlayers.Clear();
        }
    }
}
