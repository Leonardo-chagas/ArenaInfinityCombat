using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.Random;

public class ShotgunBullet : MonoBehaviour
{
    public float speed = 20;
    Rigidbody2D rb;
    public GameObject hitfx;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // rb.velocity = transform.right*speed*Random.Range(0.7f, 1.3f) + transform.up*Random.Range(-1.2f, 1.2f);
        Destroy(gameObject, 0.5f);
    }

    void OnTriggerEnter2D( Collider2D col ){
        if(col.gameObject.CompareTag("Grid")){
            GameObject hit = Instantiate(hitfx, transform.position, transform.rotation);
            Destroy(hit, 0.8f);
            Destroy(gameObject);
        }
        if(col.gameObject.CompareTag("Player")){
            GameObject hit = Instantiate(hitfx, transform.position, transform.rotation);
            Destroy(hit, 0.8f);
            Destroy(gameObject);
        }
    }
}
