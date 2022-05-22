using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player2 : MonoBehaviour
{
    public Transform playerHands;
    public float direction;
    bool facingRight = true;
    float speed = 5f;
    float xSpeed = 0f, ySpeed = 0f;

    float jumpForce = 8f;
    float dashForce = 14f;
    float dashTime = 0.15f;
    float dashCoolDownTime = 2.0f;
    bool isDashing = false;
    bool isDashCoolDown = false;

    bool canPickGun = false;
    bool isWallJumping = false;
    bool canAirJump = false;

    bool isWallSliding = false;
    public float wallSlidingSpeed = 1f;
    
    float WallJumpForce_X = 5f;
    float WallJumpForce_Y = 8f;

    GunPickup gunPickup;
    Rigidbody2D rb;
    SpriteRenderer renderer;
    Animator animator;
    Gun currentGun;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        renderer = GetComponent<SpriteRenderer>();
    }
    
    void Update()
    {
        animator.SetFloat("Move", Mathf.Abs(direction));
        if (direction > 0 && !facingRight || direction < 0 && facingRight){
            transform.Rotate(0, 180, 0);
            facingRight = !facingRight;
        }

        if( IsGrounded() || IsHoldingWall() ) canAirJump = true;
    }

    void FixedUpdate(){
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
    }

    bool IsGrounded(){
        int mask = 1 << LayerMask.NameToLayer("Ground");

        // colisão com o chão
        RaycastHit2D hit_floor = Physics2D.Raycast(transform.position, -Vector2.up, 0.55f, mask);
        // Debug.DrawRay(transform.position, new Vector2(0, -0.55f), Color.red);

        if( hit_floor.collider ){
            //animator
            return true;
        }
        //animator
        return false;
    }

    bool IsHoldingWall(){
        int mask = 1 << LayerMask.NameToLayer("Ground");
        // colisão com parede a esquerda
        RaycastHit2D hit_left = Physics2D.Raycast(transform.position, Vector2.left, 0.309f, mask);
        // Debug.DrawRay(transform.position, new Vector2( -0.309f, 0), Color.red);
        // colisão com parede a direita
        RaycastHit2D hit_right = Physics2D.Raycast(transform.position, Vector2.right, 0.309f, mask);
        if( hit_left.collider || hit_right.collider){
            //animator
            return true;
        }
        //animator
        return false;
    }

    IEnumerator WallJumping(){
        isWallJumping = true;
        float side = facingRight ? 1 : -1;
        rb.velocity = new Vector2( rb.velocity.x, 0);
        rb.AddForce( new Vector2( -WallJumpForce_X*side, WallJumpForce_Y), ForceMode2D.Impulse );
        yield return new WaitForSeconds(0.2f);
        isWallJumping = false;
    }

    public void Jump(){
        if( IsGrounded() ){
            rb.AddRelativeForce( transform.up*jumpForce, ForceMode2D.Impulse );
            canAirJump = true;
        }else if ( isWallSliding ){
            StartCoroutine("WallJumping");
            canAirJump = true;
        }else if ( canAirJump ){
            rb.velocity = new Vector2( rb.velocity.x, 0);
            rb.AddRelativeForce( transform.up*jumpForce, ForceMode2D.Impulse );
            canAirJump = false;
        }
        
        return;
    }

    IEnumerator Dashing(){
        isDashing = true;
        int side = facingRight ? 1 : -1;
        rb.AddForce( new Vector2( dashForce*side, 0), ForceMode2D.Impulse );
        yield return new WaitForSeconds(dashTime);
        isDashing = false;
        StartCoroutine("DashCoolDown");
    }

    IEnumerator DashCoolDown(){
        isDashCoolDown = true;
        yield return new WaitForSeconds(dashCoolDownTime);
        isDashCoolDown = false;
    }

    public void Dash(){
        if ( !isDashCoolDown ) StartCoroutine("Dashing");
        return;
    }

    void DestroyGun(){
        Destroy(playerHands.GetChild(0).gameObject);
        currentGun = null;
    }

    public void PickUpGun(){
        if(!canPickGun)
            return;
        
        if(currentGun != null){
            DestroyGun();
        }
        GameObject weapon = Instantiate(gunPickup.gun, playerHands.position, playerHands.rotation);
        weapon.transform.parent = playerHands.transform;
        currentGun = weapon.GetComponent<Gun>();
        if(gunPickup.transform.parent != null)
            WeaponSpawner.weaponSpawner.WeaponSpawnCheck(gunPickup.transform.parent);

        Destroy(gunPickup.gameObject);
        gunPickup = null;
    }

    void OnCollisionEnter2D(Collision2D col){
        if(col.gameObject.CompareTag("moving platform")){
            this.transform.parent = col.transform;
        }
    }

    void OnCollisionExit2D(Collision2D col){
        if(col.gameObject.CompareTag("moving platform")){
            this.transform.parent = null;
        }
    }

    void OnTriggerEnter2D(Collider2D col){
        if(col.gameObject.CompareTag("gun")){
            canPickGun = true;
            //currentGun = col.gameObject;
            gunPickup = col.GetComponent<GunPickup>();
        }
    }

    void OnTriggerStay2D(Collider2D col){
        if(col.gameObject.CompareTag("gun"))
            canPickGun = true;
    }

    void OnTriggerExit2D(Collider2D col){
        if(col.gameObject.CompareTag("gun"))
            canPickGun = false;
    }

    public void Shoot(){
        // Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        currentGun.shooting = true;
    }

    public void StopShooting(){
        currentGun.shooting = false;
    }
}