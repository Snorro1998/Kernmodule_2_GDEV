using System.Collections.Generic;
using UnityEngine;

public class ObjectDuplicator
{
    /// <summary>
    /// POSITIONEREN NOG TOEVOEGEN. Verdubbelt geselecteerde objecten in een bepaalde richting.
    /// </summary>
    /// <param name="dir"></param>
    public void DuplicateInDirection(Vector3 dir, List<GameObject> currentSelected)
    {
        foreach (GameObject gm in currentSelected)
        {
            GameObject gmNew = GameObject.Instantiate(gm, gm.transform.position + dir, gm.transform.rotation);
        }
    }
}
