using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class WayPointWindow : EditorWindow
{
    public GameObject currentWPElement = null;
    public GameObject activeObjectPrefab = null;
    public GameObject lastActiveObjectPrefab = null;

    private bool activeLoop = false;
    private bool lastLoop = false;

    //private
    public WayPointSystem currentWayPointSystem = null;

    [MenuItem("Tools/Waypoint Editor")]
    public static void Open()
    {
        GetWindow<WayPointWindow>();
    }

    private void OnGUI()
    {
        //om te testen, kan later weggehaald worden
        SerializedObject objj = new SerializedObject(this);
        EditorGUILayout.PropertyField(objj.FindProperty("currentWayPointSystem"));
        objj.ApplyModifiedProperties();

        //om te testen, kan later weggehaald worden
        SerializedObject objjj = new SerializedObject(this);
        EditorGUILayout.PropertyField(objjj.FindProperty("currentWPElement"));
        objjj.ApplyModifiedProperties();

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
                CreateNewWayPointSystem();
            }
        }
        else
        {
            activeLoop = EditorGUILayout.Toggle("Loop", activeLoop);
            GUILayout.Label("Create new object", EditorStyles.boldLabel);

            if (GUILayout.Button("At the end"))
            {
                currentWayPointSystem.AppendWayPoint();
            }
            if (GUILayout.Button("At the beginning"))
            {
                currentWayPointSystem.PrependWayPoint();
            }
            if (currentWPElement != null)
            {
                if (GUILayout.Button("After selected"))
                {
                    currentWayPointSystem.InsertWayPointAfter(currentWPElement.transform);
                }         
                if (GUILayout.Button("Before selected"))
                {
                    currentWayPointSystem.InsertWayPointBefore(currentWPElement.transform);
                }
            }
        }     
    }

    private void Update()
    {
        if (currentWayPointSystem != null)
        {
            //prefab is aangepast
            if (lastActiveObjectPrefab != activeObjectPrefab)
            {
                lastActiveObjectPrefab = activeObjectPrefab;
                currentWayPointSystem.currentPrefab = activeObjectPrefab;
                currentWayPointSystem.UpdatePrefabOfChildren();
            }
            //loopoptie is aangepast
            if (activeLoop != lastLoop)
            {
                lastLoop = activeLoop;
                currentWayPointSystem.SetLoopAround(activeLoop);
            }
        }   
    }

    /// <summary>
    /// Pakt het object dat een niveau onder de root zit
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

    private void CreateNewWayPointSystem()
    {
        Vector3 spawnPos = CalcSpawnPosition();
        GameObject gm = new GameObject("WayPointSystem");
        gm.transform.position = spawnPos;
        currentWayPointSystem = gm.AddComponent<WayPointSystem>();
        currentWayPointSystem.currentPrefab = activeObjectPrefab;
        currentWayPointSystem.AppendWayPoint();
    }

    private void UpdateSettings()
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
            
            activeLoop = currentWayPointSystem.loop;
            lastLoop = activeLoop;
        }
    }

    private void OnSelectionChange()
    {
        UpdateSettings();
    }
}
