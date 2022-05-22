using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class WeaponSpawnPoint : MonoBehaviour
{
    PhotonView view;

    void Start(){
        view = GetComponent<PhotonView>();
    }

    public void StartSpawnWeapon(GameObject gun, float waitTime){
        StartCoroutine(SpawnWeaponTimer(gun, waitTime));
    }

    public IEnumerator SpawnWeaponTimer(GameObject gun, float waitTime){
        yield return new WaitForSeconds(waitTime);
        //GameObject weapon = PhotonNetwork.InstantiateRoomObject(gun.name, transform.position, transform.rotation);
        view.RPC("SpawnWeapon", RpcTarget.MasterClient, gun.name);
    }

    [PunRPC]
    public void SpawnWeapon(string gunName){
        GameObject weapon = PhotonNetwork.InstantiateRoomObject(gunName, transform.position, transform.rotation);
        view.RPC("PositionWeapon", RpcTarget.All, weapon.GetPhotonView().ViewID);
    }

    [PunRPC]
    void PositionWeapon(int gunView){
        Transform weapon = PhotonView.Find(gunView).transform;
        weapon.position = transform.position;
        weapon.SetParent(transform);
    }
}
