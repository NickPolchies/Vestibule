using UnityEngine;

public class PortalTraveller : MonoBehaviour
{
    public Vector3 previousOffsetFromPortal { get; set; }

    public virtual void Teleport (Transform fromPortal, Transform toPortal, Vector3 pos, Quaternion rot)
    {
        Debug.Log("TELEPORT!!!");
        transform.position = pos;
        transform.rotation = rot;
    }

    //Called entering a portal
    public virtual void EnterPortalThreshold()
    {

    }

    //Called on exiting a portal
    public virtual void ExitPortalThreshold()
    {

    }
}
