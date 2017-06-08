using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(CharacterStatsEnm))]
public class EnemyAI : MonoBehaviour {

    // Remember to change the ragdoll colliders to Raycast Ignore layer.
    [Header("Search")]
    public CharacterStats target;
    public float sightDistance = 20;
    Vector3 lastKnownPosition;

    // General Behaviours variabls.
    [Header("General Behaviours")]
    public float delayTillNewBehaviour = 3;
    float _timerTillNewBehaviour;

    // Alert variabls.
    [Header("Alert")]
    public bool onPatrol;
    public bool canChase;
    public int indexBehaviour = 0;
    public List<WaypointsBase> onAlertExtraBehaviours = new List<WaypointsBase>();

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
        charStatEnm.alert = false;

        enManager = GameObject.FindObjectOfType<EnemyManager>();
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

        sightDistance = GetComponentInChildren<EnemySightSphere>().GetComponent<SphereCollider>().radius;
	}
	
	// Update is called once per frame
	void Update () {
        switch (aiStates)
        {
            case AIStates.patrol:
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
            case AIStates.attack:
                AttackBehaviour();
                break;
        }
	}

    public void ChangeAIBehaviour(string behaviour, float delay)
    {
        Invoke(behaviour, delay);
    }

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

    void AI_State_Attack()
    {
        aiStates = AIStates.attack;
    }

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
        #region InitCheck
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

        if (!sightRaycast())
        {
            if (target)
            {
                lastKnownPosition = target.transform.position;
                target = null;
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
            if (sightRaycast())
            {
                ChangeAIBehaviour("AI_State_HasTarget", 0);
            }
        }
    }

    bool sightRaycast()
    {
        bool retVal = false;
        RaycastHit hitTowardsLowerBody;
        RaycastHit hitTowardsUpperBody;
        float raycastDst = sightDistance + (sightDistance * 0.5f);
        Vector3 targetPos = lastKnownPosition;

        if (target)
        {
            targetPos = target.transform.position;
        }

        Vector3 raycastStart = transform.position + new Vector3(0, 1.6f, 0);
        Vector3 direction = targetPos - raycastStart;

        LayerMask excludeLayer = ~((1 << 9) | (1 << 10));   // Exclude ragdoll layers and enemies 

        Debug.DrawRay(raycastStart, direction + new Vector3(0, 1, 0));
        if(Physics.Raycast(raycastStart, direction + new Vector3(0, 1, 0), out hitTowardsLowerBody, raycastDst, excludeLayer))
        {
            if (hitTowardsLowerBody.transform.GetComponent<CharacterStats>())
            {
                if (target)
                {
                    if(hitTowardsLowerBody.transform.GetComponent<CharacterStats>() == target)
                    {
                        retVal = true;
                    }
                }
            }

            //Debug.Log("Lower raycast: " + hitTowardsLowerBody.transform.name);
        }

        if (!retVal)
        {
            direction += new Vector3(0, 1.6f, 0);

            if (Physics.Raycast(raycastStart, direction + new Vector3(0, 1, 0), out hitTowardsUpperBody, raycastDst, excludeLayer))
            {
                if (target)
                {
                    if(hitTowardsUpperBody.transform == target.transform)
                    {
                        if (!target.crouch)
                        {
                            /* Instead of checking how big is the collidr,
                             * we can simply check if the player is crouching.
                             */
                            retVal = true;
                            //Debug.Log("Upper found");
                        }
                    }
                }

                //Debug.Log("Upper raycast: " + hitTowardsUpperBody.transform.name);
            }
        }

        if (retVal)
        {
            lastKnownPosition = target.transform.position;
        }

        return retVal;
    }

    void AttackBehaviour()
    {
        if (sightRaycast())
        {
            LookAtTarget(lastKnownPosition);
            charStatEnm.aim = true;
            // Add shooting behaviour.
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

        if (sightRaycast())
        {
            if(charStatEnm.alertLevel < 10)
            {
                float dstToTarget = Vector3.Distance(transform.position, target.transform.position);
                float multiplier = 1 + (dstToTarget * 0.1f);
                // How fast it recognises it's an enemy is based on distance.
                // Can add extr behaviours here, based on the unit's morale/experience/health/difficulty etc.

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
                ChangeAIBehaviour("AI_State_Chase", 0);
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
            alertTimer += Time.deltaTime;

            if(alertTimer > alertTimerIncrement * 2)
            {
                charStatEnm.alertLevel--;
                alertTimer = 0;
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
