﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerPoleWayPointObject : WayPointObject
{
    public GameObject wirePrefab;
    private Transform wires;
    private Transform wireDestinations;
    private float wireLength = 0;

    public override void OnEnable()
    {
        base.OnEnable();
        wires = transform.Find("StartPoints");
        if (wirePrefab != null)
        {
            if (wireLength == 0)
            {
                wireLength = wirePrefab.GetComponent<Renderer>().bounds.size.z;
            }
        }     
    }

    private void UpdateWireLength(Transform startPos, Transform endPos)
    {
        float distToOther = Vector3.Distance(startPos.position, endPos.position);
        Vector3 tmpScale = Vector3.one;
        //Ik weet niet wat het probleem is maar er komt soms 0 uit. Vandaar dat er een minimumwaarde is
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
        PointToNext(false);
        if (nextObj != null)
        {
            wires.gameObject.SetActive(true);
            wireDestinations = nextObj.Find("EndPoints");
            UpdateWires();
        }
        else
        {
            wires.gameObject.SetActive(false);
        }
    }
}
