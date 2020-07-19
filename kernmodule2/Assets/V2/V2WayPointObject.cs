using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class V2WayPointObject : MonoBehaviour
{
    public V2WayPointSystem wp;
    //niet bepaald een fraaie oplossing, maar in tegenstelling tot het gebruik van getsibling in onenable werkt dit wel
    public int indexInWayPoint = -1;
    private Vector3 lastPos;

    public Transform nextObj = null;
    public Transform prevObj = null;

    private void Awake()
    {
        lastPos = transform.position;
        if (indexInWayPoint != -1 && !wp.elements.Contains(this))
        {
            wp.elements.Insert(indexInWayPoint, this);
            wp.UpdateNamesAndIndicesFromIndex(indexInWayPoint);
        }
    }

    private void OnDestroy()
    {
        if (wp != null)
        {
            wp.elements.Remove(this);
            wp.UpdateNamesAndIndicesFromIndex(indexInWayPoint);
        }
    }

    public void UpdateSelf()
    {
        Debug.Log("UpdateSelf " + transform.name);
    }

    private void Update()
    {
        if (transform.position != lastPos)
        {
            Debug.Log("VERPLAATST");
            lastPos = transform.position;
            wp.UpdateFromIndex(indexInWayPoint);
        }
    }
}
