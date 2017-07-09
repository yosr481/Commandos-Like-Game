using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cart : MonoBehaviour {

    public Transform pointToGetUp;
    public Transform seat;

	void OnTriggerEnter(Collider coll)
    {
        if (coll.GetComponent<CharacterStats>())
        {
            coll.transform.position = seat.position + Vector3.up;
        }
    }
}