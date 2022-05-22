using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Smg : MonoBehaviour
{
    Gun gun;
    public GameObject bullet;
    public Transform firepoint;
    public float firerate = 0.1f;
    public float X_speed = 8f;
    public float Y_speed_range = 1.2f;
    // float damage = 10;
    public int ammo = 10;
    bool delay = false;
    AudioSource audioSource;
    public AudioClip shootSound;
    public AudioClip emptySound;
    GunSync gunSync;

    void Start()
    {
        gun = GetComponent<Gun>();
        audioSource = GetComponent<AudioSource>();
        gunSync = GetComponent<GunSync>();
    }
    
    void Update()
    {
        if(gun.shooting)
            Shoot();
    }

    void Shoot(){
        if(delay) return;
        if( ammo <= 0 ){ 
            print("sem munição!!");
            if(!audioSource.isPlaying)
                audioSource.PlayOneShot(emptySound, PlayerPrefs.GetFloat("volume", 1));
            return; 
        }
        
        ammo -= 1;
        // print("ammo: " + ammo);
        // audioSource.PlayOneShot(shootSound, audioSource.volume);
        gunSync.ShootingSound();
        gunSync.view.RPC("InstantiateBullet", RpcTarget.All, X_speed, Random.Range(-Y_speed_range, +Y_speed_range));
        StartCoroutine("Delay");
    }

    IEnumerator Delay(){
        delay = true;
        yield return new WaitForSeconds(firerate);
        delay = false;
    }
}
