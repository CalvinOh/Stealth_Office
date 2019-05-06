using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFPS : MonoBehaviour
{
    [SerializeField] private string horizontalInputName;
    [SerializeField] private string verticalInputName;
    [SerializeField] private float movementSpeed;

    private CharacterController charController;

    public Light spotlight;
    public LayerMask viewMask;
    float viewAngle;
    public bool SpotLightOn;

    SphereCollider sphereCollider;
    float radius;
    private void Awake()
    {
        charController = GetComponent<CharacterController>();
        sphereCollider = GetComponent<SphereCollider>();
        radius = sphereCollider.radius;
        viewAngle = spotlight.spotAngle;
        SpotLightOn = true;
    }

    private void Update()
    {
        PlayerMovement();

        if (Input.GetKeyDown("f"))
        {
            if (SpotLightOn)
            {
                spotlight.enabled = false;
                SpotLightOn = false;
            }
            else
            {
                spotlight.enabled = true;
                SpotLightOn = true;
            }
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            movementSpeed = movementSpeed * 1.5f;
            sphereCollider.radius = 8;
        }
        else if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            movementSpeed = movementSpeed / 1.5f;
            sphereCollider.radius = 6;
        }
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            movementSpeed = movementSpeed * 0.5f;
            sphereCollider.radius = 1;
        }
        else if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            movementSpeed = movementSpeed / 0.5f;
            sphereCollider.radius = 6;
        }

        radius = sphereCollider.radius;
    }

    private void PlayerMovement()
    {
        float horizInput = Input.GetAxis(horizontalInputName) * movementSpeed;
        float vertInput = Input.GetAxis(verticalInputName) * movementSpeed;

        Vector3 forwardMovement = transform.forward * vertInput;
        Vector3 rightMovement = transform.right * horizInput;

        charController.SimpleMove(forwardMovement + rightMovement);


    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(this.transform.position, radius);

    }
}
