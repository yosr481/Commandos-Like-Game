using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySightSphere : MonoBehaviour {

    EnemyAI enAI;
    CharacterStatsEnm charStats;

    List<CharacterStats> trackingTargets = new List<CharacterStats>();

	// Use this for initialization
	void Start () {
        enAI = GetComponentInParent<EnemyAI>();
        charStats = GetComponentInParent<CharacterStatsEnm>();
    }
	
	// Update is called once per frame
	void Update () {
        //if(enAI.target == null) // This will make the AI to stop searching if it has a target
        //{                       // For multiple player units however this is not enough
            for (int i = 0; i < trackingTargets.Count; i++)
            {
                if (trackingTargets[i] != enAI.target)
                {
                    Vector3 direction = trackingTargets[i].transform.position - transform.position;
                    float angleTowardsTarget = Vector3.Angle(transform.parent.forward, direction.normalized);

                    if (angleTowardsTarget < charStats.viewAngleLimit)
                    {
                        enAI.target = trackingTargets[i];
                    }
                }
                else
                {
                    continue;
                }
            }
        //}
	}

    void OnTriggerEnter(Collider coll)
    {
        if (coll.GetComponent<CharacterStats>())
        {
            CharacterStats otherChar = coll.GetComponent<CharacterStats>();
            if (!trackingTargets.Contains(otherChar))
            {
                trackingTargets.Add(otherChar);
            }
        }
    }

    void OnTriggerExit(Collider coll)
    {
        if (coll.GetComponent<CharacterStats>())
        {
            CharacterStats leavingChar = coll.GetComponent<CharacterStats>();

            if (trackingTargets.Contains(leavingChar))
            {
                trackingTargets.Remove(leavingChar);
            }
        }
    }
}
