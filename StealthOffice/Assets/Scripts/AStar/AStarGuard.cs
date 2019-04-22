using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarGuard : MonoBehaviour
{
    public enum States
    {
        Chase,
        Idle,
        ReturnHome,
    }

    public float speed;
    public GameObject pathfinding;
    public GameObject sentry;
   

    public States curState;

    public Light spotlight;
    public float viewDistance;
    public LayerMask viewMask;
    float viewAngle;
    Color originalSpotlightColour;

    Animator anim;

    float waitTime;
    float timeWaited;

    void Start()
    {
        viewAngle = spotlight.spotAngle;
        originalSpotlightColour = spotlight.color;
        anim = GetComponent<Animator>();

        curState = States.Idle;

        waitTime = 6f;
        timeWaited = Time.time + waitTime;
    }

    void StateUpdate()
    {
        switch (curState)
        {
            case States.Chase: UpdateChaseState(); break;
            case States.Idle: UpdateIdleState(); break;
            case States.ReturnHome: UpdateReturnHomeState(); break;
        }

        

    }

    private void UpdateReturnHomeState()
    {
        Debug.Log("Returning Home");
        pathfinding.GetComponent<Pathfinding>().target = pathfinding.GetComponent<Pathfinding>().homeLocation;

        pathfinding.GetComponent<Pathfinding>().FindPath(pathfinding.GetComponent<Pathfinding>().seeker.position, pathfinding.GetComponent<Pathfinding>().target.position);

        MoveToTarget();

        if (sentry.GetComponent<SentryScript>().CanSeePlayer())
        {
            anim.SetBool("IsSpotted", true);
            curState = States.Chase;
        }
    }

    private void UpdateIdleState()
    {
        spotlight.enabled = false;
  

        if (sentry.GetComponent<SentryScript>().CanSeePlayer())
        {
            anim.SetBool("IsSpotted", true);
            curState = States.Chase;
        }
        if(Time.time > timeWaited)
        {
            anim.SetBool("IsSpotted", false);
            curState = States.ReturnHome;

        }


    }

    private void UpdateChaseState()
    {
        pathfinding.GetComponent<Pathfinding>().target = pathfinding.GetComponent<Pathfinding>().player;

        pathfinding.GetComponent<Pathfinding>().FindPath(pathfinding.GetComponent<Pathfinding>().seeker.position, pathfinding.GetComponent<Pathfinding>().target.position);

        MoveToTarget();

        Debug.Log("Chasing");
        if (!sentry.GetComponent<SentryScript>().CanSeePlayer())
        {
            anim.SetBool("IsSpotted", false);
            curState = States.Idle;
            
        }

        timeWaited = Time.time + waitTime;
    }


    void Update()
    {
        StateUpdate();
    }

    public void MoveToTarget()
    {
        if (pathfinding.GetComponent<Pathfinding>().path.Count > 0)
        {
            transform.LookAt(pathfinding.GetComponent<Pathfinding>().path[0].worldPosition);
            transform.position = Vector3.MoveTowards(transform.position, pathfinding.GetComponent<Pathfinding>().path[0].worldPosition, speed * Time.deltaTime);

            if (Vector3.Distance(transform.position, pathfinding.GetComponent<Pathfinding>().path[0].worldPosition) < 0.4f)
            {
                pathfinding.GetComponent<Pathfinding>().path.RemoveAt(0);
            }
        }
    }


    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * viewDistance);
    }
}
