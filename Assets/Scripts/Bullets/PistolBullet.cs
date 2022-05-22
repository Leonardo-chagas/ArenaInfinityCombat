using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PistolBullet : MonoBehaviour
{
    Rigidbody2D rb;
    public GameObject hitfx;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // rb.velocity = transform.right*speed;
        Destroy(gameObject, 10f);
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
