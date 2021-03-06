﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SentryScript : MonoBehaviour
{
    public enum States
    {
        Scanning, 
        Homing,
    }

    [SerializeField]
    Vector3 startingAngle;
    [SerializeField]
    Vector3 endingAngle;
    [SerializeField]
    float frequency;

    public Light spotlight;
    public float viewDistance;
    public LayerMask viewMask;
    float viewAngle;

   
    GameObject player;
    Color originalSpotlightColour;

    //protected Vector3 m_from = new Vector3(0, 90.0f, 0);
    //protected Vector3 m_to = new Vector3(0, 180.0f, 0);
    //protected float m_frequency = 0.4f;

    public States curState;

    float lastSeenTime;
    [SerializeField]
    float waitTime = 4f;
    
    

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        viewAngle = spotlight.spotAngle;
        originalSpotlightColour = spotlight.color;

        lastSeenTime = Time.time + waitTime;

    }

    void StateUpdate()
    {
        switch (curState)
        {
            case States.Homing: UpdateHomingState(); break;
            case States.Scanning: UpdateScanningState(); break;
        }


    }

    private void UpdateScanningState()
    {
        
        if (CanSeePlayer())
        {
            spotlight.color = Color.red;
            curState = States.Homing;
        }
        else
        {
            RotateSpotlight();
            spotlight.color = originalSpotlightColour;

        }

        lastSeenTime = Time.time + waitTime;
    }

    private void UpdateHomingState()
    {
       if(!CanSeePlayer() && Time.time > lastSeenTime)
       {
           curState = States.Scanning;
          
        }
       else
       {
           transform.LookAt(player.transform);
           spotlight.color = Color.red;
            Debug.Log("Target Locked On. Deploying Hound");
        }



    }

    void Update()
    {
        StateUpdate();
       
    }

    public bool CanSeePlayer()
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

    void RotateSpotlight()
    {

        Quaternion from = Quaternion.Euler(this.startingAngle);
        Quaternion to = Quaternion.Euler(this.endingAngle);

        float lerp = 0.5f * (1.0f + Mathf.Sin(Mathf.PI * Time.realtimeSinceStartup * this.frequency));
        this.transform.localRotation = Quaternion.Lerp(from, to, lerp);

    }
 
    void OnDrawGizmos()
    {
        
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * viewDistance);
    }
}
