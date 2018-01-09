using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(CharacterStatsEnm))]
public class EnemyAI : MonoBehaviour {

    // Remember to change the ragdoll colliders to Raycast Ignore layer.
    [Header("Target")]
    public CharacterStats target;
    public float sightDistance = 20;
    Vector3 lastKnownPosition;
    FieldOfView fov;

    // General Behaviours variables.
    [Header("General Behaviours")]
    public float delayTillNewBehaviour = 3;
    float _timerTillNewBehaviour;

    // Alert variabls.
    [Header("Alert")]
    public bool onPatrol;
    public bool canChase;
    public int indexBehaviour = 0;
    public List<WaypointsBase> onAlertExtraBehaviours = new List<WaypointsBase>();

    // Searching variables
    [Header("Search")]
    public bool decideBehaviour;
    public float decideBehviourThreshold;
    public float alertMultiplier;
    public List<Transform> possibleHidingPlaces = new List<Transform>();
    public List<Vector3> positionsAroundUnit = new List<Vector3>();
    bool getPossibleHidingPositions;
    bool populateListOfPositions;
    bool searchAtPositions;
    bool createSearchPositions;
    int indexSearchPositions;
    bool searchHidingSpots;
    Transform targetHidingSpot;

    bool lookAtPOI;
    bool initAlert;
    Vector3 pointOfInterest;

    // Check for each WP.
    bool _initCheck;
    bool _lookAtTarget;
    bool _overrideAnimation;

    // Wait time for WP.
    [Header("Wait time for Waypoint")]
    public bool circularList;
    bool descendingList;
    public float _waitTime;

    // Look rotation.
    Quaternion targetRot;

    // Waypoints main.
    [Header("Waypoints main")]
    bool goToPos = false;
    public int indexWaypoints = 0;
    public List<WaypointsBase> waypoints = new List<WaypointsBase>();
    public string[] alertLogic;

    // States.
    [Header("States")]
    public AIStates aiStates;

    public enum AIStates
    {
        patrol,
        chase,
        alert,
        onAlertBehaviours,
        hasTarget,
        search,
        attack
    }

    // Components.
    EnemyControl enmControl;
    NavMeshAgent agent;
    CharacterStatsEnm charStatEnm;
    EnemyManager enManager;

	// Use this for initialization
	void Start () {
        enmControl = GetComponent<EnemyControl>();
        agent = GetComponent<NavMeshAgent>();
        charStatEnm = GetComponent<CharacterStatsEnm>();
        fov = GetComponent<FieldOfView>();
        charStatEnm.alert = false;

        enManager = FindObjectOfType<EnemyManager>();
        enManager.allEnemies.Add(charStatEnm);

        if (onPatrol)
        {
            canChase = true;
            enManager.enemiesOnPatrol.Add(charStatEnm);
        }

        if (canChase)
        {
            enManager.enemiesAvailableToChase.Add(charStatEnm);
        }

        sightDistance = GetComponent<FieldOfView>().viewRadius;
	}
	
	// Update is called once per frame
	void Update () {
        switch (aiStates)
        {
            case AIStates.patrol:
                alertMultiplier = 1;
                DecreaseAlertLevels();
                PatrolBehaviour();
                TargetAvailable();
                break;
            case AIStates.chase:
                ChaseBehaviour();
                TargetAvailable();
                break;
            case AIStates.onAlertBehaviours:
                OnAlertExtraBehaviours();
                TargetAvailable();
                break;
            case AIStates.alert:
                AlertBehaviourMain();
                TargetAvailable();
                break;
            case AIStates.hasTarget:
                HasTargetBehaviour();
                break;
            case AIStates.search:
                alertMultiplier = 0.3f;
                TargetAvailable();
                DecreaseAlertLevels();
                SearchBehaviour();
                break;
            case AIStates.attack:
                AttackBehaviour();
                break;
        }
	}

