using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class PathPoint : MonoBehaviour
{
    public int pathId;
    public PathPoint next;

    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}

public class PathPointEditor
{
    [DrawGizmo(GizmoType.Active | GizmoType.Selected | GizmoType.NotInSelectionHierarchy)]
    static void DrawGizmoForPathPoint(PathPoint pathPoint, GizmoType gizmoType)
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(pathPoint.transform.position, 0.2f);

        if (pathPoint.next != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(pathPoint.transform.position, pathPoint.next.transform.position);
        }
    }
}
