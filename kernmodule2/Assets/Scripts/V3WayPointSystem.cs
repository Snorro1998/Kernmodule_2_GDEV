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

    /// <summary>
    /// Pakt het object uit als het een prefab is.
    /// </summary>
    private void Awake()
    {
        if (PrefabUtility.IsPartOfPrefabInstance(gameObject))
        {
            PrefabUtility.UnpackPrefabInstance(gameObject, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
        }
    }

    /// <summary>
    /// Stelt in of het systeem rond gaat of niet.
    /// </summary>
    /// <param name="looping"></param>
    public void SetLoopAround(bool looping)
    {
        loop = looping;
        UpdateEverything();
    }

    /// <summary>
    /// Werkt alle elementen in het waypointsysteem bij.
    /// </summary>
    public void UpdateEverything()
    {
        for (int j = 0; j < elements.Count; j++)
        {
            // Dit misschien omschrijven naar een if-blok zodat het iets leesbaarder is
            V3WayPointObject nObj = j != elements.Count - 1 ? elements[j + 1] : loop ? elements[0] : null;
            V3WayPointObject pObj = j != 0 ? elements[j - 1] : loop ? elements[elements.Count - 1] : null;

            V3WayPointObject wo = elements[j];
            if (wo == null)
            {
                return;
            }
            wo.name = currentPrefab.name + j;
            wo.indexInWayPointSystem = j;

            wo.prevObj = pObj;
            wo.nextObj = nObj;

            wo.UpdateSelf();
        }
    }

    /// <summary>
    /// Verwijdert een waypoint van de lijst en werkt alle andere bij.
    /// </summary>
    /// <param name="wo"></param>
    /// <param name="index"></param>
    public void RemoveWayPoint(V3WayPointObject wo, int index)
    {
        elements.Remove(wo);
        UpdateEverything();
        //UpdateNamesAndIndicesFromIndex(index);
    }

    /// <summary>
    /// Voegt een waypoint opnieuw toe die voorheen verwijderd was. Wordt meestal aangeroepen als je met Ctrl+Z een verwijderd element terug haalt.
    /// </summary>
    /// <param name="wo"></param>
    /// <param name="index"></param>
    public void ReAddDeletedWayPoint(V3WayPointObject wo, int index)
    {
        if (!elements.Contains(wo))
        {
            // Index van object valt binnen de omvang van de lijst.
            if (index > 0 && index < elements.Count)
            {
                if (elements[index] == null)
                {
                    elements[index] = wo;
                }
                else
                {
                    elements.Insert(index, wo);
                }
            }
            // Als zijn index buiten de waypointlijst valt, voeg dan lege elementen vanaf het eind van de lijst toe totdat zijn eigen
            // index bereikt is. Dit gebeurt gek genoeg vaak als een scene geladen wordt en een waypoint dat later voorkomt zichzelf
            // eerder probeert toe te voegen dan zijn voorgangers.
            else
            {
                for (int i = elements.Count - 1; i < index - 1; i++)
                {
                    elements.Add(null);
                }
                elements.Add(wo);
            }
            UpdateEverything();
            //UpdateNamesAndIndicesFromIndex(index + 1);
        }
    }

    /// <summary>
    /// Werkt alle elementen bij wanneer de prefab is aangepast.
    /// </summary>
    public void UpdatePrefabForChildren()
    {
        int index = -1;
        V3WayPointObject woCur = Selection.activeTransform.GetComponent<V3WayPointObject>();
        if (woCur != null)
        {
            index = woCur.indexInWayPointSystem;
        }
        List<Vector3> positions = new List<Vector3>();
        List<Quaternion> rotations = new List<Quaternion>();

        // Verwijdert alle objecten.
        int i = transform.childCount - 1;
        for (int j = i; j >= 0; j--)
        {
            positions.Add(elements[j].transform.position);
            rotations.Add(elements[j].transform.rotation);
            DestroyImmediate(elements[j].gameObject);
        }
        // Maakt objecten opnieuw aan met nieuwe prefab.
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
        // Stelt huidig geselecteerde object juist in na alles opnieuw aan te hebben gemaakt.
        Selection.activeTransform = index != -1 ? elements[index].transform : transform;      
    }

    /// <summary>
    /// Maakt een nieuwe waypoint aan en geeft deze door.
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// Voeg een nieuwe waypoint aan het begin of einde toe.
    /// </summary>
    /// <param name="append"></param>
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
    }

    /// <summary>
    /// Maakt een nieuwe waypoint aan het begin aan
    /// </summary>
    public void Prepend()
    {
        AppendOrPrepend(false);
    }

    /// <summary>
    /// Maakt een nieuwe waypoint na de huidige aan
    /// </summary>
    public void InsertAfter(V3WayPointObject wo)
    {
        // Doe hetzelfde als append als het geselecteerde object de laatste is.
        if (wo.indexInWayPointSystem == elements.Count - 1)
        {
            Append();
        }
        // Voeg in alle andere gevallen gewoon in na de huidige.
        else
        {
            V3WayPointObject woNext = elements[wo.indexInWayPointSystem + 1];
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
    /// Maakt een nieuwe waypoint voor de huidige aan.
    /// </summary>
    public void InsertBefore(V3WayPointObject wo)
    {
        // Doe hetzelfde als prepend als het geselecteerde object de eerste is.
        if (wo.indexInWayPointSystem == 0)
        {
            Prepend();
        }
        // Voeg in alle andere gevallen gewoon in voor de huidige.
        else
        {
            V3WayPointObject woPrev = elements[wo.indexInWayPointSystem - 1];
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
