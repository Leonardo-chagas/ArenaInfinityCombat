using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class WeaponSpawner : MonoBehaviourPunCallbacks
{
    public Guns guns;
    public Transform[] spawnPoints;
    public static WeaponSpawner weaponSpawner;
    public float spawnTime;
    int[] weaponIds;
    PhotonView view;
    
    void Start()
    {
        weaponIds = new int[spawnPoints.Length];
        view = GetComponent<PhotonView>();
        weaponSpawner = this;
        int cont = 0;
        if(PhotonNetwork.IsMasterClient){
            foreach(Transform point in spawnPoints){
                int randomPos = Random.Range(0, guns.Weapons.Length);
                GameObject weapon = PhotonNetwork.InstantiateRoomObject(guns.Weapons[randomPos].name, point.position, point.rotation);
                weapon.transform.SetParent(point);
                weaponIds[cont] = weapon.GetPhotonView().ViewID;
                cont += 1;
            }
            view.RPC("InitialWeaponSpawns", RpcTarget.All, weaponIds);
        }
    }

    [PunRPC]
    public void InitialWeaponSpawns(int[] ids){
        int cont = 0;
        foreach(int id in ids){
            Transform weapon = PhotonView.Find(id).transform;
            weapon.position = spawnPoints[cont].position;
            weapon.parent = spawnPoints[cont];
            cont += 1;
        }
    }

    public void WeaponSpawnCheck(Transform point){
        /* foreach(Transform point in spawnPoints){
            Transform[] children = point.gameObject.GetComponentsInChildren<Transform>();
            if(children.Length == 0){
                int randomPos = Random.Range(0, guns.Weapons.Length);
                WeaponSpawnPoint weaponSpawnPoint = point.gameObject.GetComponent<WeaponSpawnPoint>();
                weaponSpawnPoint.StartSpawnWeapon(guns.Weapons[randomPos], spawnTime);
            }
        } */
        int randomPos = Random.Range(0, guns.Weapons.Length);
        point.gameObject.GetComponent<WeaponSpawnPoint>().StartSpawnWeapon(guns.Weapons[randomPos], spawnTime);
    }
}
