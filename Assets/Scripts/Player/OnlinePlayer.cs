using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class OnlinePlayer : MonoBehaviourPunCallbacks, IPunObservable
{
    public Transform playerHands;
    public float direction;
    bool facingRight = true, isDead = false;
    float speed = 5f;
    float xSpeed = 0f, ySpeed = 0f;
    Vector3 smoothMove;
    Quaternion smoothRotation;

    float jumpForce = 8f;
    float dashForce = 14f;
    float dashTime = 0.15f;
    float dashCoolDownTime = 2.0f;
    bool isDashing = false;
    bool isGrounded = false;
    bool isDashCoolDown = false;

    bool canPickGun = false;
    bool isWallJumping = false;
    bool canAirJump = false;
    bool wasInAir = false;

    bool isWallSliding = false;
    public float wallSlidingSpeed = 1f;
    
    float WallJumpForce_X = 5f;
    float WallJumpForce_Y = 8f;

    
    AudioSource audioSource;
    bool isWalkingSoundOn = false;
    float WalkingSoundSpeed = 1.5f;

    [Header("Footstep Sounds")]
    [SerializeField] AudioClip[] footstepSound = null;

    [Header("Metal Jump Start Sounds")]
    [SerializeField] AudioClip[] metalJumpStartSound = null;

    [Header("Metal Jump Land Sounds")]
    [SerializeField] AudioClip[] metalJumpLandSound = null;

    [Header("Dash Sounds")]
    [SerializeField] AudioClip[] DashSound = null;

    GunPickup gunPickup;
    Rigidbody2D rb;
    SpriteRenderer renderer;
    Animator animator;
    Gun currentGun;
    Sync sync;
    public AnimatorControllers animatorControllers;

    PhotonView view;

    [HideInInspector]
    public bool invincible = false, flicker = false;
    [HideInInspector] public HealthSystem health;
    int maxHealth;
    public Slider healthBar;
    
    void Start()
    {
        health = new HealthSystem(100);
        maxHealth = health.GetHealth();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        renderer = GetComponent<SpriteRenderer>();
        view = GetComponent<PhotonView>();
        sync = GetComponent<Sync>();
        audioSource = GetComponent<AudioSource>();
        StartCoroutine("InvincibleTime");
        StartCoroutine("Flicker");
    }

    IEnumerator Flicker(){
        if(view.IsMine){
            yield return new WaitForSeconds(0.2f);

            sync.view.RPC("SyncFlicker", RpcTarget.All, view.ViewID, true);

            yield return new WaitForSeconds(0.2f);

            sync.view.RPC("SyncFlicker", RpcTarget.All, view.ViewID, false);

            if (flicker) StartCoroutine("Flicker");
        }
    }

    IEnumerator InvincibleTime(){
        if(view.IsMine){
            flicker = invincible = true;

            yield return new WaitForSeconds(2.0f);

            flicker = invincible = false;
        }
    }
    
    void Update()
    {
        if(view.IsMine){
			audioSource.volume =  PlayerPrefs.GetFloat("volume", 1);
            isGrounded = IsGrounded();
            animator.SetFloat("Move", Mathf.Abs(direction));
            if (direction > 0 && !facingRight || direction < 0 && facingRight){
                transform.Rotate(0, 180, 0);
                facingRight = !facingRight;
                if(healthBar.direction == Slider.Direction.LeftToRight)
                    healthBar.direction = Slider.Direction.RightToLeft;
                else
                    healthBar.direction = Slider.Direction.LeftToRight;
            }
            
            if( isGrounded || IsHoldingWall() ) canAirJump = true;
        }
        else{
            SmoothMovement();
            if (rb.velocity.x > 0 && !facingRight || rb.velocity.x < 0 && facingRight){
                facingRight = !facingRight;
                if(healthBar.direction == Slider.Direction.LeftToRight)
                    healthBar.direction = Slider.Direction.RightToLeft;
                else
                    healthBar.direction = Slider.Direction.LeftToRight;
            }
        }
    }

    void SmoothMovement(){
        //transform.position = Vector3.Lerp(transform.position, smoothMove, 5.0f*Time.deltaTime);
        if(Vector3.Distance(transform.position, smoothMove) > 2)
            transform.position = smoothMove;
        else
            transform.position = Vector3.MoveTowards(transform.position, smoothMove, 1.0f*Time.deltaTime);
        transform.rotation = smoothRotation;
    }

    void FixedUpdate(){
        if(view.IsMine){

            if( !isWallJumping && !isDashing ) xSpeed = speed*direction;
            else xSpeed = rb.velocity.x;
            
            if( !IsHoldingWall() ){ 
                ySpeed = rb.velocity.y;
                isWallSliding = false;
            }else{
                isWallSliding = true;
                ySpeed = Mathf.Clamp(rb.velocity.y, -wallSlidingSpeed, float.MaxValue);
            }
            
            rb.velocity = new Vector2(xSpeed, ySpeed);

            WalkingSound();
            
            if( wasInAir && isGrounded ){
                LandJumpingSound();
                wasInAir = false;
            }else if( !isGrounded ){
                wasInAir = true;
            }else{
                wasInAir = false;
            }
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info){
        if(stream.IsWriting){
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else if(stream.IsReading){
            smoothMove = (Vector3)stream.ReceiveNext();
            smoothRotation = (Quaternion)stream.ReceiveNext();
        }
    }

    void WalkingSound(){
        if(view.IsMine){
            if( xSpeed == 0 ) return;
            if( !isGrounded ) return;
            if( isWalkingSoundOn ) return;

            isWalkingSoundOn = true;
            int index = Random.Range(0, footstepSound.Length);
            view.RPC("SyncWalkingSound", RpcTarget.All, view.ViewID, index);
            StartCoroutine("WalkingDelay", footstepSound[index].length/WalkingSoundSpeed);
        }
    }

    [PunRPC]
    public void SyncWalkingSound(int playerView, int SoundID){
        // if( !view.IsMine ) return;
        if( view.ViewID != playerView ) return;

        audioSource.pitch = WalkingSoundSpeed;
        audioSource.PlayOneShot(footstepSound[SoundID], PlayerPrefs.GetFloat("volume", 1));
    }

    IEnumerator WalkingDelay(float time){
        if(view.IsMine){
            isWalkingSoundOn = true;
            yield return new WaitForSeconds(time);
            isWalkingSoundOn = false;
        }
    }

    void StartJumpingSound(){
        if(view.IsMine){
            int index = Random.Range(0, metalJumpStartSound.Length);
            view.RPC("SyncStartJumpingSound", RpcTarget.All, view.ViewID, index);
        }
    }

    [PunRPC]
    public void SyncStartJumpingSound(int playerView, int SoundID){
        // if( !view.IsMine ) return;
        if( view.ViewID != playerView ) return;

        audioSource.pitch = 1.3f;
        audioSource.PlayOneShot(metalJumpStartSound[SoundID], PlayerPrefs.GetFloat("volume", 1));
    }

    void LandJumpingSound(){
        if(view.IsMine){
            int index = Random.Range(0, metalJumpLandSound.Length);
            view.RPC("SyncLandJumpingSound", RpcTarget.All, view.ViewID, index);
        }
    }

    [PunRPC]
    public void SyncLandJumpingSound(int playerView, int SoundID){
        // if( !view.IsMine ) return;
        if( view.ViewID != playerView ) return;

        audioSource.pitch = 3.0f;
        audioSource.PlayOneShot(metalJumpLandSound[SoundID], PlayerPrefs.GetFloat("volume", 1));
    }

    void DashingSound(){
        if(view.IsMine){
            int index = Random.Range(0, DashSound.Length);
            view.RPC("SyncDashSound", RpcTarget.All, view.ViewID, index);
        }
    }

    [PunRPC]
    public void SyncDashSound(int playerView, int SoundID){
        // if( !view.IsMine ) return;
        if( view.ViewID != playerView ) return;

        audioSource.pitch = 1.7f;
        audioSource.PlayOneShot(DashSound[SoundID], PlayerPrefs.GetFloat("volume", 1));
    }

    bool IsGrounded(){
        if(view.IsMine){
            int mask = 1 << LayerMask.NameToLayer("Ground");

            // colis�o com o ch�o
            RaycastHit2D hit_floor = Physics2D.Raycast(transform.position, -Vector2.up, 0.55f, mask);
            // Debug.DrawRay(transform.position, new Vector2(0, -0.55f), Color.red);

            if( hit_floor.collider ){
                //animator
                return true;
            }
            //animator
            return false;
        }
        return false;
    }

    bool IsHoldingWall(){
        if(view.IsMine){
            int mask = 1 << LayerMask.NameToLayer("Ground");
            // colis�o com parede a esquerda
            RaycastHit2D hit_left = Physics2D.Raycast(transform.position, Vector2.left, 0.309f, mask);
            // Debug.DrawRay(transform.position, new Vector2( -0.309f, 0), Color.red);
            // colis�o com parede a direita
            RaycastHit2D hit_right = Physics2D.Raycast(transform.position, Vector2.right, 0.309f, mask);
            if( hit_left.collider || hit_right.collider){
                //animator
                return true;
            }
            //animator
            return false;
        }
        return false;
    }

    IEnumerator WallJumping(){
        if(view.IsMine){
            isWallJumping = true;
            float side = facingRight ? 1 : -1;
            rb.velocity = new Vector2( rb.velocity.x, 0);
            rb.AddForce( new Vector2( -WallJumpForce_X*side, WallJumpForce_Y), ForceMode2D.Impulse );
            yield return new WaitForSeconds(0.2f);
            isWallJumping = false;
        }
    }

    public void Jump(){
        if(view.IsMine){
            if( isGrounded ){
                rb.AddRelativeForce( transform.up*jumpForce, ForceMode2D.Impulse );
                canAirJump = true;
                StartJumpingSound();
            }else if ( isWallSliding ){
                StartCoroutine("WallJumping");
                canAirJump = true;
                StartJumpingSound();
            }else if ( canAirJump ){
                rb.velocity = new Vector2( rb.velocity.x, 0);
                rb.AddRelativeForce( transform.up*jumpForce, ForceMode2D.Impulse );
                canAirJump = false;
                StartJumpingSound();
            }
            return;
        }
    }

    IEnumerator Dashing(){
        if(view.IsMine){
            isDashing = true;
            int side = facingRight ? 1 : -1;
            rb.AddForce( new Vector2( dashForce*side, 0), ForceMode2D.Impulse );
            DashingSound();
            yield return new WaitForSeconds(dashTime);
            isDashing = false;
            StartCoroutine("DashCoolDown");
        }
    }

    IEnumerator DashCoolDown(){
        if(view.IsMine){
            isDashCoolDown = true;
            yield return new WaitForSeconds(dashCoolDownTime);
            isDashCoolDown = false;
        }
    }

    public void Dash(){
        if(view.IsMine){
            if ( !isDashCoolDown ) StartCoroutine("Dashing");
            return;
        }
    }

    void DestroyGun(){
        if(view.IsMine){
            Destroy(playerHands.GetChild(0).gameObject);
            sync.view.RPC("SyncDestroyGuns", RpcTarget.All, playerHands.GetChild(0).gameObject.GetPhotonView().ViewID);
            //PhotonNetwork.Destroy(playerHands.GetChild(0).gameObject);
            currentGun = null;
        }
    }

    public void PickUpGun(){
        if(view.IsMine){
            if(!canPickGun)
                return;
            if(currentGun != null) DestroyGun();

            GameObject weapon = PhotonNetwork.Instantiate(gunPickup.gun.name, playerHands.position, playerHands.rotation);
            weapon.GetComponent<GunInfo>().owner = PhotonNetwork.LocalPlayer;
            weapon.transform.parent = playerHands.transform;
            sync.view.RPC("SyncGuns", RpcTarget.All, weapon.GetPhotonView().ViewID, view.ViewID);
            currentGun = weapon.GetComponent<Gun>();
            if(gunPickup.transform.parent != null){
                WeaponSpawner.weaponSpawner.WeaponSpawnCheck(gunPickup.transform.parent);
            }

            sync.view.RPC("SyncDestroyGuns", RpcTarget.All, gunPickup.gameObject.GetPhotonView().ViewID);
            Destroy(gunPickup.gameObject);
            //PhotonNetwork.Destroy(gunPickup.gameObject);
            gunPickup = null;
        }
    }

    void OnCollisionEnter2D(Collision2D col){
        if(view.IsMine){
            if(col.gameObject.CompareTag("moving platform")){
                this.transform.parent = col.transform;
            }
        }
    }

    void OnCollisionExit2D(Collision2D col){
        if(view.IsMine){
            if(col.gameObject.CompareTag("moving platform")){
                this.transform.parent = null;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D col){
        if(view.IsMine){
            if(col.gameObject.CompareTag("gun")){
                canPickGun = true;
                //currentGun = col.gameObject;
                gunPickup = col.GetComponent<GunPickup>();
            }
            if(col.gameObject.CompareTag("bullet") && !invincible){
                BulletInfo bulletInfo = col.gameObject.GetComponent<BulletInfo>();
                health.Damage(bulletInfo.damage);
                float currentHealth = health.GetHealth() > 0 ? (float)health.GetHealth()/(float)maxHealth : 0;
                //healthBar.value = currentHealth;
                sync.view.RPC("SyncHealthBar", RpcTarget.All, view.ViewID, currentHealth);
                if(health.GetHealth() <= 0 && !isDead){
                    isDead = true;
                    view.RPC("GetPoint", bulletInfo.owner);
                    PlayerSpawner.instance.StartCoroutine("PlayerSpawnTimer");
                    sync.view.RPC("SyncDeathFX", RpcTarget.All, view.ViewID);
                    PhotonNetwork.Destroy(gameObject);
                }
            }
            if(col.gameObject.CompareTag("death box")){
                isDead = true;
                PlayerSpawner.instance.StartCoroutine("PlayerSpawnTimer");
                sync.view.RPC("SyncDeathFX", RpcTarget.All, view.ViewID);
                PhotonNetwork.Destroy(gameObject);
            }
        }
    }

    void OnTriggerStay2D(Collider2D col){
        if(view.IsMine){
            if(col.gameObject.CompareTag("gun"))
                canPickGun = true;
        }
    }

    void OnTriggerExit2D(Collider2D col){
        if(view.IsMine){
            if(col.gameObject.CompareTag("gun"))
                canPickGun = false;
        }
    }

    public void Shoot(){
        if(view.IsMine){
            if(currentGun == null) return;
            // Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            currentGun.shooting = true;
        }
    }

    public void StopShooting(){
        if(view.IsMine){
            if(currentGun == null) return;
            currentGun.shooting = false;
        }
    }

    [PunRPC]
    public void GetPoint(){
        GameManager.instance.ScorePoint();
    }
}
