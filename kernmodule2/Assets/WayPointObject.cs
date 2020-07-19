using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class WayPointObject : MonoBehaviour
{
    public WayPointSystem wpSys = null;
    public Transform nextObj = null;
    public Transform prevObj = null;
    private Vector3 lastPos;

    private bool hasMoved
    {
        get { return lastPos != transform.position; }
    }

    public virtual void OnEnable()
    {
        lastPos = transform.position;
    }

    private void OnDestroy()
    {
        int i = wpSys.elements.IndexOf(this);
        Debug.Log("weg, index was " + i);
        //wpSys.elements.Remove(this);
        //wpSys.UpdateNamesAfterIndex();
    }

    private void Update()
    {
        if (wpSys == null)
        {
            GameObject gm = transform.root.gameObject;
            WayPointSystem wp = gm.GetComponent<WayPointSystem>();
            if (wp == null)
            {
                return;
            }
            wpSys = wp;
        }

        //wanneer het object verplaatst is
        if (hasMoved)
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
        //wanneer hij niet het laatste element is
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
        else if (prevObj != null)
        {
            transform.forward = prevObj.forward;
        }
    }

    public virtual void UpdateSelf()
    {
        PointToNext(true);
    }
}