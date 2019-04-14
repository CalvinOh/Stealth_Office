using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavMeshPlayersScript : MonoBehaviour
{
    public float moveSpeed;

    Rigidbody rb;
    SphereCollider sphereCollider;
    Camera viewCamera;
    Vector3 velocity;
    float radius;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        sphereCollider = GetComponent<SphereCollider>();
        radius = sphereCollider.radius;
        viewCamera = Camera.main;


    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mousePos = viewCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, viewCamera.transform.position.y));
        transform.LookAt(mousePos + Vector3.up * transform.position.y);
        velocity = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized * moveSpeed;
        
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            moveSpeed = moveSpeed * 1.5f;
            sphereCollider.radius = 8;
        }
        else if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            moveSpeed = moveSpeed / 1.5f;
            sphereCollider.radius = 6;
        }
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            moveSpeed = moveSpeed * 0.5f;
            sphereCollider.radius = 1;
        }
        else if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            moveSpeed = moveSpeed / 0.5f;
            sphereCollider.radius = 6;
        }

        radius = sphereCollider.radius;
    }
    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + velocity * Time.deltaTime);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(this.transform.position, radius);

    }
}
