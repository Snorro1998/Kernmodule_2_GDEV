using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class ArrayModifier : MonoBehaviour
{
    Renderer rend;
    //Bounds ownSize = new Bounds();
    List<Component> components = new List<Component>();

    private int oldCount = 1;
    public int count = 1;

    Vector3 oldOffset;
    public Vector3 offset;

    private Mesh oldMesh;
    private MeshFilter mesh;

    private GameObject array;

    enum Mode
    {
        doNothing,
        addObjects,
        deleteObjects,
        updatePositions,
        updateMeshes
    }

    Mode mode = Mode.doNothing;

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

    void Awake()
    {
        mesh = GetComponent<MeshFilter>();
        oldMesh = mesh.sharedMesh;

        Component[] tmpComp = GetComponents<Component>();
        foreach(Component comp in tmpComp)
        {
            components.Add(comp);
        }
    }

    Transform CreateChild()
    {
        Transform t = new GameObject().transform;
        CopyComponents(gameObject, t.gameObject);
        t.parent = transform;
        return t;
    }

    void CreateChildren()
    {
        for (int i = oldCount; i < count; i++)
        {
            Transform t = CreateChild();
            Vector3 vec = transform.position + offset * (t.GetSiblingIndex() + 1);
            t.position = vec;
            t.name = transform.name + i;
        }
    }

    void DestroyChildren()
    {
        for (int i = transform.childCount - 1; i > count - 1; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }

    void UpdatePositions()
    {
        foreach(Transform t in transform)
        {
            Vector3 vec = transform.position + offset * (t.GetSiblingIndex() + 1);
            t.position = vec;
        }
    }

    void UpdateMeshes()
    {
        foreach (Transform t in transform)
        {
            MeshFilter m = t.GetComponent<MeshFilter>();
            m.sharedMesh = mesh.sharedMesh;
            /*
            Vector3 vec = transform.position + offset * (t.GetSiblingIndex() + 1);
            t.position = vec;*/
        }
    }

    //blijkbaar mag je niks verwijderen in onvalidate, anders was dit niet nodig
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
