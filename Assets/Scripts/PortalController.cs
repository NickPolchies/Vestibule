using System.Collections.Generic;
using UnityEngine;

public class PortalController : MonoBehaviour
{
    [SerializeField] PortalController frontPortal = null, backPortal = null, activePortal = null;
    [SerializeField] MeshRenderer portalMesh = null;
    private List<PortalTraveller> trackedTravellers;
    private Camera playerCamera, portalCamera;
    RenderTexture viewTexture;

    // Start is called before the first frame update
    void Start()
    {
        playerCamera = Camera.main;
        portalCamera = GetComponentInChildren<Camera>(true);
        portalCamera.enabled = false;
        playerCamera.GetComponent<MainCamera>().RegisterNewPortal(this);
        trackedTravellers = new List<PortalTraveller>();
        //portalMesh = transform.Find("PortalSurface").GetComponent<MeshRenderer>();
    }

    void LateUpdate()
    {
        HandleTravellers();
    }

    void HandleTravellers()
    {
        for (int i = 0; i < trackedTravellers.Count; i++)
        {
            PortalTraveller traveller = trackedTravellers[i];
            Transform travellerTransform = traveller.transform;
            Matrix4x4 portalLocationOffset = activePortal.transform.localToWorldMatrix * transform.worldToLocalMatrix * travellerTransform.localToWorldMatrix;

            Vector3 offsetFromPortal = travellerTransform.position - transform.position;
            int portalSide = System.Math.Sign(Vector3.Dot(offsetFromPortal, transform.forward));
            int portalSideOld = System.Math.Sign(Vector3.Dot(traveller.previousOffsetFromPortal, transform.forward));

            //Teleport if player crosses portal
            //TODO seems to activate when crossing backwards on a one-way portal. Fix this bug
            if (portalSide != portalSideOld)
            {
                Vector3 positionOld = travellerTransform.position;
                Quaternion rotationOld = travellerTransform.rotation;

                traveller.Teleport(transform, activePortal.transform, portalLocationOffset.GetColumn(3), portalLocationOffset.rotation);
                //traveller.graphicsClone.transform.SetPositionAndRotation(positionOld, rotationOld);

                activePortal.OnTravellerEnterPortal(traveller);
                trackedTravellers.RemoveAt(i);

                CalculateActivePortal();
                activePortal?.CalculateActivePortal();
            }
            else
            {
                //traveller.graphicsClone.transform.SetPositionAndRotation(portalLocationOffset.GetColumn(3), portalLocationOffset.rotation);
                //UpdateSliceParams(traveller);
                traveller.previousOffsetFromPortal = offsetFromPortal;
            }
        }
    }

    public void Render()
    {
        CalculateActivePortal();

        //TODO Split code out into function
        //Checks if portal surface is in view frustum
        Plane[] t = GeometryUtility.CalculateFrustumPlanes(playerCamera);
        if (!GeometryUtility.TestPlanesAABB(t, portalMesh.bounds) || activePortal == null)
        {
            portalMesh.enabled = false; //TODO Test this
            return;
        }

        portalMesh.enabled = true; //TODO test this
        activePortal.portalMesh.enabled = false;
        CreateViewTexture();

        Matrix4x4 m = activePortal.transform.localToWorldMatrix * transform.worldToLocalMatrix * playerCamera.transform.localToWorldMatrix;
        portalCamera.transform.SetPositionAndRotation(m.GetColumn(3), m.rotation);

        portalCamera.Render();

        activePortal.portalMesh.enabled = true;

        PreventScreenClipping(); //TODO see definition
    }

    void CreateViewTexture()
    {
        if(viewTexture == null || viewTexture.width != Screen.width || viewTexture.height != Screen.height)
        {
            if(viewTexture != null)
            {
                viewTexture.Release();
            }
            viewTexture = new RenderTexture(Screen.width, Screen.height, 0);
            portalCamera.targetTexture = viewTexture;
            portalMesh.material.SetTexture("_MainTex", viewTexture);
        }
    }

    void OnTravellerEnterPortal (PortalTraveller traveller)
    {
        if (!trackedTravellers.Contains(traveller))
        {
            traveller.EnterPortalThreshold();
            traveller.previousOffsetFromPortal = traveller.transform.position - transform.position;
            trackedTravellers.Add(traveller);
        }
    }

    void OnTriggerEnter (Collider other)
    {
        PortalTraveller traveller = other.GetComponent<PortalTraveller>();
        if (traveller)
        {
            OnTravellerEnterPortal(traveller);
        }
    }

    void OnTriggerExit(Collider other)
    {
        PortalTraveller traveller = other.GetComponent<PortalTraveller>();
        if(traveller && trackedTravellers.Contains(traveller))
        {
            traveller.ExitPortalThreshold();
            trackedTravellers.Remove(traveller);
        }
    }

    //* TODO fix this up to allow an FOV slider
    void PreventScreenClipping()
    {
        float halfHeight = playerCamera.nearClipPlane * Mathf.Tan(playerCamera.fieldOfView * 0.5f * Mathf.Deg2Rad);
        float halfWidth = halfHeight * playerCamera.aspect;
        float nearPlaneCornerDistance = new Vector3(halfWidth, halfHeight, playerCamera.nearClipPlane).magnitude;

        Transform portalSuface = portalMesh.transform;
        bool camFacingPortal = Vector3.Dot(transform.forward, transform.position - playerCamera.transform.position) > 0;
        portalSuface.localScale = new Vector3(portalSuface.localScale.x, portalSuface.localScale.y, nearPlaneCornerDistance);
        portalSuface.localPosition = -Vector3.forward * nearPlaneCornerDistance * ((camFacingPortal) ? -0.5f : 0.5f);
    }
    //*/

    public void CalculateActivePortal()
    {
        float playerPosDotProduct = Vector3.Dot(transform.forward, playerCamera.transform.position - transform.position);
        activePortal = playerPosDotProduct > 0 ? frontPortal : backPortal;
    }

    public void Disable()
    {
        enabled = false;
    }

    public void Enable()
    {
        enabled = true;
    }

    public void SetFrontPortal(PortalController portal)
    {
        frontPortal = portal;
    }

    public void SetBackPortal(PortalController portal)
    {
        backPortal = portal;
    }
}
