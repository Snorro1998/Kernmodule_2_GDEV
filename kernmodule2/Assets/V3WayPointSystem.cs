using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class V3WayPointSystem : MonoBehaviour
{
    private List<V3WayPointObject> elements = new List<V3WayPointObject>();
    [HideInInspector]
    public GameObject currentPrefab = null;
    [HideInInspector]
    public bool loop = false;

    public void SetLoopAround(bool looping)
    {
        loop = looping;
        UpdateEverything();
    }

    public void UpdateNamesAndIndicesFromIndex(int i)
    {
        UpdateEverything();
        /*
        for (int j = i; j < elements.Count; j++)
        {
            V3WayPointObject wo = elements[j];
            wo.name = currentPrefab.name + j;
            wo.indexInWayPointSystem = j;
        }
        */
    }

    public void UpdateEverything()
    {
        Debug.Log("updateeverything");
        for (int j = 0; j < elements.Count; j++)
        {
            V3WayPointObject nObj = j != elements.Count - 1 ? elements[j + 1] : loop ? elements[0] : null;
            V3WayPointObject pObj = j != 0 ? elements[j - 1] : loop ? elements[elements.Count - 1] : null;

            V3WayPointObject wo = elements[j];
            wo.name = currentPrefab.name + j;
            wo.indexInWayPointSystem = j;

            wo.prevObj = pObj;
            wo.nextObj = nObj;

            wo.UpdateSelf();
        }
    }

    public void RemoveWayPoint(V3WayPointObject wo, int index)
    {
        elements.Remove(wo);
        UpdateNamesAndIndicesFromIndex(index);
    }

    public void ReAddDeletedWayPoint(V3WayPointObject wo, int index)
    {
        if (!elements.Contains(wo) /*&& elements.Count > 0 && index > 0 && index < elements.Count - 1*/)
        {
            elements.Insert(index, wo);
            UpdateNamesAndIndicesFromIndex(index + 1);
        }
    }

    public void UpdatePrefabForChildren()
    {
        Debug.Log("updateprefab");
        List<Vector3> positions = new List<Vector3>();
        List<Quaternion> rotations = new List<Quaternion>();

        int i = transform.childCount - 1;
        for (int j = i; j >= 0; j--)
        {
            positions.Add(elements[j].transform.position);
            rotations.Add(elements[j].transform.rotation);
            DestroyImmediate(elements[j].gameObject);
        }
        for (int j = 0; j < i + 1; j++)
        {
            V3WayPointObject wo = CreateWayPoint();
            wo.indexInWayPointSystem = transform.childCount;
            wo.transform.position = positions[i - j];
            wo.transform.rotation = rotations[i - j];
            wo.transform.parent = transform;
            wo.name = currentPrefab.name + wo.indexInWayPointSystem;
            elements.Add(wo);
        }
    }

    public V3WayPointObject CreateWayPoint()
    {
        GameObject gm = Instantiate(currentPrefab);
        V3WayPointObject wo = gm.GetComponent<V3WayPointObject>();
        if (wo == null)
        {
            wo = gm.AddComponent<V3WayPointObject>();
        }
        wo.wpSys = this;
        Selection.activeObject = gm;
        return wo;
    }

    private void AppendOrPrepend(bool append)
    {
        V3WayPointObject wo = CreateWayPoint();
        wo.indexInWayPointSystem = append ? transform.childCount : 0;
        wo.transform.parent = transform;
        wo.transform.SetSiblingIndex(wo.indexInWayPointSystem);
        wo.name = currentPrefab.name + wo.indexInWayPointSystem;
        elements.Insert(wo.indexInWayPointSystem, wo);
        Vector3 tmpPos = transform.position;
        if (wo.indexInWayPointSystem > 0)
        {
            V3WayPointObject woPrev = elements[wo.indexInWayPointSystem - 1];
            tmpPos = woPrev.transform.position + woPrev.transform.forward * -10;
        }
        else if (elements.Count > 1)
        {
            V3WayPointObject woPrev = elements[wo.indexInWayPointSystem + 1];
            tmpPos = woPrev.transform.position + woPrev.transform.forward * 10;
        }
        wo.transform.position = tmpPos;
        UpdateEverything();
    }

    /// <summary>
    /// Maakt een nieuwe waypoint aan het einde aan
    /// </summary>
    public void Append()
    {
        AppendOrPrepend(true);
#if false
        //Debug.Log("Append");
        V3WayPointObject wo = CreateWayPoint();     
        wo.indexInWayPointSystem = transform.childCount;
        wo.transform.parent = transform;
        wo.name = currentPrefab.name + wo.indexInWayPointSystem;
        elements.Add(wo);
        if (wo.indexInWayPointSystem > 0)
        {
            V3WayPointObject woPrev = elements[wo.indexInWayPointSystem - 1];
            wo.transform.position = woPrev.transform.position + woPrev.transform.forward * -10;
        }
        UpdateEverything();
#endif
    }

    /// <summary>
    /// Maakt een nieuwe waypoint aan het begin aan
    /// </summary>
    public void Prepend()
    {
        AppendOrPrepend(false);
#if false
        //Debug.Log("Prepend");
        V3WayPointObject wo = CreateWayPoint();
        wo.indexInWayPointSystem = 0;
        wo.transform.parent = transform;
        wo.transform.SetAsFirstSibling();
        wo.name = currentPrefab.name + wo.indexInWayPointSystem;
        elements.Insert(0, wo);
        //UpdateNamesAndIndicesFromIndex(1);
        if (elements.Count > 1)
        {
            V3WayPointObject woPrev = elements[wo.indexInWayPointSystem + 1];
            wo.transform.position = woPrev.transform.position + woPrev.transform.forward * 10;
        }
        UpdateEverything();
#endif
    }

    /// <summary>
    /// Maakt een nieuwe waypoint na de huidige aan
    /// </summary>
    public void InsertAfter(V3WayPointObject wo)
    {
        //geselecteerde object is de laatste
        if (wo.indexInWayPointSystem == elements.Count - 1)
        {
            Append();
        }
        //insertafter
        else
        {
            V3WayPointObject woNext = elements[wo.indexInWayPointSystem + 1];
            //Debug.Log("InsertAfter");
            V3WayPointObject woNew = CreateWayPoint();
            woNew.indexInWayPointSystem = wo.indexInWayPointSystem + 1;
            woNew.transform.parent = transform;
            woNew.transform.SetSiblingIndex(woNew.indexInWayPointSystem);
            woNew.name = currentPrefab.name + woNew.indexInWayPointSystem;
            elements.Insert(woNew.indexInWayPointSystem, woNew);
            //UpdateNamesAndIndicesFromIndex(woNew.indexInWayPointSystem + 1);
            woNew.transform.position = Vector3.Lerp(wo.transform.position, woNext.transform.position, .5f);
            UpdateEverything();
        }
    }

    /// <summary>
    /// Maakt een nieuwe waypoint voor de huidige aan
    /// </summary>
    public void InsertBefore(V3WayPointObject wo)
    {
        //geselecteerde object is de eerste
        if (wo.indexInWayPointSystem == 0)
        {
            Prepend();
        }
        //insertbefore
        else
        {
            V3WayPointObject woPrev = elements[wo.indexInWayPointSystem - 1];
            //Debug.Log("InsertBefore");
            V3WayPointObject woNew = CreateWayPoint();
            woNew.indexInWayPointSystem = wo.indexInWayPointSystem;
            woNew.transform.parent = transform;
            woNew.transform.SetSiblingIndex(woNew.indexInWayPointSystem);
            woNew.name = currentPrefab.name + woNew.indexInWayPointSystem;
            elements.Insert(woNew.indexInWayPointSystem, woNew);
            //UpdateNamesAndIndicesFromIndex(woNew.indexInWayPointSystem + 1);
            woNew.transform.position = Vector3.Lerp(wo.transform.position, woPrev.transform.position, .5f);
            UpdateEverything();
        }
    }
}
