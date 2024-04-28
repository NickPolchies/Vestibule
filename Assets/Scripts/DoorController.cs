using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : Interactable
{
    PortalController portal;
    Animator animator;
    bool open;

    void Start()
    {
        portal = GetComponent<PortalController>();
        animator = GetComponent<Animator>();
        open = false;
    }

    public override void Interact()
    {
        if (open)
        {
            animator.SetTrigger("Close");
            open = false;
        }
        else
        {
            animator.SetTrigger("Open");
            open = true;
        }
    }
}
