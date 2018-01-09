using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour {

    public List<CharacterStatsEnm> allEnemies = new List<CharacterStatsEnm>();
    public List<CharacterStatsEnm> enemiesAvailableToChase = new List<CharacterStatsEnm>();
    public List<CharacterStatsEnm> enemiesOnPatrol = new List<CharacterStatsEnm>();

    public bool showBehaviours;
    public bool resetAll;
    public bool universalAlert;
    public bool enyoneWhoCanChase;
    public bool patrolOnly;
    public Transform debugPOI;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (resetAll)
        {
            for (int i = 0; i < allEnemies.Count; i++)
            {
                allEnemies[i].ChangeToNormal();
            }

            resetAll = false;
        }

        if (universalAlert)
        {
            for (int i = 0; i < allEnemies.Count; i++)
            {
                allEnemies[i].ChangeToAlert(debugPOI.position);
            }

            universalAlert = false;
        }

        if (enyoneWhoCanChase)
        {
            for (int i = 0; i < enemiesAvailableToChase.Count; i++)
            {
                enemiesAvailableToChase[i].ChangeToAlert(debugPOI.position);
            }

            enyoneWhoCanChase = false;
        }

        if (patrolOnly)
        {
            for (int i = 0; i < enemiesOnPatrol.Count; i++)
            {
                enemiesOnPatrol[i].ChangeToAlert(debugPOI.position);
            }

            patrolOnly = false;
        }

        if (showBehaviours)
        {
            for (int i = 0; i < allEnemies.Count; i++)
            {
                allEnemies[i].GetComponent<EnemyUI>().EnableDisableUI();
            }

            showBehaviours = false;
        }
    }

    public void UpdateListOfChaseEnemies()
    {
        if(allEnemies.Count > 0)
        {
            for (int i = 0; i < allEnemies.Count; i++)
            {
                if (allEnemies[i].GetComponent<EnemyAI>().canChase)
                {
                    if (!enemiesAvailableToChase.Contains(allEnemies[i]))
                    {
                        enemiesAvailableToChase.Add(allEnemies[i]);
                    }
                }
            }
        }
    }
}
