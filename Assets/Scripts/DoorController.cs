using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : Interactable
{
    [SerializeField] private DoorController frontDoor;
    [SerializeField] private DoorController backDoor;

    [SerializeField] private DoorController linkedDoor;
    private PortalController portal;
    private Animator animator;
    private bool isOpen;
    private Camera playerCamera;

    private bool IsOpen { get { return isOpen; } }

    void Start()
    {
        portal = GetComponent<PortalController>();
        animator = GetComponent<Animator>();
        playerCamera = Camera.main;
        isOpen = false;
        portal?.Disable();
    }

    public override void Interact()
    {

        if (isOpen && (linkedDoor == null ? true : linkedDoor.IsOpen))
        {
            linkedDoor?.Close();
            Close();
        }
        else if (!isOpen && (linkedDoor == null ? true : !linkedDoor.isOpen))
        {
            updateLinkedDoor();
            linkedDoor?.Open();
            Open();
        }
    }

    public void Open()
    {
        animator.SetTrigger("Open");
        isOpen = true;
        portal?.Enable();
    }

    public void Close()
    {
        animator.SetTrigger("Close");
        isOpen = false;
        portal?.Disable();
    }

    private void updateLinkedDoor()
    {
        if (frontDoor != null && playerIsInFront())
        {
            linkedDoor = frontDoor;
            frontDoor.linkedDoor = this;
        }
        else if (backDoor != null && !playerIsInFront())
        {
            linkedDoor = backDoor;
            frontDoor.linkedDoor = this;
        }
    }

    private bool playerIsInFront()
    {
        return Vector3.Dot(transform.forward, playerCamera.transform.position - transform.position) > 0;
    }
}
