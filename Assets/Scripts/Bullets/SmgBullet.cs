using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.Random;

public class SmgBullet : MonoBehaviour
{
    public Rigidbody2D rb;
    public GameObject hitfx;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, 0.3f);
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
