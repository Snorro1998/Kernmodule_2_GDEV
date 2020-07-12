using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// Oud stuk code om waypoints te visualiseren
/// </summary>
public class WayPointEditor : MonoBehaviour
{
    [DrawGizmo(GizmoType.NonSelected | GizmoType.Selected | GizmoType.Pickable)]
    public static void OnDrawSceneGizmo(WayPoint waypoint, GizmoType gizmoType)
    {
        if((gizmoType & GizmoType.Selected) != 0)
        {
            Gizmos.color = Color.yellow;
        }

        else
        {
            Gizmos.color = Color.yellow * 0.5f;
        }

        Gizmos.DrawSphere(waypoint.transform.position, .1f);

        Gizmos.color = Color.white;
        Gizmos.DrawLine(waypoint.transform.position + (waypoint.transform.right * waypoint.width / 2f),
            waypoint.transform.position - (waypoint.transform.right * waypoint.width / 2f));

        if(waypoint.previousWayPoint != null)
        {
            Gizmos.color = Color.red;
            Vector3 offset = waypoint.transform.right * waypoint.width / 2f;
            Vector3 offsetTo = waypoint.previousWayPoint.transform.right * waypoint.previousWayPoint.width / 2f;

            Gizmos.DrawLine(waypoint.transform.position + offset, waypoint.previousWayPoint.transform.position + offsetTo);
        }

        if (waypoint.nextWayPoint != null)
        {
            Gizmos.color = Color.green;
            Vector3 offset = waypoint.transform.right * -waypoint.width / 2f;
            Vector3 offsetTo = waypoint.nextWayPoint.transform.right * -waypoint.nextWayPoint.width / 2f;

            Gizmos.DrawLine(waypoint.transform.position + offset, waypoint.nextWayPoint.transform.position + offsetTo);
        }
    }
}
