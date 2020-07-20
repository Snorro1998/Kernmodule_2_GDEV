using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class V3WayPointObject : MonoBehaviour
{
    [HideInInspector]
    public V3WayPointSystem wpSys;
    [HideInInspector]
    public int indexInWayPointSystem = -1;
    [HideInInspector]
    public V3WayPointObject prevObj = null;
    [HideInInspector]
    public V3WayPointObject nextObj = null;

    private Vector3 lastPosition;

    /// <summary>
    /// Zal waar zijn als de positie is verandert.
    /// </summary>
    private bool PositionChanged
    {
        get
        {
            return lastPosition != transform.position;
        }
    }

    public virtual void Awake()
    {
        if (wpSys != null && indexInWayPointSystem != -1)
        {
            wpSys.ReAddDeletedWayPoint(this, indexInWayPointSystem);
        }
    }

    private void OnDestroy()
    {
        if (wpSys != null && indexInWayPointSystem != -1)
        {
            wpSys.RemoveWayPoint(this, indexInWayPointSystem);
        }
    }

    private void Update()
    {
        if (PositionChanged && wpSys != null)
        {
            lastPosition = transform.position;
            wpSys.UpdateEverything();
        }
    }

    public void PointToPrevious(bool tilt)
    {
        // Hij is niet de eerste.
        if (prevObj != null)
        {
            Vector3 tmpPos = prevObj.transform.position;
            if (!tilt)
            {
                tmpPos.y = transform.position.y;
            }
            transform.LookAt(tmpPos);
        }
        // Hij is de eerste maar niet de enige.
        else if (nextObj != null)
        {
            transform.forward = nextObj.transform.forward;
        }
    }

    /// <summary>
    /// Werkt zichzelf bij.
    /// </summary>
    public virtual void UpdateSelf()
    {
        PointToPrevious(true);
    }
}
