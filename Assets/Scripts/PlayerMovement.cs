using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private PlayerInputs defaultPlayerActions;
    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction attackAction;
    private InputAction blinkAction;

    private Rigidbody2D body;
    private float speed = 8f;

    private bool isGrounded = true;
    private bool alreadyCanceled = false;
    private float jumpForce = 10f;

    private void Awake() 
    {
        body = GetComponent<Rigidbody2D>();
        defaultPlayerActions = new PlayerInputs();        
    }

    private void OnEnable() 
    {
        moveAction = defaultPlayerActions.Player.Move;
        moveAction.Enable();

        jumpAction = defaultPlayerActions.Player.Jump;
        jumpAction.Enable();

        attackAction = defaultPlayerActions.Player.Attack;
        attackAction.Enable();

        blinkAction = defaultPlayerActions.Player.Blink;
        blinkAction.Enable();        
    }

    private void OnDisable() 
    {
        moveAction.Disable();
        jumpAction.Disable();
        attackAction.Disable();
        blinkAction.Disable();
    }

    public void OnAttack(InputAction.CallbackContext ctx)
    {
        //Debug.Log("Player attacked");
    }

    public void OnBlink(InputAction.CallbackContext ctx)
    {
        //Debug.Log("Player blinked");
    }

    //New variable-height jump for tighter controls
    public void OnJump(InputAction.CallbackContext ctx)
    { 
        if (ctx.started && isGrounded)
        {
            alreadyCanceled = false;
            body.velocity = new Vector2(body.velocity.x, jumpForce);
        }

        //Don't want to be able to press+release jump key again
        //which sets velocity to 0. Looks bad on the way down
        if (!alreadyCanceled)
        {
            if (ctx.canceled && body.velocity.y > 0)
            {
                body.velocity = new Vector2(body.velocity.x, 0);
                alreadyCanceled = true;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the player collided with a ground collider
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        // Check if the player is no longer colliding with the ground collider
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }


    private void FixedUpdate() 
    {
        Vector2 moveDir = moveAction.ReadValue<Vector2>();
        Vector2 vel = GetComponent<Rigidbody2D>().velocity;
        vel.x = speed * moveDir.x;

        body.velocity = vel;        
    }
}