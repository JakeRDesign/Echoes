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
    public bool pathLoops = false;
    public float moveSpeed = 10.0f;
    [Tooltip("Angle of view cone in radians")]
    [Range(0.0f, Mathf.PI)]
    public float viewCone = 0.1f;
    public float viewDistance = 5.0f;

    int pathIndex = 0;
    float pauseTimer = 0.0f;

    int increment = 1;

    Player target;

    private void Awake()
    {
        target = FindObjectOfType<Player>();
    }

    private void FixedUpdate()
    {
        DoMovement();

        transform.localScale = Vector3.one;
        if (CanSeePlayer())
            transform.localScale *= Mathf.Abs(Mathf.Sin(Time.time * 10.0f)) + 1.0f;
    }

    bool CanSeePlayer()
    {
        // check if the player is within the view cone
        if (GetAngleToPlayer() > viewCone)
            return false;
        // make sure the player is within the desired distance
        if (Vector3.Distance(target.transform.position, transform.position) >= viewDistance)
            return false;

        // make sure there isn't a wall in the way
        RaycastHit hit;
        if (!Physics.Raycast(transform.position + transform.forward, (target.transform.position - transform.position).normalized, out hit))
            return false;

        if (!hit.collider.CompareTag("Player"))
            return false;

        return true;
    }

    void DoMovement()
    {
        if (pathPoints.Count == 0)
            return;

        if (pauseTimer > 0.0f)
        {
            pauseTimer -= Time.fixedDeltaTime;
            return;
        }

        PathPoint thisPoint = pathPoints[pathIndex];

        Vector3 difference = thisPoint.position - transform.position;
        float mag = difference.magnitude;

        if (mag < 0.1f)
        {
            pathIndex += increment;
            if (!pathLoops)
            {
                if (pathIndex >= pathPoints.Count)
                {
                    pathIndex = pathPoints.Count - 1;
                    increment = -1;
                }
                if (pathIndex <= 0)
                {
                    pathIndex = 0;
                    increment = 1;
                }
            }

            pathIndex = Mathf.Abs(pathIndex % pathPoints.Count);
            pauseTimer = thisPoint.pauseTime;
            return;
        }

        if (!(difference.sqrMagnitude < 1.0f && thisPoint.pauseTime > 0.0f))
            difference.Normalize();

        Vector3 movement = difference * moveSpeed * Time.fixedDeltaTime;
        if (movement.magnitude > mag)
            movement = (movement.normalized * mag);
        transform.position += movement;

        // turn towards the direction we're moving

        // get angle from difference between current position and target
        float ang = Mathf.Rad2Deg * Mathf.Atan2(difference.x, difference.z);
        // turn that angle into a target quaternion
        Quaternion targetAng = Quaternion.Euler(0.0f, ang, 0.0f);
        // smoothly rotate between em
        transform.rotation = Quaternion.Lerp(transform.rotation, targetAng, Time.fixedDeltaTime * 5.0f * moveSpeed);
    }

    public float GetAngleToPlayer()
    {
        Vector3 dif = (target.transform.position - transform.position).normalized;
        Vector3 dir = transform.forward;
        float dot = Vector3.Dot(dif, dir);

        return Mathf.Acos(dot);
    }


}
