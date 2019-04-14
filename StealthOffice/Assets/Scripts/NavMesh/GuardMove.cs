using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GuardMove : MonoBehaviour
{
    public enum States
    {
        Chase,
        Patrol,
        Catch,
    }


    [SerializeField]
    Transform target;
    [SerializeField]
    LayerMask layerMask;
    [SerializeField]
    Transform center;
    [SerializeField]
    float radius;
    [SerializeField]
    States curState;

    NavMeshAgent _navMeshAgent;
    public Collider[] sphereCollider;

    [SerializeField]
    bool _patrolWaiting;
    [SerializeField]
    float _totalWaitTime = 3.0f;

    ConnectedWaypoint _currentWaypoint;
    ConnectedWaypoint _previousWaypoint;

    bool _travelling;
    bool _waiting;
    float _waitTimer;
    int _waypointsVisited;

    // Start is called before the first frame update
    void Start()
    {
        
        _navMeshAgent = this.GetComponent<NavMeshAgent>();

        if(_navMeshAgent == null)
        {
            Debug.Log("NavMesh Not Found");
        }
        else
        {
            if (_currentWaypoint == null)
            {
                GameObject[] allWaypoints = GameObject.FindGameObjectsWithTag("Waypoint");
                if (allWaypoints.Length > 0)
                {
                    while (_currentWaypoint == null)
                    {
                        int random = UnityEngine.Random.Range(0, allWaypoints.Length);
                        ConnectedWaypoint startingWaypoint = allWaypoints[random].GetComponent<ConnectedWaypoint>();

                        if (startingWaypoint != null)
                        {
                            _currentWaypoint = startingWaypoint;
                        }
                    }
                }
            }
            else
            {
                Debug.LogError("Failed to find waypoint");
            }
        }
        curState = States.Patrol;
        Patrolling();
    }

    bool IsPlayerWithinHearing()
    {
        sphereCollider = Physics.OverlapSphere(center.position, radius, layerMask);
        if (sphereCollider.Length != 0)
        {
            return true;
        }
       
        return false;
    }

    private void Update()
    {
        StatesUpdate();
       
    }
    void StatesUpdate()
    {
        switch (curState)
        {
            case States.Patrol: UpdatePatrolState(); break;
            case States.Chase: UpdateChaseState(); break;
            case States.Catch: UpdateCatchState(); break;
        }

    }

    private void UpdateCatchState()
    {
        Debug.Log("Caught, Game Over");
    }

    private void UpdateChaseState()
    {
        if (IsPlayerWithinHearing())
        {
            if (target != null)
            {
                _navMeshAgent.speed = 5.0f;
                Vector3 targetVector = target.transform.position;
                _navMeshAgent.SetDestination(targetVector);
            }
        }
        else
            curState = States.Patrol;
    }

    private void UpdatePatrolState()
    {
        _navMeshAgent.speed = 3.5f;
        if (_travelling && _navMeshAgent.remainingDistance < 1.0f)
        {
            _travelling = false;
            _waypointsVisited++;

            if (_patrolWaiting)
            {
                _waiting = true;
                _waitTimer = 0f;
            }
            else
            {
                Patrolling();
            }
        }

        if (_waiting)
        {
            _waitTimer += Time.deltaTime;
            if (_waitTimer >= _totalWaitTime)
            {
                _waiting = false;

                Patrolling();
            }
        }
        if (IsPlayerWithinHearing())
            curState = States.Chase;
    }

    private void Patrolling()
    {
        if (_waypointsVisited > 0)
        {
            ConnectedWaypoint nextWaypoint = _currentWaypoint.NextWaypoint(_previousWaypoint);
            _previousWaypoint = _currentWaypoint;
            _currentWaypoint = nextWaypoint;
        }

        Vector3 targetVector = _currentWaypoint.transform.position;
        _navMeshAgent.SetDestination(targetVector);
        _travelling = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(center.position, radius);

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            curState = States.Catch;
        }
        else
            curState = States.Chase;
    }
}
