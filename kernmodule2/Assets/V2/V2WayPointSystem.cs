using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class V2WayPointSystem : MonoBehaviour
{
    public List<V2WayPointObject> elements = new List<V2WayPointObject>();

    public int index = 0;
    public bool boolAppend = false;
    public bool boolPrepend = false;
    public bool boolInsertAfter = false;
    public bool boolInsertBefore = false;

    public GameObject currentPrefab = null;
    public bool loop = false;

    /// <summary>
    /// Een functie die objecten bijwerkt door ze opnieuw aan te maken. Breekt andere objecten als die referenties zouden hebben.
    /// </summary>
    /// <param name="t"></param>
    /// <param name="i"></param>
    private void CreateNewObject(Transform t, int i)
    {
        Vector3 tmpPos = t.position;
        Quaternion tmpRot = t.rotation;
        DestroyImmediate(t.gameObject);
        Transform tNew = Instantiate(currentPrefab).transform;
        tNew.position = tmpPos;
        tNew.rotation = tmpRot;
        tNew.parent = transform;
        tNew.SetSiblingIndex(i);
        tNew.name = currentPrefab.name + (i + 1).ToString();

        V2WayPointObject vo = tNew.GetComponent<V2WayPointObject>();
        if (vo == null)
        {
            vo = tNew.gameObject.AddComponent<V2WayPointObject>();
        }

        vo.indexInWayPoint = i;
        elements.Insert(0, vo);
    }

    /// <summary>
    /// Wordt uitgevoerd wanneer de prefab is aangepast
    /// </summary>
    public void UpdatePrefabOfChildren()
    {
        if (transform.childCount == 0)
        {
            return;
        }
        Debug.Log("UpdateChildrenToNewPrefab");
        int j = -1;
        Transform tmpObj = Selection.activeTransform;
        Transform tmpObjPar = tmpObj.root;
        if (tmpObjPar != tmpObj)
        {
            j = tmpObj.GetSiblingIndex();
        }
        for (int i = transform.childCount - 1; i >= 0; i--)
        {      
            Transform t = transform.GetChild(i);
            Vector3 tmpPos = t.position;
            Quaternion tmpRot = t.rotation;
            //V2WayPointObject vo = elements[i];
            //vo.indexInWayPoint = 0;
            DestroyImmediate(t.gameObject);
            Transform tNew = Instantiate(currentPrefab).transform;
            tNew.position = tmpPos;
            tNew.rotation = tmpRot;
            tNew.parent = transform;
            tNew.SetSiblingIndex(i);
            V2WayPointObject vo = tNew.GetComponent<V2WayPointObject>();
            if (vo == null)
            {
                vo = tNew.gameObject.AddComponent<V2WayPointObject>();
                elements.Insert(i, vo);
                vo.indexInWayPoint = i;
            }
        }
        if (j != -1)
        {
            Selection.activeTransform = tmpObjPar.GetChild(j);
        }
        /*
        #region nodig voor CreateNewObject, maar niet voor KeepObject
        int j = -1;
        Transform tmpObj = Selection.activeTransform;
        Transform tmpObjPar = tmpObj.root;
        if (tmpObjPar != tmpObj)
        {
            j = tmpObj.GetSiblingIndex();
        }
        #endregion
        */
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Transform t = transform.GetChild(i);
            //KeepObjectAndChangeComponents(t, i);
            CreateNewObject(t, i);
        }
        #region ook nodig voor CreateNewObject, maar niet voor KeepObject
        if (j != -1)
        {
            Selection.activeTransform = tmpObjPar.GetChild(j);
        }
        #endregion

        CallChildrenUpdateMethods();
    }

    public void SetLoopAround(bool looping)
    {
        loop = looping;
        CallChildrenUpdateMethods();
    }

    public void CallChildrenUpdateMethods()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            //Huidig, vorig en volgend object.
            Transform t = transform.GetChild(i);
            Transform tNext = i != transform.childCount - 1 ? transform.GetChild(i + 1) : loop ? transform.GetChild(0) : null;
            Transform tPrev = i != 0 ? transform.GetChild(i - 1) : loop ? transform.GetChild(transform.childCount - 1) : null;

            V2WayPointObject w = null;
            //Kijkt of het object het component WayPointObject heeft of iets wat hiervan afstamt.
            w = t.GetComponent<V2WayPointObject>();
            //Zo niet voeg er dan een toe.
            if (w == null)
            {
                w = t.gameObject.AddComponent<V2WayPointObject>();
            }
            if (w.wp == null)
            {
                w.wp = this;
            }
            w.nextObj = tNext;
            w.prevObj = tPrev;
            w.UpdateSelf();
            w.name = currentPrefab.name + (i + 1).ToString();
        }
    }

    public void UpdateNamesAndIndicesFromIndex(int i)
    {
        //return;
        for (int j = i; j < elements.Count; j++)
        {
            V2WayPointObject vo = elements[j];
            if (vo != null)
            {
                elements[j].indexInWayPoint = j;
                elements[j].transform.name = currentPrefab.name + j;
            }
            
        }
    }

    public void UpdateFromIndex(int i)
    {
        for (int j = i; j < elements.Count; j++)
        {
            elements[j].UpdateSelf();
        }
    }

    //deze functie werkt weer redelijk
    public void Append()
    {
        Vector3 tmpPos = transform.position;
        Quaternion tmpRot = transform.rotation;
        int index = 0;

        V2WayPointObject wo = null;
        if (transform.childCount != 0)
        {
            wo = elements[elements.Count - 1];
            tmpPos = wo.transform.position + wo.transform.forward * 20;
            tmpRot = wo.transform.rotation;
            index = wo.indexInWayPoint + 1;
        }

        Transform tNew = Instantiate(currentPrefab).transform;
        tNew.position = tmpPos;
        tNew.rotation = tmpRot;
        tNew.parent = transform;
        tNew.name = currentPrefab.name + (index).ToString();

        V2WayPointObject vo = tNew.GetComponent<V2WayPointObject>();
        if (vo == null)
        {
            vo = tNew.gameObject.AddComponent<V2WayPointObject>();
        }

        vo.indexInWayPoint = index;
        vo.wp = this;
        elements.Add(vo);

        /*
        GameObject gm = new GameObject();
        V2WayPointObject d = gm.AddComponent<V2WayPointObject>();
        d.indexInWayPoint = transform.childCount;
        d.wp = this;
        d.transform.name = "obj" + transform.childCount;
        d.transform.parent = transform;
        elements.Add(d);*/
    }

    public void Prepend()
    {
        GameObject gm = new GameObject();
        V2WayPointObject d = gm.AddComponent<V2WayPointObject>();
        d.indexInWayPoint = 0;
        d.wp = this;
        d.transform.name = "obj" + 0;
        d.transform.parent = transform;
        d.transform.SetAsFirstSibling();
        elements.Insert(0, d);
        UpdateNamesAndIndicesFromIndex(1);
    }

    public void InsertAfter(V2WayPointObject d)
    {
        if (d.indexInWayPoint == elements.Count - 1)
        {
            Append();
        }

        else
        {
            GameObject gm = new GameObject();
            V2WayPointObject dd = gm.AddComponent<V2WayPointObject>();
            dd.indexInWayPoint = d.indexInWayPoint + 1;
            dd.wp = this;
            dd.transform.name = "obj" + dd.indexInWayPoint;
            dd.transform.parent = transform;
            dd.transform.SetSiblingIndex(dd.indexInWayPoint);
            elements.Insert(dd.indexInWayPoint, dd);
            UpdateNamesAndIndicesFromIndex(dd.indexInWayPoint);
        }
    }

    public void InsertBefore(V2WayPointObject d)
    {
        if (d.indexInWayPoint == 0)
        {
            Prepend();
        }

        else
        {
            GameObject gm = new GameObject();
            V2WayPointObject dd = gm.AddComponent<V2WayPointObject>();
            dd.indexInWayPoint = d.indexInWayPoint;
            dd.wp = this;
            dd.transform.name = "obj" + dd.indexInWayPoint;
            dd.transform.parent = transform;
            dd.transform.SetSiblingIndex(dd.indexInWayPoint);
            elements.Insert(dd.indexInWayPoint, dd);
            UpdateNamesAndIndicesFromIndex(dd.indexInWayPoint);
        }
    }

    private void Update()
    {
        if (boolAppend)
        {
            Append();
            boolAppend = false;
        }
        if (boolPrepend)
        {
            Prepend();
            boolPrepend = false;
        }
        if (boolInsertAfter)
        {
            InsertAfter(elements[index]);
            boolInsertAfter = false;
        }
        if (boolInsertBefore)
        {
            InsertBefore(elements[index]);
            boolInsertBefore = false;
        }
    }
}