    public void ChangeAIBehaviour(string behaviour, float delay)
    {
        Invoke(behaviour, delay);
    }

    #region AIStatas

    void AI_State_Normal()
    {
        aiStates = AIStates.patrol;
        target = null;
        charStatEnm.alert = false;
        goToPos = false;
        lookAtPOI = false;
        _initCheck = false;
    }

    void AI_State_Chase()
    {
        aiStates = AIStates.chase;
        goToPos = false;
        lookAtPOI = false;
        _initCheck = false;
    }

    void AI_State_OnAlert_RunListOfBehaviours()
    {
        aiStates = AIStates.onAlertBehaviours;
        charStatEnm.run = true;
        goToPos = false;
        lookAtPOI = false;
        _initCheck = false;
    }

    void AI_State_HasTarget()
    {
        aiStates = AIStates.hasTarget;
        charStatEnm.alert = true;
        goToPos = false;
        lookAtPOI = false;
        _initCheck = false;
    }

    void AI_State_Search()
    {
        aiStates = AIStates.search;
        target = null;
        goToPos = false;
        lookAtPOI = false;
        _initCheck = false;
    }

    void AI_State_Attack()
    {
        aiStates = AIStates.attack;
    }

    #endregion

    void PatrolBehaviour()
    {
        if(waypoints.Count > 0)
        {
            WaypointsBase curWaypoint = waypoints[indexWaypoints];

            if (!goToPos)
            {
                charStatEnm.MoveToPosition(curWaypoint.targetDestination.position);
                goToPos = true;
            }
            else
            {
                float dstToTarget = Vector3.Distance(transform.position, curWaypoint.targetDestination.position);

                if(dstToTarget < enmControl.stopDistance)
                {
                    CheckWaypoint(curWaypoint, 0);
                }
            }
        }
    }

    void CheckWaypoint(WaypointsBase wp, int listCase)
    {
        #region InitialCheck
        if (!_initCheck)
        {
            _lookAtTarget = wp.lookTorwards;
            _overrideAnimation = wp.overrideAnimations;
            _initCheck = true;
        }
        #endregion

        #region WaitTimeSwitch
        if (!wp.stopList)
        {
            switch (listCase)
            {
                case 0:
                    WaitTimerForEachWP(wp, waypoints);
                    break;
                case 1:
                    WaitTimerForExtraBehaviours(wp, onAlertExtraBehaviours);
                    break;
            }
        }
        #endregion

        #region LookTowards
        if (_lookAtTarget)
        {
            Vector3 direction = wp.targetToLookTo.position - transform.position;
            direction.y = 0;

            float angle = Vector3.Angle(transform.forward, direction);
            if (angle > 0.1f)
            {
                targetRot = Quaternion.LookRotation(direction);
                transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRot, Time.deltaTime * wp.speedToLook);
            }
            else
            {
                _lookAtTarget = false;
            }
        }
        #endregion

