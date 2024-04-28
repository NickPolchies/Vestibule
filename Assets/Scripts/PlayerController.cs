using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : PortalTraveller
{
    Camera cam;
    CharacterController controller;
    public float mouseSensitivity, movementSpeed, runMultiplier, gravity, jumpHeight, groundDistance;
    private float cameraAngle, verticalVelocity, reset;
    [SerializeField]private bool jumped, grounded;
    public Transform groundCheck;
    public LayerMask groundMask;
    public float interactDistance;
    public LayerMask interactMask;

    void Start()
    {
        cam = Camera.main;
        Cursor.lockState = CursorLockMode.Locked;
        cameraAngle = 0f;
        controller = GetComponent<CharacterController>();
        grounded = true; //TODO remove
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width/2, Screen.height/2, 0));
        Debug.DrawRay(ray.origin, ray.direction*2, Color.red);
        UpdateActions();
        UpdateFacing();
        UpdateMovement();
    }

    void UpdateActions()
    {
        if (Input.GetButtonDown("Interact"))
        {
            Ray r = cam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
            RaycastHit hit;

            if(Physics.Raycast(r, out hit, interactDistance, interactMask))
            {
                Interactable interactable = hit.collider.GetComponentInParent<Interactable>();
                if (interactable != null)
                {
                    interactable.Interact();
                }
            }
        }
    }

    void UpdateFacing()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        cameraAngle -= mouseY;
        cameraAngle = Mathf.Clamp(cameraAngle, -90f, 90f);

        transform.Rotate(transform.up, mouseX);
        cam.transform.localRotation = Quaternion.Euler(cameraAngle, 0, 0);
    }

    void UpdateMovement()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        jumped = Input.GetButtonDown("Jump");
        grounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        Vector3 movement = transform.right * x + transform.forward * y * (Input.GetButton("Run") ? runMultiplier : 1);
        movement = movement.magnitude > 1 ? movement.normalized * (Input.GetButton("Run") ? runMultiplier : 1) : movement;

        verticalVelocity = grounded && verticalVelocity < 0 ? -1f : verticalVelocity + gravity * Time.deltaTime;
        
        if(jumped && grounded)
        {
            verticalVelocity = jumpHeight;
//            movement.y += ;
        }

        movement.y += verticalVelocity;
        controller.Move(movement * movementSpeed * Time.deltaTime);

        if(transform.position.y < -500)
        {
            reset = 3;
            transform.position = new Vector3(-0f, 500f, -4f);
            Physics.SyncTransforms();
        }
        if (reset > 0)
        {
            reset -= Time.deltaTime;
        }
    }

    public override void Teleport(Transform fromPortal, Transform toPortal, Vector3 pos, Quaternion rot)
    {
        transform.position = pos;
        transform.rotation = rot;
        Physics.SyncTransforms();
    }
}
