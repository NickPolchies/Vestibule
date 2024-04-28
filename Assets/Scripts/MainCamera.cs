using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    List<PortalController> portals;

    void Awake()
    {
        portals = new List<PortalController>();
    }

    public void RegisterNewPortal(PortalController portal)
    {
        portals.Add(portal);
    }

    void OnPreCull()
    {
        foreach(PortalController portal in portals)
        {
            portal.Render();
        }
    }
}
