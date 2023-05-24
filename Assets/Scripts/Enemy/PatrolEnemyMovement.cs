using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolEnemyMovement : MonoBehaviour
{
    [SerializeField] float patrolSpeed;
    [SerializeField] float chasingSpeed;
    bool facingRight = true;

    float moveSpeed;
    Rigidbody2D enemyBody;

    public Animator animator;

    bool isPatrolling;
    bool isHunting;

    private void Awake()
    {
        animator.SetBool("isRunning", true);
        enemyBody = GetComponent<Rigidbody2D>();
        moveSpeed = patrolSpeed;
    }

    private void FlipEnemyFacing()
    {
        transform.localScale = new Vector2(-(transform.localScale.x), transform.localScale.y);
        facingRight = !facingRight;
    }

    private void OnTriggerExit2D(Collider2D collision) 
    {
        if (collision.gameObject.CompareTag("Ground") && isPatrolling)
        {
            moveSpeed = -moveSpeed;
            FlipEnemyFacing();
        }
        else if(collision.gameObject.CompareTag("Ground") && isHunting)
        {
            // Have the Slayer stand and wait for the player to come back to its level
            //moveSpeed = 0;
        }
    }

    private void statusCheck()
    {
        if (transform.GetComponent<EnemyVision>().detectPlayer(25f, 90f, 15, facingRight)) // If Slayer sees the Player
        {
            isHunting = true;
            isPatrolling = false;
        }
        else
        {
            isHunting = false;
            isPatrolling = true;
        }
    }

    private void patrol()
    {
        enemyBody.velocity = new Vector2(moveSpeed, 0);
    }

    private void hunt()
    {
        float newSpeed = chasingSpeed;
            if (moveSpeed < 0)
                newSpeed *= -1;

        enemyBody.velocity = new Vector2(newSpeed, 0);
    }

    private void Update() 
    {
        statusCheck();
    }

    void FixedUpdate()
    {
        if (isPatrolling)
        {
            patrol();
        }
        else if(isHunting)
        {
            hunt();
        }
    }

    private void LateUpdate() 
    {
        
    }
}
