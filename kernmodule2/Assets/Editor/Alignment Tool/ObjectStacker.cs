using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectStacker 
{
    /// <summary>
    /// NOG NIET GEÏMPLEMENTEERD. Stapelt alles in de x-richting.
    /// </summary>
    public void StackXAxis(List<GameObject> currentSelected)
    {

    }

    /// <summary>
    /// Stapelt alles in de y-richting.
    /// </summary>
    public void StackYAxis(List<GameObject> currentSelected)
    {
        for (int i = currentSelected.Count - 1; i > 0; i--)
        {
            Transform currentBottom = currentSelected[i].transform;
            Transform currentTop = currentSelected[i - 1].transform;
            MeshRenderer meshBottom = currentBottom.GetComponent<MeshRenderer>();
            MeshRenderer meshTop = currentTop.GetComponent<MeshRenderer>();

            if (meshBottom != null && meshTop != null)
            {
                Vector3 pos = currentBottom.position;
                pos.y += meshBottom.bounds.extents.y + meshTop.bounds.extents.y;
                currentTop.position = pos;
            }
        }
    }

    /// <summary>
    /// NOG NIET GEÏMPLEMENTEERD. Stapelt alles in de z-richting.
    /// </summary>
    public void StackZAxis(List<GameObject> currentSelected)
    {

    }
}
