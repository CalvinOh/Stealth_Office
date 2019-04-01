using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    
    public float moveSpeed = 6;

    Rigidbody rb;
    Camera viewCamera;
    Vector3 velocity;

    public Light spotlight;
    public float viewDistance;
    public LayerMask viewMask;
    float viewAngle;
    Color originalSpotlightColour;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        viewCamera = Camera.main;

        viewAngle = spotlight.spotAngle;
        originalSpotlightColour = spotlight.color;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mousePos = viewCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, viewCamera.transform.position.y));
        transform.LookAt(mousePos + Vector3.up * transform.position.y);
        velocity = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized * moveSpeed;
    }
    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + velocity * Time.deltaTime);
    }

    void OnDrawGizmos()
    {

        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * viewDistance);
    }
}
