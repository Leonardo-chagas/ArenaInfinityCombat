using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Shotgun : MonoBehaviour
{
    Gun gun;
    public GameObject bullet;
    public Transform firepoint;
    public float firerate = 2f;
    public float X_speed = 10f;
    public float X_variationRate = 0.3f;
    public float Y_speed_range = 1.2f;
    // float damage = 10;
    public int ammo = 2;
    bool delay = false;
    AudioSource audioSource;
    public AudioClip shootSound;
    public AudioClip emptySound;
    GunSync gunSync;

    PhotonView view;
    
    void Start()
    {
        gun = GetComponent<Gun>();
        audioSource = GetComponent<AudioSource>();
        gunSync = GetComponent<GunSync>();
        view = GetComponent<PhotonView>();
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
        for(int i=0; i<10; i++){
            gunSync.view.RPC("InstantiateBullet", RpcTarget.All, 
                X_speed*Random.Range(1-X_variationRate, 1+X_variationRate), 
                Random.Range(-Y_speed_range, +Y_speed_range)
            );
        }
        gunSync.ShootingSound();
        // audioSource.PlayOneShot(shootSound);
        StartCoroutine("Delay");
    }

    IEnumerator Delay(){
        delay = true;
        yield return new WaitForSeconds(firerate);
        delay = false;
    }

    // void ShootingSound(AudioClip shootSound){
    //     if(view.IsMine){
    //         // int index = Random.Range(0, metalJumpStartSound.Length);
    //         view.RPC("SyncShootingSound", RpcTarget.All, view.ViewID, shootSound);
    //     }
    // }

    // [PunRPC]
    // public void SyncShootingSound(int playerView, AudioClip shootSound, int SoundID){
    //     // if( !view.IsMine ) return;
    //     if( view.ViewID != playerView ) return;

    //     // audioSource.pitch = 1.3f;
    //     audioSource.PlayOneShot(shootSound, audioSource.volume);
    // }
}
