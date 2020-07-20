using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// Probeersel om te kijken of je modifiers op objecten kunt gebruiken zoals bijvoorbeeld in Blender.
/// </summary>
[ExecuteInEditMode]
public class ArrayModifier : MonoBehaviour
{
    private Renderer rend;
    private List<Component> components = new List<Component>();
    private int oldCount = 1;
    public int count = 1;
    private Vector3 oldOffset;
    public Vector3 offset;
    private Mesh oldMesh;
    private MeshFilter mesh;
    private GameObject array;

    private enum Mode
    {
        doNothing,
        addObjects,
        deleteObjects,
        updatePositions,
        updateMeshes
    }

    private Mode mode = Mode.doNothing;

    /// <summary>
    /// Kopieërt alle componenten van het ene naar het andere object.
    /// </summary>
    /// <param name="origObject"></param>
    /// <param name="destObject"></param>
    private void CopyComponents(GameObject origObject, GameObject destObject)
    {
        foreach(Component component in components)
        {
            var componentType = component.GetType();
            if (componentType == typeof(Transform))
            {
                UnityEditorInternal.ComponentUtility.PasteComponentValues(destObject.transform);
            }
            else if (componentType != typeof(ArrayModifier))
            {
                UnityEditorInternal.ComponentUtility.CopyComponent(component);
                UnityEditorInternal.ComponentUtility.PasteComponentAsNew(destObject);
            }
        }
    }

    private void Awake()
    {
        mesh = GetComponent<MeshFilter>();
        oldMesh = mesh.sharedMesh;
        Component[] tmpComp = GetComponents<Component>();
        foreach(Component comp in tmpComp)
        {
            components.Add(comp);
        }
    }

    /// <summary>
    /// Maakt een childobject aan.
    /// </summary>
    /// <returns></returns>
    private Transform CreateChild()
    {
        Transform t = new GameObject().transform;
        CopyComponents(gameObject, t.gameObject);
        t.parent = transform;
        return t;
    }

    /// <summary>
    /// Maakt alle childobjecten aan.
    /// </summary>
    private void CreateChildren()
    {
        for (int i = oldCount; i < count; i++)
        {
            Transform t = CreateChild();
            Vector3 vec = transform.position + offset * (t.GetSiblingIndex() + 1);
            t.position = vec;
            t.name = transform.name + i;
        }
    }

    /// <summary>
    /// Verwijdert alle childobjecten.
    /// </summary>
    private void DestroyChildren()
    {
        for (int i = transform.childCount - 1; i > count - 1; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }

    /// <summary>
    /// Werkt posities van childobjecten bij.
    /// </summary>
    private void UpdatePositions()
    {
        foreach(Transform t in transform)
        {
            Vector3 vec = transform.position + offset * (t.GetSiblingIndex() + 1);
            t.position = vec;
        }
    }

    /// <summary>
    /// Werkt meshes van childobjecten bij.
    /// </summary>
    private void UpdateMeshes()
    {
        foreach (Transform t in transform)
        {
            MeshFilter m = t.GetComponent<MeshFilter>();
            m.sharedMesh = mesh.sharedMesh;
        }
    }
    
    /// <summary>
    /// Bepaalt wat hij doet. Dit is alleen nodig omdat je destroy niet kunt aanroepen vanuit onvalidate
    /// </summary>
    private void Update()
    {
        switch(mode)
        {
            case Mode.addObjects:
                CreateChildren();
                oldCount = count;
                break;
            case Mode.deleteObjects:
                DestroyChildren();
                oldCount = count;
                break;
            case Mode.updatePositions:
                UpdatePositions();
                oldOffset = offset;
                break;
        }
        mode = Mode.doNothing;

        if (mesh.sharedMesh != oldMesh)
        {
            UpdateMeshes();
            oldMesh = mesh.sharedMesh;
        }
    }

    /// <summary>
    /// Werkt zijn instellingen bij.
    /// </summary>
    private void OnValidate()
    {
        count = Mathf.Clamp(count, 0, 100);
        if (count != oldCount)
        {
            if (count > oldCount)
            {
                mode = Mode.addObjects;
            }
            else
            {
                mode = Mode.deleteObjects;
            }
        }

        else if (offset != oldOffset)
        {
            mode = Mode.updatePositions;
        }
    }
}
