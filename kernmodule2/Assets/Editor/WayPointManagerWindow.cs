using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class WayPointManagerWindow : EditorWindow
{
    /*
    [MenuItem("Tools/Waypoint Editor")]
    public static void Open()
    {
        GetWindow<WayPointManagerWindow>();
    }
    */

    public Transform waypointRoot;

    private void OnGUI()
    {
        SerializedObject obj = new SerializedObject(this);

        EditorGUILayout.PropertyField(obj.FindProperty("waypointRoot"));

        if(waypointRoot == null)
        {
            EditorGUILayout.HelpBox("Root transform must be selected. Please assign a root transform.", MessageType.Warning);
        }

        else
        {
            EditorGUILayout.BeginVertical("box");
            DrawButtons();
            EditorGUILayout.EndVertical();
        }

        obj.ApplyModifiedProperties();
    }

    void DrawButtons()
    {
        if(GUILayout.Button("Create Waypoint"))
        {
            CreateWaypoint();
        }
        if(Selection.activeGameObject != null && Selection.activeGameObject.GetComponent<WayPoint>())
        {
            if(GUILayout.Button("Create WayPoint Before"))
            {
                CreateWaypointBefore();
            }
            if(GUILayout.Button("Create Waypoint After"))
            {
                CreateWaypointAfter();
            }
            if(GUILayout.Button("Remove Waypoint"))
            {
                RemoveWaypoint();
            }
        }
    }

    void CreateWaypoint()
    {
        GameObject waypointObject = new GameObject("Waypoint" + waypointRoot.childCount, typeof(WayPoint));
        waypointObject.transform.SetParent(waypointRoot, false);

        WayPoint waypoint = waypointObject.GetComponent<WayPoint>();
        if(waypointRoot.childCount > 1)
        {
            waypoint.previousWayPoint = waypointRoot.GetChild(waypointRoot.childCount - 2).GetComponent<WayPoint>();
            waypoint.previousWayPoint.nextWayPoint = waypoint;
            //placethe waypoint at the last position
            waypoint.transform.position = waypoint.previousWayPoint.transform.position;
            waypoint.transform.forward = waypoint.previousWayPoint.transform.forward;
        }

        Selection.activeGameObject = waypoint.gameObject;
    }

    void CreateWaypointBefore()
    {
        GameObject waypointObject = new GameObject("Waypoint " + waypointRoot.childCount, typeof(WayPoint));
        waypointObject.transform.SetParent(waypointRoot, false);

        WayPoint newWaypoint = waypointObject.GetComponent<WayPoint>();

        WayPoint selectedWaypoint = Selection.activeGameObject.GetComponent<WayPoint>();

        waypointObject.transform.position = selectedWaypoint.transform.position;
        waypointObject.transform.forward = selectedWaypoint.transform.forward;

        if(selectedWaypoint.previousWayPoint)
        {
            if(selectedWaypoint.previousWayPoint != null)
            {
                newWaypoint.previousWayPoint = selectedWaypoint.previousWayPoint;
                selectedWaypoint.previousWayPoint.nextWayPoint = newWaypoint;
            }

            newWaypoint.nextWayPoint = selectedWaypoint;
            selectedWaypoint.previousWayPoint = newWaypoint;

            newWaypoint.transform.SetSiblingIndex(selectedWaypoint.transform.GetSiblingIndex());

            Selection.activeGameObject = newWaypoint.gameObject;
        }
    }

    void CreateWaypointAfter()
    {
        GameObject waypointObject = new GameObject("Waypoint " + waypointRoot.childCount, typeof(WayPoint));
        waypointObject.transform.SetParent(waypointRoot, false);

        WayPoint newWaypoint = waypointObject.GetComponent<WayPoint>();

        WayPoint selectedWaypoint = Selection.activeGameObject.GetComponent<WayPoint>();

        waypointObject.transform.position = selectedWaypoint.transform.position;
        waypointObject.transform.forward = selectedWaypoint.transform.forward;

        newWaypoint.previousWayPoint = selectedWaypoint;

        if(selectedWaypoint.nextWayPoint != null)
        {
            selectedWaypoint.nextWayPoint.previousWayPoint = newWaypoint;
            newWaypoint.nextWayPoint = selectedWaypoint.nextWayPoint;
        }

        selectedWaypoint.nextWayPoint = newWaypoint;
        newWaypoint.transform.SetSiblingIndex(selectedWaypoint.transform.GetSiblingIndex());

        Selection.activeGameObject = newWaypoint.gameObject;
    }

    void RemoveWaypoint()
    {
        WayPoint selectedWaypoint = Selection.activeGameObject.GetComponent<WayPoint>();

        if(selectedWaypoint.nextWayPoint != null)
        {
            selectedWaypoint.nextWayPoint.previousWayPoint = selectedWaypoint.previousWayPoint;
        }
        if(selectedWaypoint.previousWayPoint != null)
        {
            selectedWaypoint.previousWayPoint.nextWayPoint = selectedWaypoint.nextWayPoint;
            Selection.activeGameObject = selectedWaypoint.previousWayPoint.gameObject;
        }

        DestroyImmediate(selectedWaypoint.gameObject);
    }
}
