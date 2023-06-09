using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMirror : MonoBehaviour
{
    private GameObject Player;
    private float DIMENSION_DIF;

    private LevelManager level;
    PlayerController playerControl;

    private void Awake() 
    {
        level = GameObject.FindGameObjectsWithTag("LevelManager")[0].GetComponent<LevelManager>();
        Player = GameObject.FindGameObjectsWithTag("Player")[0];
        playerControl = Player.GetComponent<PlayerController>();
        DIMENSION_DIF = -21.45f;
    }

    private void Start() 
    {
        
    }
    
    private void FixedUpdate() 
    {
        Vector3 playerPos = Player.transform.position;
        //Copy player position at all times at a DIMENSION_DIF interval
        transform.position = new Vector3(playerPos.x, playerPos.y + DIMENSION_DIF, 0);               
    }

    public void OnBlink(InputAction.CallbackContext ctx)
    {
        if (ctx.started && !playerControl.isPeeking)
            dimensionFlip();
    }

    public void dimensionFlip()
    {
        DIMENSION_DIF *= -1;
    }
}
