using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerSpawner : MonoBehaviour
{
    public GameObject player;
    PhotonView view;

    public Transform[] spawnPoints;
    public int cont = 0;
    public AnimatorControllers animatorControllers;
    int spriteIndex;
    public static PlayerSpawner instance;

    private void Awake(){
        instance = this;
        view = GetComponent<PhotonView>();
        if(PhotonNetwork.IsMasterClient){
            foreach(Player pl in PhotonNetwork.PlayerList){
                view.RPC("SpawnPlayer",pl, cont, cont);
                cont += 1;
            }
        }
    }

    [PunRPC]
    void SpawnPlayer(int index, int spawnIndex){
        GameObject playerCharacter = PhotonNetwork.Instantiate(player.name, spawnPoints[spawnIndex].position, Quaternion.identity);
        view.RPC("PlayerSprite", RpcTarget.All, index, playerCharacter.GetPhotonView().ViewID);
        spriteIndex = index;
    }

    [PunRPC]
    void PlayerSprite(int index, int playerView){
        GameObject playerCharacter = PhotonView.Find(playerView).gameObject;
        Animator playerAnimator = playerCharacter.GetComponent<Animator>();
        playerAnimator.runtimeAnimatorController = animatorControllers.animatorControllers[index] as RuntimeAnimatorController;
    }

    public IEnumerator PlayerSpawnTimer(){
        yield return new WaitForSeconds(5.0f);
        GameObject playerCharacter = PhotonNetwork.Instantiate(player.name, spawnPoints[Random.Range(0, 3)].position, Quaternion.identity);
        view.RPC("PlayerSprite", RpcTarget.All, spriteIndex, playerCharacter.GetPhotonView().ViewID);
    }
}
