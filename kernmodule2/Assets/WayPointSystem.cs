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

    public void CallChildrenUpdateMethods()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            //Huidig, vorig en volgend object.
            Transform t = transform.GetChild(i);
            Transform tNext = i != transform.childCount - 1 ? transform.GetChild(i + 1) : loop ? transform.GetChild(0) : null;
            Transform tPrev = i != 0 ? transform.GetChild(i - 1) : loop ? transform.GetChild(transform.childCount - 1) : null;

            Component[] components = t.GetComponents<Component>();
            WayPointObject w = null;

            //Kijkt of het object het component WayPointObject heeft of iets wat hiervan afstamt.
            foreach (Component comp in components)
            {
                w = comp as WayPointObject;
                if (w != null)
                {
                    break;
                }
            }
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
