using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class Sync : MonoBehaviourPunCallbacks
{
    [HideInInspector] public PhotonView view;
    public Guns guns;
    OnlinePlayer player;
    public GameObject deathfx;

    void Start()
    {
        view = GetComponent<PhotonView>();
        player = GetComponent<OnlinePlayer>();
    }


    [PunRPC]
    public void SyncGuns(int gunView, int playerView, PhotonMessageInfo info){
        PhotonView.Find(gunView).transform.parent = PhotonView.Find(playerView).GetComponent<OnlinePlayer>().playerHands.transform;
    }

    [PunRPC]
    public void SyncDestroyGuns(int gunView, PhotonMessageInfo info){
        PhotonNetwork.Destroy(PhotonView.Find(gunView).gameObject);
    }

    [PunRPC]
    public void SyncFlicker(int playerView, bool fade){
        SpriteRenderer renderer = PhotonView.Find(playerView).GetComponent<SpriteRenderer>();
        renderer.color = fade ? new Color(1, 1, 1, 0.5f) : new Color(1, 1, 1, 1);
    }

    [PunRPC]
    public void SyncDeathFX(int playerView){
        Transform playerTransform = PhotonView.Find(playerView).transform;
        GameObject fx = Instantiate(deathfx, playerTransform.position, playerTransform.rotation);
        Destroy(fx, 1.0f);
    }

    [PunRPC]
    public void SyncHealthBar(int playerView, float health){
        GameObject playerObject = PhotonView.Find(playerView).gameObject;
        playerObject.GetComponent<OnlinePlayer>().healthBar.value = health;
    }
}