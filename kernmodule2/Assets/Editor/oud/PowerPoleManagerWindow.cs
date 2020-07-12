using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// Venster waarmee je stroompalen kunt plaatsen
/// </summary>
public class PowerPoleManagerWindow : EditorWindow
{
    [MenuItem("Tools/Power Pole Editor")]
    public static void Open()
    {
        GetWindow<PowerPoleManagerWindow>();
    }

    public Transform powerPoleRoot;

    private int menuIndex = 0;
    private Vector3 lastPos, currentPos;
    private GameObject lastSelectedObject, currentSelectedObject;
    
    private GameObject powerPolePrefab;

    //met oninspectorupdate zag je dat hij het telkens traag bijwerkt, update runt veel vaker en ziet er wel goed uit maar is niet bepaald efficient
    //het zou dan ook beter zijn om hiervoor nog even naar een alternatief te zoeken

    //OnInspectorUpdate()
    private void Update()
    {
        if (currentSelectedObject != null)
        {
            currentPos = currentSelectedObject.transform.position;

            if (lastSelectedObject != currentSelectedObject)
            {
                lastPos = currentPos;
                lastSelectedObject = currentSelectedObject;
            }
        }

        if (currentPos != lastPos)
        {
            PowerPole pole = currentSelectedObject.GetComponent<PowerPole>();
            if (pole != null)
            {
                pole.UpdatePreviousObjectsInLine();
                if (pole.nextPowerPole != null)
                {
                    pole.LookAtNext();
                }
                else
                {
                    pole.LookAtPrevious();
                }
               
            }

            lastPos = currentPos;
        }
    }

    private void OnSelectionChange()
    {
        menuIndex = 0;
        lastSelectedObject = currentSelectedObject;
        currentSelectedObject = Selection.activeGameObject;

        if (currentSelectedObject != null)
        {
            currentPos = currentSelectedObject.transform.position;
            lastPos = currentPos;

            if (currentSelectedObject.GetComponent<PowerPole>())
            {
                powerPoleRoot = currentSelectedObject.transform.parent;
                menuIndex = powerPoleRoot == null ? 0 : 1;
            }

            else if (currentSelectedObject.transform.childCount > 0)
            {
                if (currentSelectedObject.transform.GetChild(0).GetComponent<PowerPole>())
                {
                    powerPoleRoot = currentSelectedObject.transform;
                    menuIndex = 1;
                }
            }
        }
    }

    private void OnGUI()
    {
        powerPolePrefab = EditorGUILayout.ObjectField("Power Pole Prefab", powerPolePrefab, typeof(GameObject), true) as GameObject;

        //tekent de knoppen
        switch (menuIndex)
        {
            default:
                DrawCreateButton();
                break;
            case 1:
                DrawButtons();
                break;
        }
    }

    private void CreateSingleObject()
    {
        var myGO = PrefabUtility.InstantiatePrefab(powerPolePrefab);
        var newPowerPole = PrefabUtility.GetOutermostPrefabInstanceRoot(myGO);
        newPowerPole.name = "Powerpole" + powerPoleRoot.childCount;
        newPowerPole.transform.parent = powerPoleRoot;
        newPowerPole.transform.position = powerPoleRoot.position;
        Selection.activeGameObject = newPowerPole.gameObject;
    }

    private void CreateNewPowerLine()
    {
        Camera sceneCam = SceneView.lastActiveSceneView.camera;
        Vector3 spawnPos = Vector3.zero;

        RaycastHit hit;
        if (Physics.Raycast(sceneCam.transform.position, sceneCam.transform.forward, out hit, Mathf.Infinity))
        {
            spawnPos = hit.point;
        }

        else
        {
            float camAngle = Vector3.Angle(sceneCam.transform.forward, Vector3.down);

            //kijkt naar de grond
            if (camAngle < 90)
            {
                float vectorMagnitude = sceneCam.transform.position.y / Mathf.Cos(Mathf.Deg2Rad * camAngle);
                spawnPos = sceneCam.transform.position + sceneCam.transform.forward * vectorMagnitude;
            }
        }
        
        powerPoleRoot = new GameObject("Powerline").transform;
        powerPoleRoot.position = spawnPos;
        CreateSingleObject();
        /*
        var myGO = PrefabUtility.InstantiatePrefab(powerPolePrefab);
        var newPowerPole = PrefabUtility.GetOutermostPrefabInstanceRoot(myGO);
        newPowerPole.name = "Powerpole0";
        newPowerPole.transform.parent = powerPoleRoot;
        newPowerPole.transform.position = powerPoleRoot.position;
        Selection.activeGameObject = newPowerPole.gameObject;*/
    }

    private void DrawCreateButton()
    {
        if (GUILayout.Button("Create new power line"))
        {
            CreateNewPowerLine();
        }
    }

    private void DrawButtons()
    {
        GUILayout.Space(10);
        GUILayout.Label("Create new power pole", EditorStyles.boldLabel);

        //EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("At the end"))
        {
            CreateWaypoint();
        }

        if (GUILayout.Button("At the beginning"))
        {
            Selection.activeObject = powerPoleRoot.GetChild(0);
            CreateWaypointBefore();
        }

        //EditorGUILayout.EndHorizontal();

        if (Selection.activeGameObject != null && Selection.activeGameObject.GetComponent<PowerPole>())
        {
            //EditorGUILayout.BeginHorizontal();

            if(GUILayout.Button("Before selected"))
            {
                CreateWaypointBefore();
            }
            
            if(GUILayout.Button("After selected"))
            {
                CreateWaypointAfter();
            }

            //EditorGUILayout.EndHorizontal();
            /*
            if(GUILayout.Button("Remove Waypoint"))
            {
                RemoveWaypoint();
            }*/
        }
    }

