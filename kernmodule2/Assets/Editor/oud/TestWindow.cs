//#define CHANGE_X_Z
//#define SHOWSELECTED

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TestWindow : EditorWindow
{
    Color colorX = Color.red;
    Color colorY = Color.green;
    Color colorZ = Color.blue;

    public List<GameObject> currentSelected = new List<GameObject>();

    [System.Serializable]
    public enum Axis
    {
        All,
        XAxis,
        YAxis,
        ZAxis,
        NegXAxis,
        NegYAxis,
        NegZAxis
    }

    public enum TargetObject
    {
        FirstSelected,
        LastSelected
    }

    public enum WindowMode
    {
        Extensive,
        Short
    }

    public Axis affectedAxis;
    public TargetObject target;
    public WindowMode windowMode;

    [MenuItem("Tools/Object Alignment")]
    private static void ShowWindow()
    {
        GetWindow<TestWindow>("Object Alignment");
    }

    private void OnSelectionChange()
    {
        UpdateSelectionList();
    }

    private void UpdateSelectionList()
    {
        GameObject currentObject = Selection.activeGameObject;

        if (!currentSelected.Contains(currentObject) && currentObject != null)
        {
            currentSelected.Add(currentObject);
        }

        foreach(GameObject gm in Selection.gameObjects)
        {
            if(!currentSelected.Contains(gm))
            {
                currentSelected.Add(gm);
            }
        }

        for (int i = 0; i < currentSelected.Count; i++)
        {
            if (!Selection.Contains(currentSelected[i]))
            {
                currentSelected.Remove(currentSelected[i]);
                i--;
            }
        }
    }

    private void OnGUI()
    {

        DrawCreatePrefabButton();
#if SHOWSELECTED
        SerializedObject objList = new SerializedObject(this);
        EditorGUILayout.PropertyField(objList.FindProperty("currentSelected"));
#endif
        SerializedObject winField = new SerializedObject(this);
        EditorGUILayout.PropertyField(winField.FindProperty("windowMode"));
        winField.ApplyModifiedProperties();
        EditorGUILayout.Space(10);

        switch(windowMode)
        {
            default:
                DrawAlignButtons();
                DrawStackButtons();
                DrawDuplicateButtons();
                DrawSnapButtons();
                break;
            case WindowMode.Extensive:
                DrawAlignButtonsExt();
                EditorGUILayout.Space(10);
                DrawStackButtonsExt();
                EditorGUILayout.Space(10);
                DrawDuplicateButtonsExt();
                EditorGUILayout.Space(10);
                DrawSnapButtonsExt();
                break;
        }
    }

    #region ButtonDisplay

    private void DisplayButton(string name, Action a)
    {
        if (GUILayout.Button(name))
        {
            a();
        }
    }

    private void DisplayButton(string name, Color col, Action a)
    {
        Color tmpCol = GUI.backgroundColor;
        GUI.backgroundColor = col;

        if (GUILayout.Button(name))
        {
            a();
        }

        GUI.backgroundColor = tmpCol;
    }

    private void DisplayXButton(Action a, bool positive)
    {
        DisplayButton(positive ? "X Axis" : "-X Axis", colorX, a);
    }

    private void DisplayYButton(Action a, bool positive)
    {
        DisplayButton(positive ? "Y Axis" : "-Y Axis", colorY, a);
    }

    private void DisplayZButton(Action a, bool positive)
    {
        DisplayButton(positive ? "Z Axis" : "-Z Axis", colorZ, a);
    }

    #endregion

    #region Snap

    private void SnapToSurface(GameObject gm, Vector3 dir)
    {
        MeshRenderer renderer = gm.GetComponent<MeshRenderer>();
        if (renderer != null)
        {
            RaycastHit rayhit;
            if (Physics.Raycast(renderer.bounds.center, dir, out rayhit))
            {
                Vector3 vec = gm.transform.position;
                int tmp = (dir == Vector3.right || dir == Vector3.forward || dir == Vector3.up) ? -1 : 1;

                //x-as
                if (dir == Vector3.right || dir == Vector3.left)
                {
                    float xPos = rayhit.point.x + tmp * renderer.bounds.extents.x;
                    vec = new Vector3(xPos, gm.transform.position.y, gm.transform.position.z);
                }

                //z-as
                else if (dir == Vector3.forward || dir == Vector3.back)
                {
                    float zPos = rayhit.point.z + tmp * renderer.bounds.extents.z;
                    vec = new Vector3(gm.transform.position.x, gm.transform.position.y, zPos);
                }

                //y-as
                else if (dir == Vector3.up || dir == Vector3.down)
                {
                    float distFromCenterOfBoundingBox = renderer.transform.position.y - renderer.bounds.center.y;
                    distFromCenterOfBoundingBox += tmp * renderer.bounds.extents.y;
                    float yPos = rayhit.point.y + distFromCenterOfBoundingBox;
                    vec = new Vector3(gm.transform.position.x, yPos, gm.transform.position.z);
                }

                gm.transform.position = vec;
            }
        }
    }

    private void SnapAllToSurface(Vector3 dir)
    {
        foreach (GameObject gm in currentSelected)
        {
            SnapToSurface(gm, dir);
        }
    }

    private void DrawSnapButtons()
    {

    }

    private void DrawSnapButtonsExt()
    {
        GUILayout.Label("Snap objects to nearest surface", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();

        DisplayXButton(() => { SnapAllToSurface(Vector3.right); }, true);
        DisplayYButton(() => { SnapAllToSurface(Vector3.up); }, true);
        DisplayZButton(() => { SnapAllToSurface(Vector3.forward); }, true);

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();

        DisplayXButton(() => { SnapAllToSurface(Vector3.left); }, false);
        DisplayYButton(() => { SnapAllToSurface(Vector3.down); }, false);
        DisplayZButton(() => { SnapAllToSurface(Vector3.back); }, false);

        EditorGUILayout.EndHorizontal();
    }

    #endregion

    #region Align

    private void AlignAllAxis()
    {
        Transform tTarget = Selection.activeTransform;

        foreach (Transform t in Selection.transforms)
        {
            t.position = tTarget.position;
        }
    }

    private void AlignXAxis()
    {
        Transform tTarget = currentSelected[currentSelected.Count - 1].transform;

#if (CHANGE_X_Z)
        float xPos = tTarget != null ? tTarget.position.z : 0;
#else
        float xPos = tTarget != null ? tTarget.position.x : 0;
#endif

        foreach (Transform t in Selection.transforms)
        {
            Vector3 tmpPos = t.position;

#if (CHANGE_X_Z)
            tmpPos.z = xPos;
#else
            tmpPos.x = xPos;
#endif
            t.position = tmpPos;
        }
    }

    private void AlignYAxis()
    {
        Transform tTarget = currentSelected[currentSelected.Count - 1].transform;
        float yPos = tTarget != null ? tTarget.position.y : 0;

        foreach (Transform t in Selection.transforms)
        {
            Vector3 tmpPos = t.position;
            tmpPos.y = yPos;
            t.position = tmpPos;
        }
    }

    private void AlignZAxis()
    {
        Transform tTarget = currentSelected[currentSelected.Count - 1].transform;

#if (CHANGE_X_Z)
        float zPos = tTarget != null ? tTarget.position.x : 0;
#else
        float zPos = tTarget != null ? tTarget.position.z : 0;
#endif

        foreach (Transform t in Selection.transforms)
        {
            Vector3 tmpPos = t.position;

#if (CHANGE_X_Z)
            tmpPos.x = zPos;
#else
            tmpPos.z = zPos;
#endif
            t.position = tmpPos;
        }
    }

    private void DrawAlignButtons()
    {
        SerializedObject axField = new SerializedObject(this);
        EditorGUILayout.PropertyField(axField.FindProperty("affectedAxis"));
        axField.ApplyModifiedProperties();

        SerializedObject tarField = new SerializedObject(this);
        EditorGUILayout.PropertyField(tarField.FindProperty("target"));
        tarField.ApplyModifiedProperties();

        EditorGUILayout.Space(10);

        if (GUILayout.Button("Align objects to target"))
        {
            switch (affectedAxis)
            {
                default:
                    AlignAllAxis();
                    break;
                case Axis.XAxis:
                    AlignXAxis();
                    break;
                case Axis.YAxis:
                    AlignYAxis();
                    break;
                case Axis.ZAxis:
                    AlignZAxis();
                    break;
            }
        }
    }

    private void DrawAlignButtonsExt()
    {
        GUILayout.Label("Align objects to last selected", EditorStyles.boldLabel);
        DisplayButton("All axis", () => { AlignAllAxis(); });

        EditorGUILayout.BeginHorizontal();

        DisplayXButton(() => { AlignXAxis(); }, true);
        DisplayYButton(() => { AlignYAxis(); }, true);
        DisplayZButton(() => { AlignZAxis(); }, true);

        EditorGUILayout.EndHorizontal();
    }

    #endregion

    #region Stack
    private void StackXAxis()
    {

    }

    private void StackYAxis()
    {
        for (int i = currentSelected.Count - 1; i > 0; i--)
        {
            Transform currentBottom = currentSelected[i].transform;
            Transform currentTop = currentSelected[i - 1].transform;
            MeshRenderer meshBottom = currentBottom.GetComponent<MeshRenderer>();
            MeshRenderer meshTop = currentTop.GetComponent<MeshRenderer>();

            if (meshBottom != null && meshTop != null)
            {
                Vector3 pos = currentBottom.position;
                pos.y += meshBottom.bounds.extents.y + meshTop.bounds.extents.y;
                currentTop.position = pos;
            }
        }
    }

    private void StackZAxis()
    {

    }
    
    private void DrawStackButtons()
    {
        if (GUILayout.Button("Stack objects on target"))
        {
            switch (affectedAxis)
            {
                default:
                    StackYAxis();
                    break;
                case Axis.XAxis:
                    StackXAxis();
                    break;
                case Axis.ZAxis:
                    StackZAxis();
                    break;
            }
        }
    }

    private void DrawStackButtonsExt()
    {
        GUILayout.Label("Stack objects on last selected", EditorStyles.boldLabel);
        Rect r = EditorGUILayout.BeginHorizontal();

        DisplayXButton(() => { StackXAxis(); }, true);
        DisplayYButton(() => { StackYAxis(); }, true);
        DisplayZButton(() => { StackZAxis(); }, true);

        /*
        if (GUILayout.Button("X-axis"))
        {
            StackXAxis();
        }

        if (GUILayout.Button("Y-axis"))
        {
            StackYAxis();
        }

        if (GUILayout.Button("Z-axis"))
        {
            StackZAxis();
        }*/

        EditorGUILayout.EndHorizontal();
    }

    #endregion

    #region Duplicate

    //positioneren moet hier nog aan toegevoegd worden
    private void DuplicateInDirection(Vector3 dir)
    {
        foreach(GameObject gm in currentSelected)
        {
            GameObject gmNew = Instantiate(gm, gm.transform.position + dir, gm.transform.rotation);
        }
    }

    private void DrawDuplicateButtons()
    {
        if (GUILayout.Button("Duplicate objects in direction"))
        {
            Vector3 dir;

            switch(affectedAxis)
            {
                default:
                    dir = Vector3.up;
                    break;
                case Axis.XAxis:
                    dir = Vector3.right;
                    break;
                case Axis.ZAxis:
                    dir = Vector3.forward;
                    break;
                case Axis.NegXAxis:
                    dir = Vector3.left;
                    break;
                case Axis.NegYAxis:
                    dir = Vector3.down;
                    break;
                case Axis.NegZAxis:
                    dir = Vector3.back;
                    break;
            }

            DuplicateInDirection(dir);
        }
    }

    private void DrawDuplicateButtonsExt()
    {
        GUILayout.Label("Duplicate objects in direction", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();

        DisplayXButton(() => { DuplicateInDirection(Vector3.right); }, true);
        DisplayYButton(() => { DuplicateInDirection(Vector3.up); }, true);
        DisplayZButton(() => { DuplicateInDirection(Vector3.forward); }, true);

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();

        DisplayXButton(() => { DuplicateInDirection(Vector3.left); }, false);
        DisplayYButton(() => { DuplicateInDirection(Vector3.down); }, false);
        DisplayZButton(() => { DuplicateInDirection(Vector3.back); }, false);

        EditorGUILayout.EndHorizontal();
    }

    #endregion

    private void CreatePrefabAsset()
    {
        if (Selection.activeObject != null)
        {
            GameObject gm = Selection.activeGameObject;
            if (gm.transform.parent == null)
            {
                Debug.Log("maakt prefab van object zonder parent");
            }
        }
    }

    private void DrawCreatePrefabButton()
    {
        if (GUILayout.Button("Create prefab"))
        {
            CreatePrefabAsset();
        }
    }
}
