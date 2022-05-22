using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class GaussRifle : MonoBehaviour
{
    Gun gun;
    public int ammo = 2;
    float charge = 0;
    bool delay = false;
    Animator animator;
    public GameObject chargeEffect;
    public EdgeCollider2D collider;
    public Transform firePoint;
    public LineRenderer lineRenderer;
    public float maxCharge = 100;
    AudioSource audioSource;
    public AudioClip chargingSound;
    public AudioClip shootSound;
    public AudioClip emptySound;
    PhotonView view;
    bool charging = false;
    
    void Start()
    {
        gun = GetComponent<Gun>();
        chargeEffect.SetActive(false);
        collider.gameObject.SetActive(false);
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        collider.gameObject.GetComponent<BulletInfo>().owner = GetComponent<GunInfo>().owner;
        view = GetComponent<PhotonView>();
    }

    
    void Update()
    {
        if( delay ) return;
        audioSource.volume = PlayerPrefs.GetFloat("volume", 1);

        if(gun.shooting){
            if( ammo <= 0 ){
                print("sem munição!!");
                charge = 0;
                chargeEffect.SetActive(false);
                audioSource.Stop();
                if(!audioSource.isPlaying)
                    audioSource.PlayOneShot(emptySound, PlayerPrefs.GetFloat("volume", 1));
                return;
            }
            charge += 1*Time.deltaTime;
            if(!charging)
                view.RPC("SyncCharge", RpcTarget.All, true);
            charging = true;
            }
        else{
            charge = 0;
            if(charging)
                view.RPC("SyncCharge", RpcTarget.All, false);
            charging = false;
        }
        
        if(charge >= maxCharge){
            //Shoot();
            view.RPC("Shoot", RpcTarget.All);
        }
    }

    [PunRPC]
    void SyncCharge(bool isCharging){
        if(isCharging){
            chargeEffect.SetActive(true);
            if (!audioSource.isPlaying){
                audioSource.Play();
            }
        }
        else{
            chargeEffect.SetActive(false);
            audioSource.Stop();
        }
    }

    [PunRPC]
    void Shoot(){

        ammo -= 1;
        print("ammo: " + ammo);
        gun.shooting = false;
        chargeEffect.SetActive(false);
        int mask = 1 << LayerMask.NameToLayer("Ground");

        RaycastHit2D hit = Physics2D.Raycast(firePoint.position, transform.right, 50, mask);
        Vector3 endPoint;
        if(hit.collider){
            lineRenderer.SetPosition(0, (Vector2)transform.InverseTransformPoint(firePoint.position));
            endPoint = transform.InverseTransformPoint(hit.point);
            print(endPoint);
            
        }else{
            lineRenderer.SetPosition(0, (Vector2)transform.InverseTransformPoint(firePoint.position));
            int side = firePoint.rotation[1] == -1 ? -1 : 1;
            endPoint = transform.InverseTransformPoint(firePoint.position) + transform.right*20*side ;
        }
        lineRenderer.SetPosition(1, (Vector2)endPoint);

        List<Vector2> points = new List<Vector2>();
        points.Add(new Vector2(0.2f, 0.11f));
        points.Add(new Vector2(endPoint.x, 0.11f));
        points.Add(new Vector2(endPoint.x, -0.16f));
        points.Add(new Vector2(0.2f, -0.16f));
        collider.gameObject.SetActive(true);
        collider.SetPoints(points);
        //collider.transform.localPosition = new Vector2(middlePoint/3, collider.transform.localPosition.y);

        audioSource.PlayOneShot(shootSound, audioSource.volume);
        StartCoroutine("ShotEnd");
    }

    IEnumerator ShotEnd(){
        delay = true;
        yield return new WaitForSeconds(0.3f);
        lineRenderer.SetPosition(0, Vector2.zero);
        lineRenderer.SetPosition(1, Vector2.zero);
        collider.gameObject.SetActive(false);
        StartCoroutine("Delay");
    }

    IEnumerator Delay(){
        animator.SetBool("reload", true);
        yield return new WaitForSeconds(1.0f);
        animator.SetBool("reload", false);
        delay = false;
    }
}
