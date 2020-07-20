using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// Zorgt voor het uitlijnen van objecten.
/// </summary>
public class ObjectAligner
{
    /// <summary>
    /// Verspringt alles naar het huidige geselecteerde object in alle richtingen.
    /// </summary>
    public void AlignAllAxis()
    {
        Transform tTarget = Selection.activeTransform;
        foreach (Transform t in Selection.transforms)
        {
            t.position = tTarget.position;
        }
    }

    /// <summary>
    /// Verspringt alles naar het huidige geselecteerde object in de x-richting.
    /// </summary>
    public void AlignXAxis(List<GameObject> currentSelected)
    {
        Transform tTarget = currentSelected[currentSelected.Count - 1].transform;
        float xPos = tTarget != null ? tTarget.position.x : 0;
        foreach (Transform t in Selection.transforms)
        {
            Vector3 tmpPos = t.position;
            tmpPos.x = xPos;
            t.position = tmpPos;
        }
    }

    /// <summary>
    /// Verspringt alles naar het huidige geselecteerde object in de y-richting.
    /// </summary>
    public void AlignYAxis(List<GameObject> currentSelected)
    {
        Transform tTarget = currentSelected[currentSelected.Count - 1].transform;
        float yPos = tTarget != null ? tTarget.position.y : 0;
        foreach (Transform t in Selection.transforms)
        {
            Vector3 tmpPos = t.position;
            tmpPos.y = yPos;
            t.position = tmpPos;
        }
    }

    /// <summary>
    /// Verspringt alles naar het huidige geselecteerde object in de z-richting.
    /// </summary>
    public void AlignZAxis(List<GameObject> currentSelected)
    {
        Transform tTarget = currentSelected[currentSelected.Count - 1].transform;
        float zPos = tTarget != null ? tTarget.position.z : 0;
        foreach (Transform t in Selection.transforms)
        {
            Vector3 tmpPos = t.position;
            tmpPos.z = zPos;
            t.position = tmpPos;
        }
    }
}
