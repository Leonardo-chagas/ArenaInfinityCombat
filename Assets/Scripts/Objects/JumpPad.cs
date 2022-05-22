using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPad : MonoBehaviour
{
    public float force = 15f;
    
    void OnTriggerEnter2D(Collider2D col){
        if(col.gameObject.CompareTag("Player")){
            Rigidbody2D playerRb = col.gameObject.GetComponent<Rigidbody2D>();

            playerRb.velocity = Vector2.zero;
            playerRb.AddForce(Vector2.up*force, ForceMode2D.Impulse);
        }
    }
}
