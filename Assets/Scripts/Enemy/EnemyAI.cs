using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyAI : MonoBehaviour
{
    [Header("Pathfinding")]
    public Transform target;
    public float activateDistance = 50f;
    public float pathUpdateSeconds = 0.5f;

    [Header("Physics")]
    public float speed = 200f;
    public float nextWaypointDistance = 3f;
    public float jumpNodeHeightRequirement = 0.8f;
    public float jumpModifier = 0.3f;
    public float jumpCheckOffset = 0.1f;

    [Header("Custom Behaivor")]
    public bool followEnabled = true;
    public bool jumpEnabled = true;
    public bool directionLookEnabled = true;

    private Path path;
    private int currentWaypoint = 0;
    bool isGrounded = false;
    Seeker seeker;
    Rigidbody2D enemyBody;

    private void Start()
    {
        seeker = GetComponent<Seeker>();
        enemyBody = GetComponent<Rigidbody2D>();

        InvokeRepeating("Updatepath", 0f, pathUpdateSeconds);
    }

    private void FixedUpdate() 
    {
        if (TargetInDistance() && followEnabled)
        {
            PathFollow();
        }
    }

    private void Updatepath()
    {
        if (TargetInDistance() && followEnabled && seeker.IsDone())
        {
            seeker.StartPath(enemyBody.position, target.position, OnPathComplete);
        }
    }

    private void PathFollow()
    {
        if (path == null)
        {
            return;
        }

        //Reached end of path
        if (currentWaypoint >= path.vectorPath.Count)
        {
            return;
        }

        //See if colliding with anything
        Vector3 startOffset = transform.position - new Vector3(0f, GetComponent<Collider2D>().bounds.extents.y + jumpCheckOffset);
        isGrounded = Physics2D.Raycast(startOffset, -Vector3.up, 0.05f);

        // Direction Calculation
        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - enemyBody.position).normalized;
        Vector2 force = direction * speed *Time.deltaTime;

        // Jump
        if (jumpEnabled && isGrounded)
        {
            if(direction.y > jumpNodeHeightRequirement)
            {
                enemyBody.AddForce(Vector2.up * speed * jumpModifier);
            }
        }

        // Movement
        enemyBody.AddForce(force);

        // Next Waypoint
        float distance = Vector2.Distance(enemyBody.position, path.vectorPath[currentWaypoint]);
        if(distance < nextWaypointDistance)
        {
            currentWaypoint++;
        }

        //Direction Graphics Handling
        if (directionLookEnabled)
        {
            if (enemyBody.velocity.x > 0.05f)
            {
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            else if (enemyBody.velocity.x < -0.05f)
            {
                transform.localScale = new Vector3(-1f * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
        }
    }

    private bool TargetInDistance()
    {
        return Vector2.Distance(transform.position, target.transform.position) < activateDistance;
    }

    private void OnPathComplete(Path p)
    {
        path = p;
        currentWaypoint = 0;
    }
}
