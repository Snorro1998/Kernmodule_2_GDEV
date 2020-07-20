﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class V3WayPointWindow : EditorWindow
{
    private GameObject currentWPElement = null;
    public GameObject activeObjectPrefab = null;
    private GameObject lastActiveObjectPrefab = null;

    private bool activeLoop = false;
    private bool lastLoop = false;

    private V3WayPointSystem currentWayPointSystem = null;

    [MenuItem("Tools/V3 Waypoint Editor")]
    public static void Open()
    {
        GetWindow<V3WayPointWindow>();
    }
    
    /// <summary>
    /// Toont een knop die een functie uitvoert wanneer hij aangeklikt wordt.
    /// </summary>
    /// <param name="txt"></param>
    /// <param name="a"></param>
    private void DisplayButton(string txt, Action a)
    {
        if (GUILayout.Button(txt))
        {
            a();
        }
    }

    private void OnGUI()
    {
        SerializedObject obj = new SerializedObject(this);
        EditorGUILayout.PropertyField(obj.FindProperty("activeObjectPrefab"));
        obj.ApplyModifiedProperties();

        if (currentWayPointSystem == null)
        {
            if (activeObjectPrefab == null)
            {
                GUILayout.Label("No prefab selected", EditorStyles.boldLabel);
            }
            else
            {
                DisplayButton("Create new waypoint system", () => { CreateNewWayPointSystem(); });
            }
        }
        else
        {
            activeLoop = EditorGUILayout.Toggle("Loop", activeLoop);
            GUILayout.Label("Create new " + activeObjectPrefab.name, EditorStyles.boldLabel);

            DisplayButton("At the end", () => { currentWayPointSystem.Append(); });
            DisplayButton("At the beginning", () => { currentWayPointSystem.Prepend(); });
            if (currentWPElement != null)
            {
                DisplayButton("After selected", () => { currentWayPointSystem.InsertAfter(currentWPElement.GetComponent<V3WayPointObject>()); });
                DisplayButton("Before selected", () => { currentWayPointSystem.InsertBefore(currentWPElement.GetComponent<V3WayPointObject>()); });
            }
        }
    }

    private void Update()
    {
        if (currentWayPointSystem != null)
        {
            // Prefab is aangepast.
            if (lastActiveObjectPrefab != activeObjectPrefab)
            {
                lastActiveObjectPrefab = activeObjectPrefab;
                currentWayPointSystem.currentPrefab = activeObjectPrefab;
                currentWayPointSystem.UpdatePrefabForChildren();
            }
            // Loopoptie is aangepast.
            if (activeLoop != lastLoop)
            {
                lastLoop = activeLoop;
                currentWayPointSystem.SetLoopAround(activeLoop);
            }
        }
    }

    /// <summary>
    /// Pakt het object dat een niveau onder de root zit.
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="par"></param>
    /// <returns></returns>
    private GameObject GetChildGameObjectFromHighestLevel(GameObject obj, GameObject par)
    {
        Transform tmpTransform = obj.transform;
        Transform tmpTransformPar = par.transform;
        for (int i = 0; i < 200; i++)
        {
            if (tmpTransform.parent == tmpTransformPar || tmpTransform.parent == null)
            {
                break;
            }
            tmpTransform = tmpTransform.parent;
        }
        return tmpTransform.gameObject;
    }

    /// <summary>
    /// Berekent spawnpositie afhankelijk van de camera-orientatie.
    /// </summary>
    /// <returns></returns>
    private Vector3 CalcSpawnPosition()
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

            // Kijkt naar de grond.
            if (camAngle < 90)
            {
                float vectorMagnitude = sceneCam.transform.position.y / Mathf.Cos(Mathf.Deg2Rad * camAngle);
                spawnPos = sceneCam.transform.position + sceneCam.transform.forward * vectorMagnitude;
            }
        }
        return spawnPos;
    }

    /// <summary>
    /// Maakt een nieuw waypointsysteem aan.
    /// </summary>
    private void CreateNewWayPointSystem()
    {
        Vector3 spawnPos = CalcSpawnPosition();
        GameObject gm = new GameObject("WayPointSystem");
        gm.transform.position = spawnPos;
        currentWayPointSystem = gm.AddComponent<V3WayPointSystem>();
        currentWayPointSystem.currentPrefab = activeObjectPrefab;
        currentWayPointSystem.Append();
    }

    /// <summary>
    /// Werk getoonde instellingen bij als de selectie is verandert.
    /// </summary>
    private void UpdateSettings()
    {
        currentWPElement = null;
        currentWayPointSystem = null;
        GameObject currentSelected = Selection.activeGameObject;
        lastActiveObjectPrefab = activeObjectPrefab;

        // Er is iets geselecteerd.
        if (currentSelected != null)
        {
            currentWayPointSystem = currentSelected.GetComponent<V3WayPointSystem>();
            // Het geselecteerde object is geen waypointsysteem.
            if (currentWayPointSystem == null)
            {
                currentWayPointSystem = currentSelected.transform.root.GetComponent<V3WayPointSystem>();
                // Het geselecteerde object maakt ook geen deel uit van waypointsysteem.
                if (currentWayPointSystem == null)
                {
                    return;
                }
                currentWPElement = GetChildGameObjectFromHighestLevel(currentSelected, currentSelected.transform.root.gameObject);
            }
            activeObjectPrefab = currentWayPointSystem.currentPrefab;
            lastActiveObjectPrefab = activeObjectPrefab;
            activeLoop = currentWayPointSystem.loop;
            lastLoop = activeLoop;
        }
    }

    private void OnSelectionChange()
    {
        UpdateSettings();
    }
}
