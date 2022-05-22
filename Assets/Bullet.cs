using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    public Rigidbody2D rb;
    public GameObject impactEffect;

    // Start is called before the first frame update
    void Start()
    {
        rb.velocity = transform.right * speed;
        // Debug.Log("gggg");
        Destroy(gameObject, 10f);
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        
        Debug.Log(collider.name);
        if ( !collider.CompareTag("gun") )
        {
            GameObject impactEffect_X = Instantiate(impactEffect, transform.position, transform.rotation);   
            Destroy(impactEffect_X, 0.04f);

            if ( collider.CompareTag("Grid") )
            {
                Debug.Log("colidiu com o Grid");
                Destroy(gameObject);
            }
            if ( collider.CompareTag("Player") )
            {
                Debug.Log("colidiu com o Player");
                Destroy(gameObject);
            }
        }
        
    }

}
