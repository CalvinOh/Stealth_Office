using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarGuard : MonoBehaviour
{
    public enum States
    {
        Chase,
        Alert,
        Idle,
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

    float elapsedTime;

    void Start()
    {
        viewAngle = spotlight.spotAngle;
        originalSpotlightColour = spotlight.color;
        elapsedTime = 5f;
 
    }

    void StateUpdate()
    {
        switch (curState)
        {
            case States.Alert: UpdateAlertState(); break;
            case States.Chase: UpdateChaseState(); break;
            case States.Idle: UpdateIdleState(); break;
        }

        elapsedTime += Time.deltaTime;

    }

    private void UpdateIdleState()
    {
        spotlight.enabled = false;

        if(sentry.GetComponent<SentryScript>().CanSeePlayer())
        {
            curState = States.Chase;
        }
        

    }

    private void UpdateChaseState()
    {
        MoveToTarget();

        if (!sentry.GetComponent<SentryScript>().CanSeePlayer())
        {
            curState = States.Alert;
        }
     
       
    }

    private void UpdateAlertState()
    {
        spotlight.enabled = true;
        spotlight.transform.Rotate(0,1,0,Space.Self);

        if(elapsedTime >= 15f + Time.deltaTime)
        {
            print("Switching to idle mode");
            curState = States.Idle;
        }

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
