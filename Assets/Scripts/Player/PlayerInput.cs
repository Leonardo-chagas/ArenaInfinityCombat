using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    PlayerControls controls;
    Player2 player;

    void Awake(){
        controls = new PlayerControls();
        player = GetComponent<Player2>();
    }

    void OnEnable(){
        controls.Enable();
    }

    void OnDisable(){
        controls.Disable();
    }

    void Start()
    {
        controls.Player.Jump.performed += _ => player.Jump();
        controls.Player.Pickup.performed += _ => player.PickUpGun();
        controls.Player.Shoot.performed += _ => player.Shoot();
        controls.Player.Shoot.canceled += _ => player.StopShooting();
        controls.Player.Dash.performed += _ => player.Dash();
        
    }

    void Update(){
        // enemyHealth = GetComponent <EnemyHealth>();
        if (player == null)
        {
            Debug.LogError("No Player component found.");  
        }else{
            player.direction = controls.Player.Move.ReadValue<float>();
        }
    }
}
