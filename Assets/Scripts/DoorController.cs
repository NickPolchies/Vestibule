using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : Interactable
{
    [SerializeField] private DoorController frontDoor;
    [SerializeField] private DoorController backDoor;

    [SerializeField] private DoorController frontLinkedDoor;
    [SerializeField] private DoorController backLinkedDoor;
    private PortalController portal;
    private Animator animator;
    private bool isOpen;

    private bool IsOpen { get { return isOpen; } }

    void Start()
    {
        portal = GetComponentInChildren<PortalController>();
        animator = GetComponent<Animator>();
        isOpen = false;
        portal?.Disable();
    }

    public override void Interact()
    {
        if (isOpen)
        {
            Close();
        }
        else if (!isOpen)
        {
            Open();
        }
    }

    public void Open()
    {
        if (isOpen)
        {
            return;
        }

        animator.SetTrigger("Open");
        portal?.Enable();
        isOpen = true;

        UpdateLinkedDoor();

        frontLinkedDoor?.Open();
        backLinkedDoor?.Open();
    }

    public void Close()
    {
        if(!isOpen)
        { 
            return;
        }

        animator.SetTrigger("Close");
        portal?.Disable();
        isOpen = false;

        frontLinkedDoor?.Close();
        backLinkedDoor?.Close();
    }

    private void UpdateLinkedDoor()
    {
        frontLinkedDoor = frontDoor;
        backLinkedDoor = backDoor;
        frontDoor.backDoor = this;
        backDoor.frontDoor = this;

        portal.SetFrontPortal(frontDoor.portal);
        portal.SetBackPortal(backDoor.portal);
    }

    private bool PlayerIsInFront()
    {
        return Vector3.Dot(transform.forward, PlayerController.Player.transform.position - transform.position) > 0;
    }
}
