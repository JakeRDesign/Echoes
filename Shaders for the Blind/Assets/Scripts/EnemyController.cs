using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct PathPoint
{
    public Vector3 position;
    public float pauseTime;
}

public class EnemyController : MonoBehaviour
{

    public List<PathPoint> pathPoints;
    public float moveSpeed = 10.0f;

    int pathIndex = 0;
    float pauseTimer = 0.0f;

    private void FixedUpdate()
    {
        if (pathPoints.Count == 0)
            return;

        if(pauseTimer > 0.0f)
        {
            pauseTimer -= Time.fixedDeltaTime;
            return;
        }

        PathPoint thisPoint = pathPoints[pathIndex];

        Vector3 difference = thisPoint.position - transform.position;
        float mag = difference.magnitude;

        if(mag < 0.1f)
        {
            pathIndex = (pathIndex + 1) % pathPoints.Count;
            pauseTimer = thisPoint.pauseTime;
            return;
        }

        difference.Normalize();

        Vector3 movement = difference * moveSpeed;
        transform.position += movement * Time.fixedDeltaTime;
    }


}
