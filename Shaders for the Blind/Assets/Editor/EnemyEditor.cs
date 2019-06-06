using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EnemyController))]
public class EnemyEditor : Editor
{
    private void OnSceneGUI()
    {
        EnemyController e = target as EnemyController;
        if (e == null)
            return;

        if (e.pathPoints.Count == 0)
            return;

        Handles.color = Color.cyan;
        Vector3 lastPos = e.pathPoints[0].position;
        for (int i = 0; i < e.pathPoints.Count; ++i)
        {
            PathPoint pt = e.pathPoints[i];
            pt.position = Handles.PositionHandle(pt.position, Quaternion.identity);

            Handles.DrawLine(lastPos, pt.position);
            lastPos = pt.position;
            e.pathPoints[i] = pt;
        }
        Handles.DrawLine(lastPos, e.pathPoints[0].position);
    }
}
