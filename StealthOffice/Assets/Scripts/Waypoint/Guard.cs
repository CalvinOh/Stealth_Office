﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guard : MonoBehaviour
{
    // Start is called before the first frame update
    public enum FSMState
    {
        Patrol,
        Chase,
    }

    public FSMState curState;


    public float speed = 5;
    public float chaseSpeed = 2;
    public float waitTime = .3f;
    public float turnSpeed = 90;

    public Light spotlight;
    public float viewDistance;
    public LayerMask viewMask;
    float viewAngle;

    public Transform pathHolder;
    GameObject player;
    Color originalSpotlightColour;

    Vector3[] waypoints;

    Vector3 desPos;

    Animator animator;
    void Start()
    {
        curState = FSMState.Patrol;
        player = GameObject.FindGameObjectWithTag("Player");
        viewAngle = spotlight.spotAngle;
        originalSpotlightColour = spotlight.color;
        animator = GetComponent<Animator>();
        animator.SetBool("IsSpotted", true);
        waypoints = new Vector3[pathHolder.childCount];
        for (int i = 0; i < waypoints.Length; i++)
        {
            waypoints[i] = pathHolder.GetChild(i).position;
            waypoints[i] = new Vector3(waypoints[i].x, transform.position.y, waypoints[i].z);
        }

        FSMUpdate();
    }

    void FSMUpdate()
    {
        switch (curState)
        {
            case FSMState.Patrol: UpdatePatrolState(); break;
            case FSMState.Chase: UpdateChaseState(); break;
        }

    }

    private void UpdateChaseState()
    {
       
       
        if (CanSeePlayer() == false)
        {
            curState = FSMState.Patrol;
           
        }
        else
        {
            if(this.transform.position != desPos)
            {

                this.transform.LookAt(player.transform);
                desPos = player.transform.position;
                transform.Translate(Vector3.forward * Time.deltaTime * chaseSpeed);
            }
        }

       
    }

    private void UpdatePatrolState()
    {
        StartCoroutine(FollowPath(waypoints));
        
    
    }

    void Update()
    {

        if (CanSeePlayer())
        {
            spotlight.color = Color.red;
            StopCoroutine(FollowPath(waypoints));
            curState = FSMState.Chase;
            FSMUpdate();
        }
        else
        {
            spotlight.color = originalSpotlightColour;
            
        }
    }

    bool CanSeePlayer()
    {
        if (Vector3.Distance(transform.position, player.transform.position) < viewDistance)
        {
            Vector3 dirToPlayer = (player.transform.position - transform.position).normalized;
            float angleBetweenGuardAndPlayer = Vector3.Angle(transform.forward, dirToPlayer);
            if (angleBetweenGuardAndPlayer < viewAngle / 2f)
            {
                if (!Physics.Linecast(transform.position, player.transform.position, viewMask))
                {
                    return true;
                }
            }
        }
        return false;
    }
    IEnumerator FollowPath(Vector3[] waypoints)
    {
        transform.position = waypoints[0];

        int targetWaypointIndex = 1;
        Vector3 targetWaypoint = waypoints[targetWaypointIndex];
        transform.LookAt(targetWaypoint);

        while (true)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetWaypoint, speed * Time.deltaTime);
    
           
            if (transform.position == targetWaypoint)
            {
                targetWaypointIndex = (targetWaypointIndex + 1) % waypoints.Length;
                targetWaypoint = waypoints[targetWaypointIndex];
                yield return new WaitForSeconds(waitTime);
                yield return StartCoroutine(TurnToFace(targetWaypoint));

            }
            yield return null;
        }
    }

    IEnumerator TurnToFace(Vector3 lookTarget)
    {
        Vector3 dirToLookTarget = (lookTarget - transform.position).normalized;
        float targetAngle = 90 - Mathf.Atan2(dirToLookTarget.z, dirToLookTarget.x) * Mathf.Rad2Deg;

        while (Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, targetAngle)) > 0.05f)
        {
            float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetAngle, turnSpeed * Time.deltaTime);
            transform.eulerAngles = Vector3.up * angle;
            yield return null;
        }
    }

    void OnDrawGizmos()
    {
        Vector3 startPosition = pathHolder.GetChild(0).position;
        Vector3 previousPosition = startPosition;

        foreach (Transform waypoint in pathHolder)
        {
            Gizmos.DrawSphere(waypoint.position, .3f);
            Gizmos.DrawLine(previousPosition, waypoint.position);
            previousPosition = waypoint.position;
        }
        Gizmos.DrawLine(previousPosition, startPosition);

        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * viewDistance);
    }
}
