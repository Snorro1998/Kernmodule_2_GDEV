using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class WayPointObject : MonoBehaviour
{
    [HideInInspector]
    public WayPointSystem wpSys = null;
    [HideInInspector]
    public Transform nextObj = null;
    [HideInInspector]
    public Transform prevObj = null;
    private Vector3 lastPos;

    public virtual void OnEnable()
    {
        lastPos = transform.position;
    }

    /*
    private void OnDestroy()
    {
        wpSys.CallChildrenUpdateMethods();
    }*/

    private void Update()
    {
        //wanneer het object verplaatst is
        if (lastPos != transform.position)
        {
            wpSys.CallChildrenUpdateMethods();
            lastPos = transform.position;
        }
    }
    
    /// <summary>
    /// Wijst het object naar de eerstvolgende in het waypointsysteem
    /// </summary>
    /// <param name="tilt"></param>
    public void PointToNext(bool tilt)
    {
        if (nextObj != null)
        {
            Vector3 tmpVec = nextObj.position;
            if (!tilt)
            {
                tmpVec.y = transform.position.y;
            }
            transform.LookAt(tmpVec);
        }
        //wijs hem in dezelfde richting als de een na laatste als hij de laatste is
        else
        {
            transform.forward = prevObj.forward;
        }
    }

    public virtual void UpdateSelf(/*Transform tNext, Transform tPrev*/)
    {
        PointToNext(true);
    }
}
