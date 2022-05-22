using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Pistol : MonoBehaviour
{
    Gun gun;
    public GameObject bullet;
    public Transform firepoint;
    public float firerate = 0.6f;
    public float X_speed = 10f;
    // float damage = 20;
    public int ammo = 3;
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
        if( delay ) return;
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
        gunSync.view.RPC("InstantiateBullet", RpcTarget.All, X_speed, 0f);
        StartCoroutine("ShootDelay");
    }

    IEnumerator ShootDelay(){
        delay = true;
        yield return new WaitForSeconds(firerate);
        delay = false;
    }
}
