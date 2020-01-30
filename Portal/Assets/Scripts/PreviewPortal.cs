using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewPortal : MonoBehaviour
{
    private Transform mPlayerCamera;
    public LayerMask mLayerMask;
    public List<Transform> mValidPoints;

    public bool isValidPosition()
    {
        if (mPlayerCamera == null)
            mPlayerCamera = GameObject.Find("Player").transform.GetChild(1).GetChild(0);

        float lNormal;
        float lDistance;
        int lCounter = 0;
        foreach (var lValidPoint in mValidPoints)
        {
            Ray lRay = new Ray(mPlayerCamera.position, lValidPoint.position - mPlayerCamera.position);

            RaycastHit lRaycastHit;

            if (Physics.Raycast(lRay, out lRaycastHit, 200, mLayerMask.value))
            {
                lNormal = Vector3.Angle(transform.forward, lRaycastHit.normal);
                lDistance = Vector3.Distance(lRaycastHit.point, lValidPoint.transform.position);


                if (!lRaycastHit.collider.CompareTag("Portal Friendly"))
                    return false;
                if (lNormal > 10)
                    return false;
                if (lDistance > 1)
                    return false;

                lCounter++;
            }
        }

        return mValidPoints.Count == lCounter;
    }
}
