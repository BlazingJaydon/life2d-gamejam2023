using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolEnemyMovement : MonoBehaviour
{
    [SerializeField] float patrolSpeed;
    [SerializeField] float chasingSpeed;
    bool facingRight = true;

    float moveSpeed;
    private Rigidbody2D enemyBody;

    public Animator animator;

    bool isPatrolling;
    bool isHunting;
    bool caught;

    public GameObject pointA;
    public GameObject pointB;
    public GameObject player;
    public Transform currentPoint;
    
    private void Awake()
    {
        animator = GetComponent<Animator>();
        enemyBody = GetComponent<Rigidbody2D>();
        moveSpeed = patrolSpeed;

        // set points to have same y as enemy
        pointA.transform.position = new Vector2 (pointA.transform.position.x, transform.position.y);
        pointB.transform.position = new Vector2 (pointB.transform.position.x, transform.position.y);
        //player.transform.position = new Vector2 (player.transform.position.x, transform.position.y);
    }
    private void Start() 
    {
        animator.SetBool("isRunning", true);
        currentPoint = pointB.transform;
    }

    private void FlipEnemyFacing()
    {
        transform.localScale = new Vector2(-(transform.localScale.x), transform.localScale.y);
        facingRight = !facingRight;
    }

    private void OnTriggerExit2D(Collider2D collision) 
    {
        /*
        if(collision.gameObject.CompareTag("Ground") && isPatrolling)
        {
            FlipEnemyFacing();
            moveSpeed = -moveSpeed;
        }
        */
    }

    private void OnCollisionEnter2D(Collision2D collision) 
    {
        if(collision.gameObject.CompareTag("Player") && currentPoint == player.transform && isHunting)
        {
            caught = true;
            isHunting = false;
            enemyBody.velocity = new Vector2(0, 0);
            animator.SetBool("isRunning", false);
        }
    }

    private void statusCheck()
    {
        if (transform.GetComponent<EnemyVision>().detectPlayer(25f, 90f, 15, facingRight) && !caught) // If Slayer sees the Player -- cone of vision
        {
            isHunting = true;
            isPatrolling = false;
            currentPoint = player.transform;
        }
        else
        {
            Debug.Log("Slayer doesn't see the player");
            isHunting = false;
            isPatrolling = true;
        }
    }

    private void search()
    {
        
        //move towards points
        if (currentPoint == pointB.transform && isPatrolling)
        {
            enemyBody.velocity = new Vector2(moveSpeed, 0);
        }
        else if (currentPoint == pointA.transform && isPatrolling)
        {
            enemyBody.velocity = new Vector2(-moveSpeed, 0);
        }
        else if (currentPoint == player.transform && isHunting)
        {
            float newSpeed = chasingSpeed;
            if (!facingRight)
                newSpeed *= -1;

            enemyBody.velocity = new Vector2(newSpeed, 0);
        }

        // setting up the points
        if (Vector2.Distance(transform.position, currentPoint.position) < 0.05f && currentPoint == pointB.transform && isPatrolling)
        {
            FlipEnemyFacing();
            currentPoint = pointA.transform;
        }
        if (Vector2.Distance(transform.position, currentPoint.position) < 0.05f && currentPoint == pointA.transform && isPatrolling)
        {
            FlipEnemyFacing();
            currentPoint = pointB.transform;
        }
        
        //enemyBody.velocity = new Vector2(moveSpeed, 0);
    }

    private void Update() 
    {
        statusCheck();

        search();
    }

private void OnDrawGizmos() 
{
    Gizmos.DrawWireSphere(pointA.transform.position, 0.5f);
    Gizmos.DrawWireSphere(pointB.transform.position, 0.5f);

    Gizmos.DrawLine(pointA.transform.position, pointB.transform.position);
}
}
