using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(FieldOfView))]
public class EnemyControl : MonoBehaviour {

    Animator anim;
    NavMeshAgent agent;
    CharacterStatsEnm charStatEnm;

    public float stopDistance;
    public bool moveToPosition;
    public Vector3 destPosition;

    public bool run;
    public bool crouch;

    public float walkSpeed = 1;
    public float runSpeed = 2;
    public float crouchSpeed = 0.8f;

    public float maxStance = 0.9f;
    public float minStance = 0.1f;
    float targetStance;
    float stance;

	// Use this for initialization
	void Start () {
        anim = GetComponent<Animator>();
        SetupAnimator();
        agent = GetComponent<NavMeshAgent>();
        charStatEnm = GetComponent<CharacterStatsEnm>();
        agent.stoppingDistance = stopDistance - .1f;

        agent.updateRotation = true;
        //Enable when use animations values for stopping.
         agent.angularSpeed = 500;
         agent.autoBraking = false;
        InitRagdoll();
	}
	
	// Update is called once per frame
	void Update () {
        run = charStatEnm.run;

        if (moveToPosition)
        {
            agent.Resume();
            agent.updateRotation = true;
            agent.SetDestination(destPosition);

            float distanceToTarget = Vector3.Distance(transform.position, destPosition);

            if (distanceToTarget <= stopDistance)
            {
                moveToPosition = false;
                charStatEnm.run = false;
            }
        }
        else
        {
            agent.Stop();
            agent.updateRotation = false;
        }

        HandleSpeed();
        HandleAiming();
        HandleAnimations();
        HandleStats();
	}

    void SetupAnimator()
    {
        // This is a ref to the animator component in the root.
        anim = GetComponent<Animator>();

        // We use avatar from a child animator component if present.
        // This is to enable easy swapping of the character model as a child node.
        foreach(var childAnimator in GetComponentsInChildren<Animator>())
        {
            if(childAnimator != anim)
            {
                anim.avatar = childAnimator.avatar;
                Destroy(childAnimator);
                break; // If you find the first animator, stop searching.
            }
        }
    }

    void HandleSpeed()
    {
        if (!run)
        {
            if (!crouch)
            {
                agent.speed = walkSpeed;
            }
            else
            {
                agent.speed = crouchSpeed;
            }
        }
        else
        {
            agent.speed = runSpeed;
        }
    }

    void HandleAiming()
    {
        anim.SetBool("Aim", charStatEnm.aim);
    }

    void HandleAnimations()
    {
        Vector3 relativeDirection = (transform.InverseTransformDirection(agent.desiredVelocity)).normalized;
        float animValue = relativeDirection.z;

        if (!run)
        {
            animValue = Mathf.Clamp(animValue, 0, 0.5f);
        }

        anim.SetFloat("Forward", animValue, 0.3f, Time.deltaTime);
    }

    void HandleStats()
    {
        if (charStatEnm.run)
        {
            targetStance = minStance;
        }
        else
        {
            if (charStatEnm.crouch)
            {
                targetStance = maxStance;
            }
            else
            {
                targetStance = minStance;
            }
        }

        stance = Mathf.Lerp(stance, targetStance, Time.deltaTime * 3);
        anim.SetFloat("Stance", stance);

        anim.SetBool("Alert", charStatEnm.alert);
    }

    void InitRagdoll()
    {
        Rigidbody[] rigB = GetComponentsInChildren<Rigidbody>();
        Collider[] collA = GetComponentsInChildren<Collider>();

        for (int i = 0; i < rigB.Length; i++)
        {
            rigB[i].isKinematic = true;
        }

        for (int x = 0; x < collA.Length; x++)
        {
            if(x != 0)
            {
                collA[x].gameObject.layer = 10;
            }
            collA[x].isTrigger = true;
        }
    }
}
