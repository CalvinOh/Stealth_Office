using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] private string horizontalInputName;
    [SerializeField] private string verticalInputName;
    [SerializeField] private float movementSpeed;

    private CharacterController charController;

    public Light spotlight;
    public LayerMask viewMask;
    float viewAngle;
    public bool SpotLightOn;
    private void Awake()
    {
        charController = GetComponent<CharacterController>();

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
    }

    private void PlayerMovement()
    {
        float horizInput = Input.GetAxis(horizontalInputName) * movementSpeed;
        float vertInput = Input.GetAxis(verticalInputName) * movementSpeed;

        Vector3 forwardMovement = transform.forward * vertInput;
        Vector3 rightMovement = transform.right * horizInput;

        charController.SimpleMove(forwardMovement + rightMovement);

   
    }

}
