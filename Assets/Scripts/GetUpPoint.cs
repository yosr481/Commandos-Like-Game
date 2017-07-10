using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetUpPoint : MonoBehaviour {

    public Transform seat;

    void OnTriggerEnter(Collider coll)
    {
        if (coll.GetComponent<CharacterStats>())
        {
            coll.GetComponent<CharacterStats>().StartRiding();
        }
    }
}
