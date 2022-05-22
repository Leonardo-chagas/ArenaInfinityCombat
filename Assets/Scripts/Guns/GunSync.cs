using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class GunSync : MonoBehaviour
{
    public PhotonView view;
    public GameObject bullet;
    public Transform firepoint;
    public AudioClip shootSound;
    AudioSource audioSource;
 
    void Start()
    {
        view = GetComponent<PhotonView>();
        audioSource = GetComponent<AudioSource>();
    }

    [PunRPC]
    public void InstantiateBullet(float X_speed, float Y_speed){
        GameObject b = Instantiate(bullet, firepoint.position, firepoint.rotation);
        Rigidbody2D rb = b.GetComponent<Rigidbody2D>();
        rb.velocity = transform.right*X_speed + transform.up*Y_speed;
        b.GetComponent<BulletInfo>().owner = GetComponent<GunInfo>().owner;
    }

    public void ShootingSound(){
        if(view.IsMine){
            // int index = Random.Range(0, metalJumpStartSound.Length);
            view.RPC("SyncShootingSound", RpcTarget.All, view.ViewID);
        }
    }

    [PunRPC]
    public void SyncShootingSound(int playerView){
        // if( !view.IsMine ) return;
        if( view.ViewID != playerView ) return;

        // audioSource.pitch = 1.3f;
        // print("volume: "+PlayerPrefs.GetFloat("volume", 1));
        audioSource.PlayOneShot(shootSound, PlayerPrefs.GetFloat("volume", 1));
    }
}
