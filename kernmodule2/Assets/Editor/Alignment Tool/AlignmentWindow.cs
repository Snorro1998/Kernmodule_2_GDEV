using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class AlignmentWindow : EditorWindow
{
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
    private GUIDrawer guiDrawer = new GUIDrawer();
    private ObjectAligner objAligner = new ObjectAligner();
    private ObjectSnapper objSnapper = new ObjectSnapper();
    private ObjectStacker objStacker = new ObjectStacker();
    private ObjectDuplicator objDuplicator = new ObjectDuplicator();

    [MenuItem("Tools/Object Alignment")]
    private static void ShowWindow()
    {
        GetWindow<AlignmentWindow>("Object Alignment");
    }

    private void OnSelectionChange()
    {
        UpdateSelectionList();
    }

    /// <summary>
    /// Werkt lijst van geselecteerde objecten bij.
    /// </summary>
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

    /// <summary>
    /// Zorgt voor de weergave van de UI.
    /// </summary>
    private void OnGUI()
    {
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

    #region Snap
    /// <summary>
    /// NOG NIET GEÏMPLEMENTEERD. Toont de beknopte versie van de snapbuttons.
    /// </summary>
    private void DrawSnapButtons()
    {

    }

    /// <summary>
    /// Toont de uitgebreide versie van de snapbuttons.
    /// </summary>
    private void DrawSnapButtonsExt()
    {
        GUILayout.Label("Snap objects to nearest surface", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();

        guiDrawer.DisplayXButton(() => { objSnapper.SnapAllToSurface(Vector3.right, currentSelected); }, true);
        guiDrawer.DisplayYButton(() => { objSnapper.SnapAllToSurface(Vector3.up, currentSelected); }, true);
        guiDrawer.DisplayZButton(() => { objSnapper.SnapAllToSurface(Vector3.forward, currentSelected); }, true);

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();

        guiDrawer.DisplayXButton(() => { objSnapper.SnapAllToSurface(Vector3.left, currentSelected); }, false);
        guiDrawer.DisplayYButton(() => { objSnapper.SnapAllToSurface(Vector3.down, currentSelected); }, false);
        guiDrawer.DisplayZButton(() => { objSnapper.SnapAllToSurface(Vector3.back, currentSelected); }, false);

        EditorGUILayout.EndHorizontal();
    }

    #endregion

    #region Align
    /// <summary>
    /// Toont de beknopte versie van de alignbuttons.
    /// </summary>
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
                    objAligner.AlignAllAxis();
                    break;
                case Axis.XAxis:
                    objAligner.AlignXAxis(currentSelected);
                    break;
                case Axis.YAxis:
                    objAligner.AlignYAxis(currentSelected);
                    break;
                case Axis.ZAxis:
                    objAligner.AlignZAxis(currentSelected);
                    break;
            }
        }
    }

    /// <summary>
    /// Toont de uitgebreide versie van de alignbuttons.
    /// </summary>
    private void DrawAlignButtonsExt()
    {
        GUILayout.Label("Align objects to last selected", EditorStyles.boldLabel);
        guiDrawer.DisplayButton("All axis", () => { objAligner.AlignAllAxis(); });

        EditorGUILayout.BeginHorizontal();

        guiDrawer.DisplayXButton(() => { objAligner.AlignXAxis(currentSelected); }, true);
        guiDrawer.DisplayYButton(() => { objAligner.AlignYAxis(currentSelected); }, true);
        guiDrawer.DisplayZButton(() => { objAligner.AlignZAxis(currentSelected); }, true);

        EditorGUILayout.EndHorizontal();
    }

    #endregion

    #region Stack
    /// <summary>
    /// Toont de beknopte versie van de stackbuttons.
    /// </summary>
    private void DrawStackButtons()
    {
        if (GUILayout.Button("Stack objects on target"))
        {
            switch (affectedAxis)
            {
                default:
                    objStacker.StackYAxis(currentSelected);
                    break;
                case Axis.XAxis:
                    objStacker.StackXAxis(currentSelected);
                    break;
                case Axis.ZAxis:
                    objStacker.StackZAxis(currentSelected);
                    break;
            }
        }
    }

    /// <summary>
    /// Toont de uitgebreide versie van de stackbuttons.
    /// </summary>
    private void DrawStackButtonsExt()
    {
        GUILayout.Label("Stack objects on last selected", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        guiDrawer.DisplayXButton(() => { objStacker.StackXAxis(currentSelected); }, true);
        guiDrawer.DisplayYButton(() => { objStacker.StackYAxis(currentSelected); }, true);
        guiDrawer.DisplayZButton(() => { objStacker.StackZAxis(currentSelected); }, true);
        EditorGUILayout.EndHorizontal();
    }

    #endregion

    #region Duplicate
    /// <summary>
    /// Toont de beknopte versie van de duplicatebuttons.
    /// </summary>
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
            objDuplicator.DuplicateInDirection(dir, currentSelected);
        }
    }

    /// <summary>
    /// Toont de uitgebreide versie van de duplicatebuttons.
    /// </summary>
    private void DrawDuplicateButtonsExt()
    {
        GUILayout.Label("Duplicate objects in direction", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        guiDrawer.DisplayXButton(() => { objDuplicator.DuplicateInDirection(Vector3.right, currentSelected); }, true);
        guiDrawer.DisplayYButton(() => { objDuplicator.DuplicateInDirection(Vector3.up, currentSelected); }, true);
        guiDrawer.DisplayZButton(() => { objDuplicator.DuplicateInDirection(Vector3.forward, currentSelected); }, true);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        guiDrawer.DisplayXButton(() => { objDuplicator.DuplicateInDirection(Vector3.left, currentSelected); }, false);
        guiDrawer.DisplayYButton(() => { objDuplicator.DuplicateInDirection(Vector3.down, currentSelected); }, false);
        guiDrawer.DisplayZButton(() => { objDuplicator.DuplicateInDirection(Vector3.back, currentSelected); }, false);
        EditorGUILayout.EndHorizontal();
    }
    #endregion
}
