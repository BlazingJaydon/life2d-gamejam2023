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
        if (collision.gameObject.CompareTag("Ground"))
        {
            moveSpeed = -moveSpeed;
            FlipEnemyFacing();
        }
    }

    void FixedUpdate()
    {
        enemyBody.velocity = new Vector2(moveSpeed, 0);

        if (transform.GetComponent<EnemyVision>().detectPlayer(5f, 90f, 15, facingRight))
        {
            float newSpeed = chasingSpeed;
            if (moveSpeed < 0)
                newSpeed *= -1;
            moveSpeed = newSpeed;
        }
    }
}
