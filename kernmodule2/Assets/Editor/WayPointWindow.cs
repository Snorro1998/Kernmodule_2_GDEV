using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class WayPointWindow : EditorWindow
{
    //public GameObject currentSelected = null;
    public GameObject currentWPElement = null;
    public GameObject activeObjectPrefab = null;
    public GameObject lastActiveObjectPrefab = null;
    //private
    public WayPointSystem currentWayPointSystem = null;

    [MenuItem("Tools/Waypoint Editor")]
    public static void Open()
    {
        GetWindow<WayPointWindow>();
    }

    private void OnGUI()
    {
        /*
        SerializedObject objj = new SerializedObject(this);
        EditorGUILayout.PropertyField(objj.FindProperty("currentWayPointSystem"));
        objj.ApplyModifiedProperties();

        SerializedObject objjj = new SerializedObject(this);
        EditorGUILayout.PropertyField(objjj.FindProperty("currentWPElement"));
        objjj.ApplyModifiedProperties();*/

        SerializedObject obj = new SerializedObject(this);
        EditorGUILayout.PropertyField(obj.FindProperty("activeObjectPrefab"));
        obj.ApplyModifiedProperties();

        if (currentWayPointSystem == null)
        {
            if (activeObjectPrefab == null)
            {
                GUILayout.Label("No prefab selected", EditorStyles.boldLabel);
            }
            else if (GUILayout.Button("Create new waypoint system"))
            {
                CreateNewWayPoint();
            }
        }

        else
        {
            GUILayout.Label("Create new object", EditorStyles.boldLabel);

            if (GUILayout.Button("At the end"))
            {
                //CreateWaypoint();
            }

            if (GUILayout.Button("At the beginning"))
            {
                //Selection.activeObject = powerPoleRoot.GetChild(0);
                //CreateWaypointBefore();
            }
        }     
    }

    private void Update()
    {
        if (lastActiveObjectPrefab != activeObjectPrefab && currentWayPointSystem != null)
        {
            //Debug.Log("prefab is aangepast");
            lastActiveObjectPrefab = activeObjectPrefab;
            currentWayPointSystem.currentPrefab = activeObjectPrefab;
            currentWayPointSystem.UpdatePrefabOfChildren();
        }
    }

    private GameObject GetChildGameObjectFromHighestLevel(GameObject obj, GameObject par)
    {
        Transform tmpTransform = obj.transform;
        Transform tmpTransformPar = par.transform;
        for (int i = 0; i < 100; i++)
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
    /// Berekent spawnpositie afhankelijk van de camera-orientatie
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

            //kijkt naar de grond
            if (camAngle < 90)
            {
                float vectorMagnitude = sceneCam.transform.position.y / Mathf.Cos(Mathf.Deg2Rad * camAngle);
                spawnPos = sceneCam.transform.position + sceneCam.transform.forward * vectorMagnitude;
            }
        }
        return spawnPos;
    }

    private void CreateNewWayPoint()
    {
        Vector3 spawnPos = CalcSpawnPosition();
        GameObject gm = new GameObject("WayPointSystem");
        gm.transform.position = spawnPos;
        currentWayPointSystem = gm.AddComponent<WayPointSystem>();
        currentWayPointSystem.currentPrefab = activeObjectPrefab;
    }

    private void UpdateCurrentPrefab()
    {
        currentWPElement = null;
        currentWayPointSystem = null;
        GameObject currentSelected = Selection.activeGameObject;
        lastActiveObjectPrefab = activeObjectPrefab;

        //er is iets geselecteerd
        if (currentSelected != null)
        {
            currentWayPointSystem = currentSelected.GetComponent<WayPointSystem>();
            //geselecteerde object is geen waypointsysteem
            if (currentWayPointSystem == null)
            {
                currentWayPointSystem = currentSelected.transform.root.GetComponent<WayPointSystem>();
                //geselecteerde object maakt ook geen deel uit van waypointsysteem
                if (currentWayPointSystem == null)
                {
                    return;
                }
                currentWPElement = GetChildGameObjectFromHighestLevel(currentSelected, currentSelected.transform.root.gameObject);
            }
            activeObjectPrefab = currentWayPointSystem.currentPrefab;
            lastActiveObjectPrefab = activeObjectPrefab;
        }

        /*
        if (currentSelected != null)
        {
            currentWayPointSystem = currentSelected.GetComponent<WayPointSystem>();
            if (currentWayPointSystem != null)
            {
                activeObjectPrefab = currentWayPointSystem.currentPrefab;
                lastActiveObjectPrefab = activeObjectPrefab;
                return;
            }
            if (currentSelected.transform.parent != null)
            {
                currentSelected = currentSelected.transform.root.gameObject;
            }
            currentWayPointSystem = currentSelected.GetComponent<WayPointSystem>();
            if (currentWayPointSystem != null)
            {
                activeObjectPrefab = currentWayPointSystem.currentPrefab;
                lastActiveObjectPrefab = activeObjectPrefab;
            }
            else
            {
                currentSelected = null;
            }
        }
        */
    }

    private void OnSelectionChange()
    {
        UpdateCurrentPrefab();
    }
}
