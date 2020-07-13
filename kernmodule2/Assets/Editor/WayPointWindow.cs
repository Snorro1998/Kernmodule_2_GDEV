using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class WayPointWindow : EditorWindow
{
    public GameObject currentSelected = null;
    public GameObject activeObjectPrefab = null;
    public GameObject lastActiveObjectPrefab = null;

    WayPointSystem wp = null;

    [MenuItem("Tools/Waypoint Editor")]
    public static void Open()
    {
        GetWindow<WayPointWindow>();
    }

    private void OnGUI()
    {   
        SerializedObject objj = new SerializedObject(this);
        EditorGUILayout.PropertyField(objj.FindProperty("currentSelected"));
        objj.ApplyModifiedProperties();

        SerializedObject obj = new SerializedObject(this);
        EditorGUILayout.PropertyField(obj.FindProperty("activeObjectPrefab"));
        obj.ApplyModifiedProperties();
    }

    private void Update()
    {
        if (lastActiveObjectPrefab != activeObjectPrefab && currentSelected != null)
        {
            //Debug.Log("prefab is aangepast");
            lastActiveObjectPrefab = activeObjectPrefab;
            wp.currentPrefab = activeObjectPrefab;
            wp.UpdatePrefabOfChildren();
        }
    }

    private void OnSelectionChange()
    {
        currentSelected = Selection.activeGameObject;
        //activeObjectPrefab = null;
        lastActiveObjectPrefab = activeObjectPrefab;
        if(currentSelected != null)
        {
            wp = currentSelected.GetComponent<WayPointSystem>();
            if (wp != null)
            {
                activeObjectPrefab = wp.currentPrefab;
                lastActiveObjectPrefab = activeObjectPrefab;
                return;
            }
            if (currentSelected.transform.parent != null)
            {
                currentSelected = currentSelected.transform.root.gameObject;
            }
            wp = currentSelected.GetComponent<WayPointSystem>();
            if (wp != null)
            {
                activeObjectPrefab = wp.currentPrefab;
                lastActiveObjectPrefab = activeObjectPrefab;
            }
            else
            {
                currentSelected = null;
            }
        }
    }
}
