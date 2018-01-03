using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointOfInterest : MonoBehaviour {

    public bool creatPintOfInterest;

    [SerializeField]
    List<CharacterStatsEnm> affectedChars = new List<CharacterStatsEnm>();

	void Update () {
        if (creatPintOfInterest)
        {
            for (int i = 0; i < affectedChars.Count; i++)
            {
                affectedChars[i].ChangeToAlert(transform.position);
            }

            creatPintOfInterest = false;
        }
	}

    void OnTriggerEnter(Collider coll)
    {
        if (coll.GetComponent<CharacterStatsEnm>())
        {
            if (!affectedChars.Contains(coll.GetComponent<CharacterStatsEnm>()))
            {
                affectedChars.Add(coll.GetComponent<CharacterStatsEnm>());
            }
        }
    }

    void OnTriggerExit(Collider coll)
    {
        if (coll.GetComponent<CharacterStatsEnm>())
        {
            if (affectedChars.Contains(coll.GetComponent<CharacterStatsEnm>()))
            {
                affectedChars.Remove(coll.GetComponent<CharacterStatsEnm>());
            }
        }
    }
}
