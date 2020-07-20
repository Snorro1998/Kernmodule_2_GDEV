using System.Collections.Generic;
using UnityEngine;

public class ObjectSnapper
{
    /// <summary>
    /// Maakt een object vast aan het dichtsbijzijnde oppervlak in een bepaalde richting.
    /// </summary>
    /// <param name="gm"></param>
    /// <param name="dir"></param>
    public void SnapToSurface(GameObject gm, Vector3 dir)
    {
        MeshRenderer renderer = gm.GetComponent<MeshRenderer>();
        if (renderer != null)
        {
            RaycastHit rayhit;
            if (Physics.Raycast(renderer.bounds.center, dir, out rayhit))
            {
                Vector3 vec = gm.transform.position;
                int tmp = (dir == Vector3.right || dir == Vector3.forward || dir == Vector3.up) ? -1 : 1;

                //x-as
                if (dir == Vector3.right || dir == Vector3.left)
                {
                    float xPos = rayhit.point.x + tmp * renderer.bounds.extents.x;
                    vec = new Vector3(xPos, gm.transform.position.y, gm.transform.position.z);
                }

                //z-as
                else if (dir == Vector3.forward || dir == Vector3.back)
                {
                    float zPos = rayhit.point.z + tmp * renderer.bounds.extents.z;
                    vec = new Vector3(gm.transform.position.x, gm.transform.position.y, zPos);
                }

                //y-as
                else if (dir == Vector3.up || dir == Vector3.down)
                {
                    float distFromCenterOfBoundingBox = renderer.transform.position.y - renderer.bounds.center.y;
                    distFromCenterOfBoundingBox += tmp * renderer.bounds.extents.y;
                    float yPos = rayhit.point.y + distFromCenterOfBoundingBox;
                    vec = new Vector3(gm.transform.position.x, yPos, gm.transform.position.z);
                }

                gm.transform.position = vec;
            }
        }
    }

    /// <summary>
    /// Maakt alle geselecteerde objecten vast aan het dichtsbijzijnde oppervlak in een bepaalde richting.
    /// </summary>
    /// <param name="dir"></param>
    public void SnapAllToSurface(Vector3 dir, List<GameObject> currentSelected)
    {
        foreach (GameObject gm in currentSelected)
        {
            SnapToSurface(gm, dir);
        }
    }
}
