using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class V3WayPointObject : MonoBehaviour
{
    public V3WayPointSystem wpSys;
    public int indexInWayPointSystem = -1;
    public V3WayPointObject prevObj = null;
    public V3WayPointObject nextObj = null;

    private Vector3 lastPosition;

    bool PositionChanged
    {
        get
        {
            return lastPosition != transform.position;
        }
    }

    private void Awake()
    {
        if (wpSys != null && indexInWayPointSystem != -1)
        {
            wpSys.ReAddDeletedWayPoint(this, indexInWayPointSystem);
        }
    }

    private void OnDestroy()
    {
        //Debug.Log("ondest");
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

    public void UpdateSelf()
    {
        if (prevObj != null)
        {
            transform.LookAt(prevObj.transform);
        }
        else if (nextObj != null)
        {
            transform.forward = nextObj.transform.forward;
        }
    }
}