        #region OverrideAnimations
        if (_overrideAnimation)
        {
            if(wp.animationRoutines.Length > 0)
            {
                for (int i = 0; i < wp.animationRoutines.Length; i++)
                {
                    charStatEnm.CallFunctionWithStrings(wp.animationRoutines[i], 0);
                }
            }
            else
            {
                Debug.Log("Warning! Animation override check but there's no routines assigned!");
            }
            _overrideAnimation = false;
        }
        #endregion
    }

    public void GoOnAlert(Vector3 poi)
    {
        this.pointOfInterest = poi;
        aiStates = AIStates.alert;
        lookAtPOI = false;
    }

    void ChaseBehaviour()
    {
        if(target == null)
        {
            if (!goToPos)
            {
                charStatEnm.MoveToPosition(lastKnownPosition);
                charStatEnm.run = true;
                goToPos = true;
            }
        }
        else
        {
            charStatEnm.MoveToPosition(target.transform.position);
            charStatEnm.run = true;
        }

        if (fov.visibleUnits == null)
        {
            if (target)
            {
                lastKnownPosition = target.transform.position;
                target = null;
            }
            else
            {
                float distanceFromTagetPosition = Vector3.Distance(transform.position, lastKnownPosition);

                if(distanceFromTagetPosition < 2)
                {
                    _timerTillNewBehaviour += Time.deltaTime;

                    if(_timerTillNewBehaviour > delayTillNewBehaviour)
                    {
                        ChangeAIBehaviour("AI_State_Search", 0);
                        _timerTillNewBehaviour = 0;
                    }
                }
            }
        }
    }

    void AlertBehaviourMain()
    {
        if (!lookAtPOI)
        {
            Vector3 dirToLookTo = pointOfInterest - transform.position;
            dirToLookTo.y = 0;

            float angle = Vector3.Angle(transform.forward, dirToLookTo);

            if(angle > 0.1f)
            {
                targetRot = Quaternion.LookRotation(dirToLookTo);
                transform.localRotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 3);
            }
            else
            {
                lookAtPOI = true;
            }
        }

        _timerTillNewBehaviour += Time.deltaTime;
        if(_timerTillNewBehaviour > delayTillNewBehaviour)
        {
            if(alertLogic.Length > 0)
            {
                ChangeAIBehaviour(alertLogic[0], 0);
            }

            _timerTillNewBehaviour = 0;
        }
    }

    void TargetAvailable()
    {
        if (target)
        {
            if (fov.visibleUnits.Count > 0)
            {
                ChangeAIBehaviour("AI_State_HasTarget", 0);
            }
        }
    }

    void SearchBehaviour()
    {
        if (!decideBehaviour)
        {
            int randomValue = Random.Range(0, 11);

            if(randomValue < decideBehviourThreshold)
            {
                searchAtPositions = true;
                Debug.Log(name + "Searching around unit");
            }
            else
            {
                searchHidingSpots = true;
                Debug.Log(name + "Searching in hiding spots");
            }

            decideBehaviour = true;
        }
        else
        {
            #region SearchAtHidingSpots

            if (searchHidingSpots)
            {
                if (!populateListOfPositions)
                {
                    possibleHidingPlaces.Clear();

                    Collider[] allColliders = Physics.OverlapSphere(transform.position, 20);

                    for (int i = 0; i < allColliders.Length; i++)
                    {
                        if (allColliders[i].GetComponent<HidingSpot>())
                        {
                            possibleHidingPlaces.Add(allColliders[i].transform);
                        }
                    }
                    populateListOfPositions = true;
                }
                else if(possibleHidingPlaces.Count > 0)
                {
                    if (!targetHidingSpot)
                    {
                        int randomValue = Random.Range(0, possibleHidingPlaces.Count);

                        targetHidingSpot = possibleHidingPlaces[randomValue];
                    }
                    else
                    {
                        charStatEnm.MoveToPosition(targetHidingSpot.position);

                        Debug.Log(name + "Going to hiding spot");

                        float distanceToTarget = Vector3.Distance(transform.position, targetHidingSpot.position);

                        if(distanceToTarget < 2)
                        {
                            _timerTillNewBehaviour += Time.deltaTime;

                            if(_timerTillNewBehaviour > delayTillNewBehaviour)
                            {
                                // Do Things and reset
                                populateListOfPositions = false;
                                targetHidingSpot = null;
                                decideBehaviour = false;
                                _timerTillNewBehaviour = 0;
                            }
                        }
                    }
                }
                else    //else if(possibleHidingPlaces.Count > 0)
                {
                    // No hiding spots found near unit, search at positions instead
                    Debug.Log(name + "No hiding spots found, cancel it and search at positions instead");
                    searchAtPositions = true;
                    populateListOfPositions = false;
                    targetHidingSpot = null;
                }
            }
            #endregion

            #region SearchAtPositions

            if (searchAtPositions)
            {
                if (!createSearchPositions)
                {
                    positionsAroundUnit.Clear();

                    int randomValue = Random.Range(4, 10);

                    for (int i = 0; i < randomValue; i++)
                    {
                        float offsetX = Random.Range(-10, 10);
                        float offsetZ = Random.Range(-10, 10);

                        Vector3 origionPosition = transform.position;
                        origionPosition += new Vector3(offsetX, 0, offsetZ);

                        NavMeshHit hit;

                        if(NavMesh.SamplePosition(origionPosition,out hit, 5, NavMesh.AllAreas))
                        {
                            positionsAroundUnit.Add(hit.position);
                        }
                    }

                    if(positionsAroundUnit.Count > 0)
                    {
                        indexSearchPositions = 0;
                        createSearchPositions = true;
                        // else try again until you finf one.
                    }
                }
                else
                {
                    Vector3 targetPosition = positionsAroundUnit[indexSearchPositions];

                    Debug.Log(name + "Going to position");

                    charStatEnm.MoveToPosition(targetPosition);

                    float distanceToPosition = Vector3.Distance(transform.position, targetPosition);

                    if(distanceToPosition < 2)
                    {
                        int randomValue = Random.Range(0, 11);
                        decideBehaviour = (randomValue < 5);

                        if(indexSearchPositions < positionsAroundUnit.Count - 1)
                        {
                            _timerTillNewBehaviour += Time.deltaTime;

                            if(_timerTillNewBehaviour > delayTillNewBehaviour)
                            {
                                indexSearchPositions++;
                                _timerTillNewBehaviour = 0;
                            }
                        }
                        else
                        {
                            _timerTillNewBehaviour += Time.deltaTime;

                            if(_timerTillNewBehaviour > delayTillNewBehaviour)
                            {
                                indexSearchPositions = 0;
                                _timerTillNewBehaviour = 0;
                            }
                        }
                    }
                }
            }
            #endregion
        }
    }

    void AttackBehaviour()
    {
        if (fov.visibleUnits.Count > 0)
        {
            if (!goToPos)
            {
                LookAtTarget(target.transform.position);
                charStatEnm.MoveToPosition(target.transform.position);
                //charStatEnm.aim = true;
                charStatEnm.run = true;
                goToPos = true;
            } 
        }
        else
        {
            charStatEnm.aim = false;
            ChangeAIBehaviour("AI_State_Chase", 0);
        }
    }

    void LookAtTarget(Vector3 posToLook)
    {
        Vector3 dirToLookTo = posToLook - transform.position;
        dirToLookTo.y = 0;

        float angle = Vector3.Angle(transform.forward, dirToLookTo);
        if(angle < 0.1f)
        {
            targetRot = Quaternion.LookRotation(dirToLookTo);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRot, Time.deltaTime * 3);
        }
    }

    void OnAlertExtraBehaviours()
    {
        if(onAlertExtraBehaviours.Count > 0)
        {
            WaypointsBase curBehaviour = onAlertExtraBehaviours[indexBehaviour];

            if (!goToPos)
            {
                charStatEnm.MoveToPosition(curBehaviour.targetDestination.position);
                goToPos = true;
            }
            else
            {
                float dstToTarget = Vector3.Distance(transform.position, curBehaviour.targetDestination.position);
                if(dstToTarget < enmControl.stopDistance)
                {
                    CheckWaypoint(curBehaviour, 1);
                }
            }
        }
    }

    void HasTargetBehaviour()
    {
        charStatEnm.StopMoving();

        if (fov.visibleUnits.Count > 0)
        {
            if(charStatEnm.alertLevel < 5)
            {
                float dstToTarget = Vector3.Distance(transform.position, target.transform.position);
                float multiplier = 1 + (dstToTarget * 0.1f);
                // How fast it recognises it's an enemy is based on distance.
                // Can add extra behaviours here, based on the unit's morale/experience/health/difficulty etc.

                alertTimer += Time.deltaTime * multiplier;
                if(alertTimer > alertTimerIncrement)
                {
                    charStatEnm.alertLevel++;
                    alertTimer = 0;
                }
            }
            else
            {
                ChangeAIBehaviour("AI_State_Attack", 0);
            }

            LookAtTarget(lastKnownPosition);
        }
        else
        {
            if(charStatEnm.alertLevel > 5)
            {
                ChangeAIBehaviour("AI_State_Search", 0);
                pointOfInterest = lastKnownPosition;
            }
            else
            {
                _timerTillNewBehaviour += Time.deltaTime;

                if(_timerTillNewBehaviour > delayTillNewBehaviour)
                {
                    ChangeAIBehaviour("AI_State_Normal", 0);
                    _timerTillNewBehaviour = 0;
                }
            }
        }
    }

    float alertTimer;
    float alertTimerIncrement = 1;

    void DecreaseAlertLevels()
    {
        if(charStatEnm.alertLevel > 0)
        {
            alertTimer += Time.deltaTime * alertMultiplier;

            if(alertTimer > alertTimerIncrement * 2)
            {
                charStatEnm.alertLevel--;
                alertTimer = 0;
            }
        }

        if(charStatEnm.alertLevel == 0)
        {
            if(aiStates != AIStates.patrol)
            {
                ChangeAIBehaviour("AI_State_Normal", 0);
            }
        }
    }

    void WaitTimerForEachWP(WaypointsBase wp, List<WaypointsBase> listOfWp)
    {
        if(listOfWp.Count > 1)
        {
            #region WaitTime
            _waitTime += Time.deltaTime;

            if (_waitTime > wp.waitTime)
            {
                if (circularList)
                {
                    if (waypoints.Count - 1 > indexWaypoints)
                    {
                        indexWaypoints++;
                    }
                    else
                    {
                        indexWaypoints = 0;
                    }
                }
                else
                {
                    if (!descendingList)
                    {
                        if (waypoints.Count - 1 == indexWaypoints)
                        {
                            descendingList = true;
                            indexWaypoints--;
                        }
                        else
                        {
                            indexWaypoints++;
                        }
                    }
                    else
                    {
                        if (indexWaypoints > 0)
                        {
                            indexWaypoints--;
                        }
                        else
                        {
                            descendingList = false;
                            indexWaypoints++;
                        }
                    }
                }
                _initCheck = false;
                goToPos = false;
                _waitTime = 0;
            }

            #endregion
        }
    }

    void WaitTimerForExtraBehaviours(WaypointsBase wp, List<WaypointsBase> listOfWp)
    {
        if (listOfWp.Count > 1)
        {
            #region WaitTime
            _waitTime += Time.deltaTime;

            if (_waitTime > wp.waitTime)
            {
                if (circularList)
                {
                    if (waypoints.Count - 1 > indexBehaviour)
                    {
                        indexBehaviour++;
                    }
                    else
                    {
                        indexBehaviour = 0;
                    }
                }
                else
                {
                    if (!descendingList)
                    {
                        if (waypoints.Count - 1 == indexBehaviour)
                        {
                            descendingList = true;
                            indexBehaviour--;
                        }
                        else
                        {
                            indexBehaviour++;
                        }
                    }
                    else
                    {
                        if (indexBehaviour > 0)
                        {
                            indexBehaviour--;
                        }
                        else
                        {
                            descendingList = false;
                            indexBehaviour++;
                        }
                    }
                }
                _initCheck = false;
                goToPos = false;
                _waitTime = 0;
            }

            #endregion
        }
    }
}

[System.Serializable]
public struct WaypointsBase
{
    public Transform targetDestination;
    public float waitTime;
    public bool lookTorwards;
    public Transform targetToLookTo;
    public float speedToLook;
    public bool overrideAnimations;
    public string[] animationRoutines;
    public bool stopList;
}