using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolController : MonoBehaviour
{
    private float patrolSpeed = 2;
    private float chaseSpeed = 4;
    bool facingRight = true;

    float moveSpeed;
    private Rigidbody2D enemyBody;
    private EnemyVision vision;
    private GameObject player;
    private PlayerController playerManage;
    private EnemyManager enemyManager;

    private float DESPAWN_LIMIT = 1f;
    private float despawnTimer = 0f;

    public Animator animator;
    SpriteRenderer sprite;

    float viewDistance = 10f;

    //bool isConfused = false;
    bool canSeePlayer = false;
    float CONFUSED_LIMIT = 5f;
    float confusedTimer = 0f;
    float AGGRO_LIMIT = 6f; //number of seconds it will stay aggro'd when it doesn't see player
    float aggroTimer = 0f;

    bool pauseChase = false;
    float PAUSE_LIMIT = 0.5f;
    float pauseTimer = 0f;

    float currPosChecker;
    int sameXLimit = 10;
    int sameXCount = 0;

    bool playerBlinked = false;

    float previousX = -1f;

    // public GameObject pointA;
    // public GameObject pointB;

    private enum State 
    {
        Patrol,
        Chase,
        Confused,
        Attack,
        Dead
    }

    public void OnPlayerBlink()
    {
        if (state == State.Chase)
            playerBlinked = true;
    }

    private State state;
    
    private void Awake()
    {
        animator = GetComponent<Animator>();
        enemyBody = GetComponent<Rigidbody2D>();
        vision = GetComponent<EnemyVision>();
        
       
    }
    private void Start() 
    {
        moveSpeed = patrolSpeed;
        animator.SetBool("IsRunning", true);
        player = GameObject.FindWithTag("Player");
        playerManage = player.GetComponent<PlayerController>();
        enemyManager = transform.GetComponent<EnemyManager>();
        sprite = transform.GetComponent<SpriteRenderer>();
    }

    private void FlipEnemyFacing()
    {
        transform.localScale = new Vector2(-(transform.localScale.x), transform.localScale.y);
        facingRight = !facingRight;
        moveSpeed *= -1;
    }

    // If we want to use certain spots to turn instead of bumping into walls
    // private void OnTriggerEnter2D(Collider2D other) 
    // {
    //     Debug.Log("GOT HERE");
    //     if (state == State.Patrol && other.CompareTag("PatrolPoint"))
    //     {
    //         moveSpeed *= -1;
    //         FlipEnemyFacing();
    //     }
    // }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (state == State.Patrol && collision.collider.CompareTag("Ground"))
        {   
            FlipEnemyFacing();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    { 
    }

    private void stateCheck()
    {
        //If patrolling Slayer sees the Player -- cone of vision
        canSeePlayer = vision.detectPlayer(viewDistance, 90f, 15, facingRight);

        if (enemyManager.getHP() <= 0)
        {
            state = State.Dead;
            animator.SetTrigger("Death");

        }
        else
        {
            if (!canSeePlayer)
            {
                aggroTimer += Time.deltaTime;
                if (aggroTimer > AGGRO_LIMIT)
                {
                    //trigger the confused state -> then patrol
                    playerBlinked = true;
                    aggroTimer = 0;
                }
            }

            if ((state == State.Patrol || state == State.Confused) && canSeePlayer) 
            {
                playerBlinked = false;  
                if (!animator.GetBool("IsRunning"))
                    animator.SetBool("IsRunning", true);
                state = State.Chase;
                float newSpeed = chaseSpeed;
                if (!facingRight)
                    newSpeed *= -1;
                moveSpeed = newSpeed;
            }
            else if (state == State.Chase && playerBlinked)
            {
                state = State.Confused;
                animator.SetBool("IsRunning", false);
                confusedTimer = 0;
                enemyBody.velocity = new Vector2(0f, 0f);
            }
        }
    }

    private void doConfused()
    {
        confusedTimer += Time.deltaTime;     

        //If the timer elapsed, switch to patrol
        if (confusedTimer > CONFUSED_LIMIT)
        {
            state = State.Patrol;
            sameXCount = 0;
            float newSpeed = patrolSpeed;
            if (!facingRight)
                newSpeed *= -1;    
            moveSpeed = newSpeed;
            playerBlinked = false;
        }
    }

    private void doPatrol()
    {
        if (!animator.GetBool("IsRunning"))
                animator.SetBool("IsRunning", true);

        // Fixes monster getting stuck on ledge after chasing, since
        // there's no collision to get it to turn around. Should prob
        // do this with Time.deltaTime
        float currPosX = transform.position.x;
        if (sameXCount == 0)
        {
            currPosChecker = currPosX;
        }
        if (currPosChecker == currPosX)
        {
            sameXCount++;
            if (sameXCount >= sameXLimit)
            {
                FlipEnemyFacing();
                sameXCount = 0;
            }

        }
        else
            sameXCount = 0;
        previousX = currPosX;
        enemyBody.velocity = new Vector2(moveSpeed, 0);
    }

    private void doChase()
    {
        if (!pauseChase)
        {
            float nextDist = moveSpeed;
            float xDistToPlayer = player.transform.position.x - transform.position.x;

            Vector2 nextMove = new Vector2(nextDist, 0);
            if (xDistToPlayer < 0)
            {
                if (facingRight)
                {
                    FlipEnemyFacing();
                }
            }
            else if (xDistToPlayer > 0)
            {
                if (!facingRight)
                {
                    FlipEnemyFacing();
                }            
            }

            enemyBody.velocity = nextMove;     
        }
        else
            enemyBody.velocity = new Vector2(0f, 0f);
    }

    private void doDead()
    {
        enemyBody.velocity = new Vector2(0f, 0f);
        despawnTimer += Time.deltaTime;

        if (despawnTimer >= DESPAWN_LIMIT)
        {
            StartCoroutine(FadeOut());
        }
    }

    private IEnumerator FadeOut()
     {
         float alphaVal = sprite.color.a;
         Color tmp = sprite.color;
 
         while (sprite.color.a < 1)
         {
             alphaVal += 0.01f;
             tmp.a = alphaVal;
             sprite.color = tmp;
 
             yield return new WaitForSeconds(0.05f); // update interval
         }
         Destroy(transform.gameObject);
     }

    private void Update() 
    {
        stateCheck();

        //Debug.Log(state);
        if (pauseChase)
        {
            pauseTimer += Time.deltaTime;
            if (pauseTimer >= PAUSE_LIMIT)
            {
                pauseTimer = 0;
                pauseChase = false;
                animator.SetBool("IsRunning", true);
            }
        }

        switch (state) 
        {
            case State.Patrol:
                doPatrol();
                break;
            case State.Chase:
                doChase();      
                break;      
            case State.Confused:
                doConfused();
                break;
            case State.Dead:
                doDead();
                break;
        }

            // case State.Attack:
            //     doAttack();
    }

    public void setPause()
    {
        pauseChase = true;
        animator.SetBool("IsRunning", false);
    }


    // Drawing objects in editor for debugging
    // private void OnDrawGizmos() 
    // {
    //     Gizmos.DrawWireSphere(pointA.transform.position, 0.5f);
    //     Gizmos.DrawWireSphere(pointB.transform.position, 0.5f);

    //     Gizmos.DrawLine(pointA.transform.position, pointB.transform.position);
    // }
}