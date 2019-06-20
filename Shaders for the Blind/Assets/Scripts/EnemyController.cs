using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

[System.Serializable]
public struct PathPoint
{
    public Vector3 position;
    public float pauseTime;
}

public class EnemyController : MonoBehaviour
{

    public Animator animator;
    public List<PathPoint> pathPoints;
    public bool pathLoops = false;
    public float moveSpeed = 10.0f;
    public float rotateSpeed = 2.0f;
    [Tooltip("Angle of view cone in radians")]
    [Range(0.0f, Mathf.PI)]
    public float viewCone = 0.1f;
    public float viewDistance = 5.0f;
    private float halfHeight = 1.0f;

    int pathIndex = 0;
    float pauseTimer = 0.0f;

    bool sawPlayer = false;
    [Header("Time before it starts persuing the player")]
    public float alertTime = 2.0f;
    float seePause = 0.0f;

    int increment = 1;

    Player target;

    NavMeshPath path;

    private void Awake()
    {
        target = FindObjectOfType<Player>();
        path = new NavMeshPath();
    }

    private void FixedUpdate()
    {
        transform.localScale = Vector3.one;
        if (CanSeePlayer())
        {
            if (sawPlayer)
            {
                seePause -= Time.fixedDeltaTime;
                if (seePause > 0)
                {
                    // look at player while we are paused

                    Vector3 difference = target.transform.position - transform.position;
                    // get angle from difference between current position and target
                    float ang = Mathf.Rad2Deg * Mathf.Atan2(difference.x, difference.z);
                    // turn that angle into a target quaternion
                    Quaternion targetAng = Quaternion.Euler(0.0f, ang, 0.0f);
                    // smoothly rotate between em
                    transform.rotation = Quaternion.Lerp(transform.rotation, targetAng, Time.fixedDeltaTime * 5.0f * moveSpeed);
                    animator.SetBool("walking", false);
                }
                else
                {
                    // find player
                    Vector3 movePoint = transform.position;

                    if (NavMesh.CalculatePath(transform.position, target.transform.position, ~0, path))
                    {
                        if (path.corners.Length > 1)
                            movePoint = path.corners[1] + (Vector3.up * halfHeight);
                    }

                    MoveTowards(movePoint);
                }
            }
            else
            {
                seePause = alertTime;
                sawPlayer = true;
                target.Noticed(this);
            }
        }
        else
        {
            if (sawPlayer)
            {
                target.UnNoticed();
                sawPlayer = false;
                pauseTimer = 2.0f;
            }
            DoMovement();
        }
    }

    bool CanSeePlayer()
    {
        //Debug.Log(Vector3.Distance(transform.position, target.transform.position));
        // little hack so it moves into the player when it's super close
        if (Vector3.Distance(target.transform.position, transform.position) < 2.0f)
            return true;

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

            animator.SetBool("walking", false);
            return;
        }

        PathPoint thisPoint = pathPoints[pathIndex];

        Vector3 movePoint = thisPoint.position;

        if (NavMesh.CalculatePath(transform.position, thisPoint.position, ~0, path))
        {
            if (path.corners.Length > 1)
                movePoint = path.corners[1] + (Vector3.up * halfHeight);
            if (Vector3.Distance(movePoint, thisPoint.position) < 1.0f)
                movePoint = thisPoint.position;
        }

        Vector3 difference = thisPoint.position - transform.position;
        float mag = difference.magnitude;

        if (mag < 0.15f)
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

        MoveTowards(movePoint, thisPoint.pauseTime > 0.0f);

    }

    public float GetAngleToPlayer()
    {
        Vector3 dif = (target.transform.position - transform.position).normalized;
        Vector3 dir = transform.forward;
        float dot = Vector3.Dot(dif, dir);

        return Mathf.Acos(dot);
    }

    void MoveTowards(Vector3 point, bool normalize = false)
    {
        Vector3 difference = point - transform.position;
        float mag = difference.magnitude;

        if (!(difference.sqrMagnitude < 1.0f && normalize))
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
        transform.rotation = Quaternion.Lerp(transform.rotation, targetAng, Time.fixedDeltaTime * rotateSpeed);

        animator.SetBool("walking", true);
    }


}