    private void CreateWaypoint()
    {
        GameObject waypointObject = Instantiate(powerPolePrefab, Vector3.zero, Quaternion.identity);
        waypointObject.name = "Powerpole" + powerPoleRoot.childCount;
        waypointObject.transform.SetParent(powerPoleRoot, false);

        PowerPole waypoint = waypointObject.GetComponent<PowerPole>();
        if(powerPoleRoot.childCount > 1)
        {
            waypoint.previousPowerPole = powerPoleRoot.GetChild(powerPoleRoot.childCount - 2).GetComponent<PowerPole>();
            waypoint.previousPowerPole.nextPowerPole = waypoint;
            waypoint.transform.position = waypoint.previousPowerPole.transform.position + waypoint.previousPowerPole.transform.forward * 20;
            waypoint.transform.forward = waypoint.previousPowerPole.transform.forward;

            waypoint.previousPowerPole.CreateWires  ();
        }

        Selection.activeGameObject = waypoint.gameObject;
    }

    private void CreateWaypointBefore()
    {
        GameObject waypointObject = Instantiate(powerPolePrefab, Vector3.zero, Quaternion.identity);
        GameObject selectedWayPointObject = Selection.activeGameObject;

        int objIndex = selectedWayPointObject.transform.GetSiblingIndex();
        waypointObject.name = "Powerpole" + objIndex;
        waypointObject.transform.SetParent(powerPoleRoot, false);
        waypointObject.transform.SetSiblingIndex(objIndex);

        //namen corrigeren
        for (int i = objIndex; i < powerPoleRoot.childCount; i++)
        {
            powerPoleRoot.GetChild(i).name = "Powerpole" + i;
        }

        PowerPole newWaypoint = waypointObject.GetComponent<PowerPole>();
        PowerPole selectedWaypoint = selectedWayPointObject.GetComponent<PowerPole>();

        newWaypoint.nextPowerPole = selectedWaypoint;
        
        if (selectedWaypoint.previousPowerPole != null)
        {
            newWaypoint.previousPowerPole = selectedWaypoint.previousPowerPole;
            selectedWaypoint.previousPowerPole.nextPowerPole = newWaypoint;
        }

        selectedWaypoint.previousPowerPole = newWaypoint;

        if (newWaypoint.previousPowerPole != null)
        {
            waypointObject.transform.position = Vector3.Lerp(newWaypoint.previousPowerPole.transform.position, newWaypoint.nextPowerPole.transform.position, 0.5f);
            waypointObject.transform.rotation = Quaternion.Lerp(selectedWaypoint.previousPowerPole.transform.rotation, selectedWaypoint.transform.rotation, 0.5f);
            newWaypoint.previousPowerPole.UpdateWires();
        }

        else
        {
            waypointObject.transform.position = selectedWaypoint.transform.position - selectedWaypoint.transform.forward * 20;
        }
     
        newWaypoint.CreateWires();
        Selection.activeObject = waypointObject;
    }

    private void CreateWaypointAfter()
    {
        GameObject selectedWayPointObject = Selection.activeGameObject;
        PowerPole selectedWaypoint = selectedWayPointObject.GetComponent<PowerPole>();

        //doe hetzelfde als createwaypoint als geselecteerde paal aan het eind zit
        if (selectedWaypoint.nextPowerPole == null)
        {
            CreateWaypoint();
            return;
        }

        
        GameObject waypointObject = Instantiate(powerPolePrefab, Vector3.zero, Quaternion.identity);
        PowerPole newWaypoint = waypointObject.GetComponent<PowerPole>();

        int objIndex = selectedWayPointObject.transform.GetSiblingIndex() + 1;
        waypointObject.name = "Powerpole" + objIndex;
        waypointObject.transform.SetParent(powerPoleRoot, false);
        waypointObject.transform.SetSiblingIndex(objIndex);

        //namen corrigeren
        for (int i = objIndex; i < powerPoleRoot.childCount; i++)
        {
            powerPoleRoot.GetChild(i).name = "Powerpole" + i;
        }

        //stel het voor de nieuwe paal de
        newWaypoint.previousPowerPole = selectedWaypoint;
        newWaypoint.nextPowerPole = selectedWaypoint.nextPowerPole;

        //en diegene erna en ervoor
        newWaypoint.nextPowerPole.previousPowerPole = newWaypoint;
        newWaypoint.previousPowerPole.nextPowerPole = newWaypoint;

        newWaypoint.CreateWires();
        waypointObject.transform.position = Vector3.Lerp(newWaypoint.previousPowerPole.transform.position, newWaypoint.nextPowerPole.transform.position, 0.5f);
        waypointObject.transform.rotation = Quaternion.Lerp(selectedWaypoint.previousPowerPole.transform.rotation, selectedWaypoint.transform.rotation, 0.5f);
        newWaypoint.previousPowerPole.UpdateWires();
        //vreemd genoeg werkt het zonder dit niet
        newWaypoint.UpdateWires();

#if false
        GameObject waypointObject = new GameObject("Waypoint " + powerPoleRoot.childCount, typeof(WayPoint));
        waypointObject.transform.SetParent(powerPoleRoot, false);

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
#endif
    }

    private void RemoveWaypoint()
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
