using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : Interactable
{
    [SerializeField] private DoorController linkedDoor;
    private PortalController portal;
    private Animator animator;
    private bool isOpen;

    private bool IsOpen { get; }

    void Start()
    {
        portal = GetComponent<PortalController>();
        animator = GetComponent<Animator>();
        isOpen = false;
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
            linkedDoor?.Open();
            Open();
        }
    }

    public void Open()
    {
        animator.SetTrigger("Open");
        isOpen = true;
    }

    public void Close()
    {
        animator.SetTrigger("Close");
        isOpen = false;
    }
}
