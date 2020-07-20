using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class V3PowerPoleWayPointObject : V3WayPointObject
{
    public GameObject wirePrefab;
    public Transform wires;
    private Transform wireDestinations;
    private float wireLength = 0;

    public override void Awake()
    {
        base.Awake();
        wires = transform.Find("StartPoints");
        if (wirePrefab != null)
        {
            wireLength = wirePrefab.GetComponent<Renderer>().bounds.size.z;
        }
    }

    private void UpdateWireLength(Transform startPos, Transform endPos)
    {
        float distToOther = Vector3.Distance(startPos.position, endPos.position);
        Vector3 tmpScale = Vector3.one;
        tmpScale.z *= distToOther / Mathf.Max(0.0001f, wireLength);
        startPos.transform.localScale = tmpScale;
    }

    private void UpdateWires()
    {
        for (int i = 0; i < wires.childCount; i++)
        {
            Transform wire = wires.GetChild(i);
            Transform wireDest = wireDestinations.GetChild(i);
            wire.LookAt(wireDest);
            UpdateWireLength(wire, wireDest);
        }
    }

    public override void UpdateSelf()
    {       
        PointToPrevious(false);
        if (nextObj != null)
        {
            wires.gameObject.SetActive(true);
            wireDestinations = nextObj.transform.Find("EndPoints");
            UpdateWires();
        }
        else
        {
            wires.gameObject.SetActive(false);
        }
    }
}
