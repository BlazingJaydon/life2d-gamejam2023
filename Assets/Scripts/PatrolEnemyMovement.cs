using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolEnemyMovement : MonoBehaviour
{
    [SerializeField] float moveSpeed = 1f;
    Rigidbody2D enemyBody;

    private void Awake()
    {
        enemyBody = GetComponent<Rigidbody2D>();
    }

    private void FlipEnemyFacing()
    {
        transform.localScale = new Vector2(-(transform.localScale.x), transform.localScale.y);
    }


    private void OnTriggerExit2D(Collider2D collision) 
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            moveSpeed = -moveSpeed;
            FlipEnemyFacing();
        }
    }

    void Update()
    {
        enemyBody.velocity = new Vector2(moveSpeed,0f);
    }
}
