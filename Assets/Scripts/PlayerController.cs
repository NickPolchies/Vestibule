using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : PortalTraveller
{
    Camera cam;
    CharacterController controller;
    public float mouseSensitivity, movementSpeed, runMultiplier, gravity, jumpHeight, groundDistance;
    private float cameraAngle, verticalVelocity, reset;
    [SerializeField]private bool jumped, grounded, running;
    public Transform groundCheck;
    public LayerMask groundMask;
    public float interactDistance;
    public LayerMask interactMask;
    [SerializeField] InputSystem_Actions inputActions;
    InputAction movementAction;
    InputAction lookAction;

    void Start()
    {
        cam = Camera.main;
        Cursor.lockState = CursorLockMode.Locked;
        cameraAngle = 0f;
        controller = GetComponent<CharacterController>();
        grounded = true; //TODO remove? review for removal?
    }

    void Update()
    {
        Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width/2, Screen.height/2, 0));
        Debug.DrawRay(ray.origin, ray.direction*2, Color.red);
        UpdateActions();
        UpdateFacing();
        UpdateMovement();
    }

    private void OnEnable()
    {
        inputActions = new InputSystem_Actions();
        movementAction = inputActions.Player.Move;
        movementAction.Enable();

        lookAction = inputActions.Player.Look;
        lookAction.Enable();

        inputActions.Player.Attack.performed += Interact;
        inputActions.Player.Attack.Enable();
        inputActions.Player.Interact.performed += Interact;
        inputActions.Player.Interact.Enable();

        inputActions.Player.Jump.performed += Jump;
        inputActions.Player.Jump.Enable();

        inputActions.Player.Sprint.performed += Run;
        inputActions.Player.Sprint.canceled += RunStop;
        inputActions.Player.Sprint.Enable();
    }

    private void OnDisable()
    {
        movementAction.Disable();
        lookAction.Disable();
        inputActions.Player.Attack.Disable();
        inputActions.Player.Interact.Disable();
        inputActions.Player.Jump.Disable();
        inputActions.Player.Sprint.Disable();
    }

    void UpdateActions()
    {
    }

    void UpdateFacing()
    {
        Vector2 lookInput = lookAction.ReadValue<Vector2>();

        cameraAngle -= lookInput.y * mouseSensitivity * 0.01f;
        cameraAngle = Mathf.Clamp(cameraAngle, -90f, 90f);

        transform.Rotate(transform.up, lookInput.x * mouseSensitivity * 0.01f);
        cam.transform.localRotation = Quaternion.Euler(cameraAngle, 0, 0);
    }

    void UpdateMovement()
    {
        Vector2 moveInput = movementAction.ReadValue<Vector2>();

        Vector3 movement = transform.right * moveInput.x + transform.forward * moveInput.y * (running ? runMultiplier : 1);
        movement = movement.magnitude > 1 ? movement.normalized * (running ? runMultiplier : 1) : movement;

        verticalVelocity = grounded && verticalVelocity < 0 ? -1f : verticalVelocity + gravity * Time.deltaTime;
        
        movement.y += verticalVelocity;
        controller.Move(movement * movementSpeed * Time.deltaTime);

        if(transform.position.y < -10)
        {
            reset = 3;
            transform.position = new Vector3(-0f, 20f, -4f);
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

    private void Interact(InputAction.CallbackContext ctx)
    {
        Ray r = cam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        RaycastHit hit;

        if (Physics.Raycast(r, out hit, interactDistance, interactMask))
        {
            Interactable interactable = hit.collider.GetComponentInParent<Interactable>();
            if (interactable != null)
            {
                interactable.Interact();
            }
        }
    }

    private void Jump(InputAction.CallbackContext ctx)
    {
        grounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (grounded)
        {
            verticalVelocity = jumpHeight;
        }
    }

    private void Run(InputAction.CallbackContext ctx)
    {
        running = true;
    }

    private void RunStop(InputAction.CallbackContext ctx)
    {
        running = false;
    }
}
