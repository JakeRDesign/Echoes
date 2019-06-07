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

        // draw view cone
        Vector3 pos = e.transform.position;
        float ang = e.transform.rotation.eulerAngles.y * Mathf.Deg2Rad;

        Handles.color = new Color(1.0f, 0.0f, 0.0f, 0.1f);
        Vector3 leftBound = new Vector3(Mathf.Sin(ang - e.viewCone), 0.0f, Mathf.Cos(ang - e.viewCone));
        Handles.DrawSolidArc(pos, Vector3.up, leftBound, e.viewCone*Mathf.Rad2Deg*2.0f, e.viewDistance);

        Vector3 rightBound = new Vector3(Mathf.Sin(ang + e.viewCone), 0.0f, Mathf.Cos(ang + e.viewCone));
        //Vector3 leftBound = new Vector3(Mathf.Sin(ang - e.viewCone), 0.0f, Mathf.Cos(ang - e.viewCone));

        Vector3 endLeft = pos + (leftBound * e.viewDistance);
        Vector3 endRight = pos + (rightBound * e.viewDistance);

        Handles.color = Color.red;
        Handles.DrawLine(pos, endLeft);
        Handles.DrawLine(pos, endRight);

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
        if (e.pathLoops)
            Handles.DrawLine(lastPos, e.pathPoints[0].position);
    }
}
