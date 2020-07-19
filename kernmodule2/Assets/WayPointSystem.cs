using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class WayPointSystem : MonoBehaviour
{
    [HideInInspector]
    public GameObject currentPrefab = null;
    public bool loop = false;

    public List<WayPointObject> elements = new List<WayPointObject>();

    /// <summary>
    /// Verwijdert alle componenten van een object.
    /// </summary>
    /// <param name="t"></param>
    private void RemoveAllComponents(Transform t)
    {
        Component[] currentComponents = t.GetComponents<Component>();
        foreach (Component comp in currentComponents)
        {
            //negeert transforms. Kan ook andere dingen negeren door ze hieraan toe te voegen.
            if (comp.GetType() != typeof(Transform))
            {
                DestroyImmediate(comp);
            }
        }
    }

    /// <summary>
    /// Voeg alle componenten van de nieuwe prefab toe aan een object.
    /// </summary>
    /// <param name="t"></param>
    private void AddNewComponents(Transform t)
    {
        Component[] newComponents = currentPrefab.GetComponents<Component>();
        foreach (Component comp in newComponents)
        {
            //negeert transforms. Kan ook andere dingen negeren door ze hieraan toe te voegen.
            if (comp.GetType() != typeof(Transform))
            {
                UnityEditorInternal.ComponentUtility.CopyComponent(comp);
                UnityEditorInternal.ComponentUtility.PasteComponentAsNew(t.gameObject);
            }
        }
    }

    /// <summary>
    /// Een nettere functie om objecten bij te werken, maar faalt omdat hij de children niet mee kopieert.
    /// </summary>
    /// <param name="t"></param>
    /// <param name="i"></param>
    private void KeepObjectAndChangeComponents(Transform t, int i)
    {
        RemoveAllComponents(t);
        AddNewComponents(t);

        //stelt de schaal en de naam juist in.
        t.localScale = currentPrefab.transform.localScale;
        t.name = currentPrefab.name + (i + 1).ToString();
    }

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
    }

    /// <summary>
    /// Stelt in of het systeem rondgaat of niet
    /// </summary>
    /// <param name="looping"></param>
    public void SetLoopAround(bool looping)
    {
        loop = looping;
        CallChildrenUpdateMethods();
    }

    /// <summary>
    /// Maakt nieuwe waypoint aan
    /// </summary>
    /// <param name="atBeginning"></param>
    public void CreateWayPoint(bool atBeginning)
    {
        int index = atBeginning ? 0 : transform.childCount - 1;
        Transform t = Instantiate(currentPrefab).transform;
        t.position = transform.position;

        if (transform.childCount > 0)
        {
            Transform tmpT = transform.GetChild(index);
            t.position = tmpT.position + (atBeginning ? -1 : 1) * tmpT.forward * 20;
        }

        WayPointObject wp = t.GetComponent<WayPointObject>();
        if (wp == null)
        {
            wp = t.gameObject.AddComponent<WayPointObject>();
            wp.wpSys = this;
        }

        t.parent = transform;
        t.SetSiblingIndex(atBeginning ? index : index + 1);
        Selection.activeGameObject = t.gameObject;
    }

    public void V2AppendWayPoint()
    {
        Transform t = Instantiate(currentPrefab).transform;
        WayPointObject wp = t.GetComponent<WayPointObject>();
        int i = elements.Count;

        if (wp == null)
        {
            wp = t.gameObject.AddComponent<WayPointObject>();
        }

        wp.wpSys = this;
        elements.Add(wp);
        wp.transform.parent = transform;
        wp.transform.name = currentPrefab.name + i;
    }

    public void UpdateNamesAfterIndex(int i)
    {

    }

    /// <summary>
    /// Maakt nieuwe waypoint aan het einde aan
    /// </summary>
    public void AppendWayPoint()
    {
        CreateWayPoint(false);
    }

    /// <summary>
    /// Maakt nieuwe waypoint aan het begin aan
    /// </summary>
    public void PrependWayPoint()
    {
        CreateWayPoint(true);
    }

    public void InsertWayPointAfter(Transform t)
    {
        if (t != null)
        {
            int index = t.GetSiblingIndex();
            //als het geselecteerde element aan het einde staat doe dan hetzelfde als append
            if (index == t.root.childCount - 1)
            {
                AppendWayPoint();
            }
            else
            {
                Transform tt = Instantiate(currentPrefab).transform;
                tt.position = Vector3.Lerp(t.position, t.root.GetChild(index + 1).position, 0.5f);
                tt.parent = transform;
                tt.SetSiblingIndex(index + 1);
                Selection.activeGameObject = t.gameObject;
            }
        }
    }

    public void InsertWayPointBefore(Transform t)
    {
        if (t != null)
        {
            int index = t.GetSiblingIndex();
            //als het geselecteerde element aan het begin staat doe dan hetzelfde als prepend
            if (index == 0)
            {
                PrependWayPoint();
            }
            else
            {
                Transform tt = Instantiate(currentPrefab).transform;
                tt.position = Vector3.Lerp(t.position, t.root.GetChild(index - 1).position, 0.5f);
                tt.parent = transform;
                tt.SetSiblingIndex(index);
                Selection.activeGameObject = t.gameObject;
            }
        }
    }

    public void CallChildrenUpdateMethods()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            //Huidig, vorig en volgend object.
            Transform t = transform.GetChild(i);
            Transform tNext = i != transform.childCount - 1 ? transform.GetChild(i + 1) : loop ? transform.GetChild(0) : null;
            Transform tPrev = i != 0 ? transform.GetChild(i - 1) : loop ? transform.GetChild(transform.childCount - 1) : null;

            WayPointObject w = null;
            //Kijkt of het object het component WayPointObject heeft of iets wat hiervan afstamt.
            w = t.GetComponent<WayPointObject>();
            //Zo niet voeg er dan een toe.
            if (w == null)
            {
                w = t.gameObject.AddComponent<WayPointObject>();
            }
            if (w.wpSys == null)
            {
                w.wpSys = this;
            }
            w.nextObj = tNext;
            w.prevObj = tPrev;
            w.UpdateSelf();
            w.name = currentPrefab.name + (i + 1).ToString();
        }
    }

    /// <summary>
    /// Wordt uitgevoerd wanneer de prefab is aangepast
    /// </summary>
    public void UpdatePrefabOfChildren()
    {
        #region nodig voor CreateNewObject, maar niet voor KeepObject
        int j = -1;
        Transform tmpObj = Selection.activeTransform;
        Transform tmpObjPar = tmpObj.root;
        if (tmpObjPar != tmpObj)
        {
            j = tmpObj.GetSiblingIndex();
        }
        #endregion
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
}