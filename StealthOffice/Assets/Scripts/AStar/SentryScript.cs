using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SentryScript : MonoBehaviour
{

    public Light spotlight;
    public float viewDistance;
    public LayerMask viewMask;
    float viewAngle;

   
    GameObject player;
    Color originalSpotlightColour;




    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        viewAngle = spotlight.spotAngle;
        originalSpotlightColour = spotlight.color;

    }

  

    void Update()
    {
        RotateSpotlight();
        if (CanSeePlayer())
        {
            spotlight.color = Color.red;
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

    void RotateSpotlight()
    {
        float angle = Mathf.Sin(Time.time) * 70;

        this.transform.rotation = Quaternion.AngleAxis(angle, Vector3.up);
    }
 
    void OnDrawGizmos()
    {
        
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * viewDistance);
    }
}
