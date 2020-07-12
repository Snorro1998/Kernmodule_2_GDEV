using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class WayPointSystem : MonoBehaviour
{
    [HideInInspector]
    public GameObject currentPrefab = null;

    public void UpdatePrefabOfChildren()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Transform t = transform.GetChild(i);

            var currentComponents = t.GetComponents<Component>();
            var newComponents = currentPrefab.GetComponents<Component>();

            //verwijdert alle componenten van het object
            foreach (var comp in currentComponents)
            {
                if (comp.GetType() != typeof(Transform))
                {
                    DestroyImmediate(comp);
                }
            }

            //voegt alle nieuwe componenten toe
            foreach (var comp in newComponents)
            {
                if (comp.GetType() != typeof(Transform))
                {
                    UnityEditorInternal.ComponentUtility.CopyComponent(comp);
                    UnityEditorInternal.ComponentUtility.PasteComponentAsNew(t.gameObject);
                }
            }

            //stelt de schaal en de naam juist in
            t.localScale = currentPrefab.transform.localScale;
            t.name = currentPrefab.name + (i + 1).ToString();

            /*
            for (int j = currentComponents.Length - 1; j >= 0; j--)
            {
                var comp = currentComponents[j];
                if (comp.GetType() != typeof(Transform))
                {
                    DestroyImmediate(comp);
                }
            }

            for (int k = 0; k <= currentComponents.Length - 1; k++)
            {
                var comp = currentComponents[k];
                if (comp.GetType() != typeof(Transform))
                {
                    UnityEditorInternal.ComponentUtility.CopyComponent(components[i]);
                    UnityEditorInternal.ComponentUtility.PasteComponentAsNew(targetGameObject);
                    //t.gameObject.AddComponent<>();
                }
            }*/

            /*
            foreach (var component in components)
            {
                //var a = component.GetType();
                if (component.GetType() == typeof(Transform))
                {

                }
                //currentComponents.Add(component);
            }*/

            //Debug.Log(currentComponents.Count);

            /*
            Vector3 tmpPos = t.position;
            Quaternion tmpRot = t.rotation;
            DestroyImmediate(t.gameObject);
            Transform tNew = Instantiate(currentPrefab).transform;
            tNew.position = tmpPos;
            tNew.rotation = tmpRot;
            tNew.parent = transform;
            tNew.SetSiblingIndex(i);
            tNew.name = currentPrefab.name + (i + 1).ToString();*/
        }     
    }
}
