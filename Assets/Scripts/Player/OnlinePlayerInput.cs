using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnlinePlayerInput : MonoBehaviour
{
    PlayerControls controls;
    OnlinePlayer player;

    void Awake(){
        controls = new PlayerControls();
        player = GetComponent<OnlinePlayer>();
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
        player.direction = controls.Player.Move.ReadValue<float>();
    }
}
