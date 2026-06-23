using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class PlayerMainBahvior : MonoBehaviour
{
    [SerializeField]Animator anim;
    public static bool playeCanInput = true;
    public enum SubWeapons
    {
        None,
        InfinityBullet
    }
    public enum Facing
    {
        L = -1,
        R = 1
    }
    [SerializeField]GameObject deathEffect;
    [SerializeField] Material trailMaterial;
    Rigidbody2D rb2d;
    [SerializeField] LayerMask GroundLayer;
    [SerializeField]PlayerInput playerInput;
    InputAction moveAction;
    InputAction jumpAction;
    InputAction attackAction;
    InputAction reloadAction;
    InputAction dashAction;
    InputAction subWeaponsAction;
    PlayerUI playerUI;
    public PlayerUI PlayerUI {get => playerUI; }
    [SerializeField] private int hp;

    public int HP
    {
        get => hp;
        set => hp = value;
    }

    [SerializeField] private int damageTime;
    [SerializeField] private int invicibleTime;
    [SerializeField] BoxCollider2D triggerCollider;
    [SerializeField] private Material damageMaterial;
    [SerializeField] private Material invicibleMaterial;
    [SerializeField] private GameObject damageEffect;
    [SerializeField] private Light shotLightEffect;
    [Header("Movement")]
    public float speed = 10.0f;
    public float jumpForce = 10.0f;
    public int jumpCountMax = 20;
    public int jumpCount = 0;
    public bool gounding;
    public bool canDoubleJump;
    public GameObject doubleJumpAttack;
    public Facing lastFacing = Facing.R;
    bool jumpPressing = false;
    public int dashCountMax = 20;
    public int dashCount = -1;
    public float dashForce = 20.0f;
    public GameObject brustEffect;

    [Header("Weapons")]
    public int bulletAmount;
    public int bulletAmountMax = 6;
    public Transform bulletSpawn;
    public GameObject bulletPrefab;
    public SubWeapons subWeapons;
    public int[] subWeaponEN;
    public int shootingCount = 0;
    public int  reloadCountMax = 60;
    public bool reloading = false;
    public bool attackCharging = false;
    public GameObject chargeEffect;
    public ParticleSystem shotEffect;
    public ParticleSystem chargeCompleteEffect;
    public int chargeAmout = 0;
    public int quickFireCount = 0;
    public ParticleSystem quickFireEffect;
    public ParticleSystem quickFireBrustEffect;

    #region Coroutine

    Coroutine deathCoroutine = null;

    IEnumerator IEnumerator_Death()
    {
        rb2d.linearVelocity = Vector2.zero;
        StartCoroutine(PlayerGamepadHaptics.IEnumerator_GamepadHaptics(0.15f, 1f, 0.00f));
        Time.timeScale = 0.1f;
        yield return new WaitForFixedUpdate();
        Destroy(Instantiate(deathEffect,transform.position,Quaternion.identity),3f);
        Time.timeScale = 1f;
        var tmp_InGameSceneManager = FindFirstObjectByType<InGameSceneManager>();
        StartCoroutine(tmp_InGameSceneManager.loadSceneRef(tmp_InGameSceneManager.IngameScene,90));
        transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.OutElastic);
    }

    Coroutine damageCoroutine = null;
    IEnumerator IEnumerator_Damage()
    {
        anim.SetTrigger("Damage");

        if (jumpAndFallCoroutine != null)
        {
            StopCoroutine(jumpAndFallCoroutine);
        }
        StartCoroutine(PlayerGamepadHaptics.IEnumerator_GamepadHaptics(0.15f, 0.0f, 0.01f));
        Destroy(Instantiate(damageEffect, transform.position, Quaternion.identity),3f);
        var meshrender = GetComponentsInChildren<MeshRenderer>();
        Material[] tmp_mats  =  new Material[meshrender.Length];
        for (int j = 0; j < meshrender.Length; j++)
        {
            tmp_mats[j] = meshrender[j].material;
        }
        for (int i = 0; i < damageTime; i++)
        {
            rb2d.linearVelocity = new Vector2( -(float)lastFacing* speed* (((damageTime-i)*3 / damageTime)+1)*0.4f  ,0 )  * Time.fixedDeltaTime;

            if (i % 4 != 0)
            {
                for (int j = 0; j < meshrender.Length; j++)
                {
                    meshrender[j].material =  damageMaterial;
                }
            }
            else
            {
                for (int j = 0; j < meshrender.Length;j++)
                {
                    meshrender[j].material =  tmp_mats[j];
                }
            }

            yield return new WaitForFixedUpdate();
        }
        for (int j = 0; j < meshrender.Length;j++)
        {
            meshrender[j].material =  tmp_mats[j];
        }
        damageCoroutine = null;
        jumpAndFallCoroutine = StartCoroutine(IEnumerator_JumpAndFall(true));
        invicibleCoroutine = StartCoroutine(IEnumerator_Invicible());
    }

    Coroutine invicibleCoroutine = null;
    IEnumerator IEnumerator_Invicible()
    {
        var meshrender = GetComponentsInChildren<MeshRenderer>();
        Material[] tmp_mats  =  new Material[meshrender.Length];
        for (int j = 0; j < meshrender.Length; j++)
        {
            tmp_mats[j] = meshrender[j].material;
        }
        for (int i = invicibleTime; i > 0 ; i--)
        {

            if (i % 4 != 0)
            {
                for (int j = 0; j < meshrender.Length; j++)
                {
                    meshrender[j].material =  invicibleMaterial;
                }
            }
            else
            {
                for (int j = 0; j < meshrender.Length;j++)
                {
                    meshrender[j].material =  tmp_mats[j];
                }
            }
            yield return new WaitForFixedUpdate();
        }
        for (int j = 0; j < meshrender.Length;j++)
        {
            meshrender[j].material =  tmp_mats[j];
        }
        invicibleCoroutine = null;
    }

    Coroutine reloadingCoroutine = null;
    IEnumerator IEnumerator_Reloading()
    {
        if (jumpAndFallCoroutine != null)
        {
            StopCoroutine(jumpAndFallCoroutine);
        }
        playerUI.ReloadAmmo();
        reloading = true;
        bulletAmount = 0;
        for (int i = reloadCountMax; i > 0; i--)
        {
            rb2d.linearVelocity = Vector2.zero;

            yield return new WaitForFixedUpdate();
            if (i % (reloadCountMax/bulletAmountMax)   == 0)
            {
                bulletAmount++;
                playerUI.BulletIncreaseAndDecrease(1);
                StartCoroutine(PlayerGamepadHaptics.IEnumerator_GamepadHaptics(0.1f, 0.0f, 0.01f));
            }
        }
        for (int i = 0; i < 5; i++)
        {
            rb2d.linearVelocity = Vector2.zero;

            yield return new WaitForFixedUpdate();
        }
        playerUI.RotateReel();
        StartCoroutine(PlayerGamepadHaptics.IEnumerator_GamepadHaptics(0.1f, 0.01f, 0));
        for (int i = 0; i < 30; i++)
        {
            rb2d.linearVelocity = Vector2.zero;

            yield return new WaitForFixedUpdate();
        }
        jumpAndFallCoroutine = StartCoroutine(IEnumerator_JumpAndFall(true));
        reloadingCoroutine = null;
        reloading = false;
    }

    Coroutine  jumpAndFallCoroutine = null;
    IEnumerator IEnumerator_JumpAndFall( bool _falling　)
    {

        for ( jumpCount = _falling ? -1:jumpCountMax ; !(jumpCount < 1 && gounding) ;)
        {

            if (jumpCount < -jumpCountMax - 5)
            {
                jumpCount = -jumpCountMax - 5;
            }

            if (jumpCount > 0  && ( jumpCount < jumpCountMax -2　&& !jumpPressing && canDoubleJump) )
            {
                jumpCount = 0;
            }

            if (jumpCount == 0)
            {
                rb2d.linearVelocityY = 0;
                for (int i = 0; i < 3; i++)
                {
                    yield return new WaitForFixedUpdate();
                }
            }

            if (dashingCoroutine == null)
            {

                rb2d.linearVelocityY =  jumpForce * Time.fixedDeltaTime * jumpCount ;
                jumpCount--;
            }
            yield return new WaitForFixedUpdate();
        }
        jumpAndFallCoroutine = null;
    }

    Coroutine dashingCoroutine = null;
    IEnumerator IEnumerator_Dashing()
    {
        anim.SetTrigger("Dash");

        rb2d.linearVelocity = Vector2.zero;

        StartCoroutine(PlayerGamepadHaptics.IEnumerator_GamepadHaptics(0.1f, 0.0f, 0.01f));
        StartCoroutine(MeshTrail.IEnumerator_Trail(gameObject, 10, 2,0.25f,trailMaterial));
        if (shotEffectCoroutine == null)
        {
            shotEffectCoroutine = StartCoroutine(IEnumerator_ShotEffect());
        }
        for ( dashCount = dashCountMax; dashCount>0 ;dashCount--)
        {
            rb2d.linearVelocityX = (Vector2.right * (int)lastFacing * Time.fixedDeltaTime * dashCount * dashForce).x;
            jumpCount = 0;
            yield return new WaitForFixedUpdate();
        }

        dashingCoroutine = null;
    }

    Coroutine attackChargingCoroutine = null;
    IEnumerator IEnumerator_AttackCharge()
    {
        chargeAmout = 0;
        while (attackCharging)
        {
            if (bulletAmount <= 0)
            {
                chargeAmout = 0;
                chargeCompleteEffect.Stop();
                chargeEffect.SetActive( false );
                StopCoroutine(attackChargingCoroutine);
                attackChargingCoroutine = null;
            }
            chargeAmout++;
            if (chargeAmout >15)
            {
                if (!chargeEffect.activeSelf)
                {
                    chargeEffect.SetActive( true );
                }
            }
            if (chargeAmout == 119)
            {
                StartCoroutine(PlayerGamepadHaptics.IEnumerator_GamepadHaptics(0.15f, 0.0f, 0.1f));
            }
            if (chargeAmout > 120)
            {
                chargeAmout = 120;

                    chargeCompleteEffect.Play();
            }
            yield return new WaitForFixedUpdate();
        }
        if (quickFireCount >28 && quickFireCount <45)
        {
            HitStop.instance.StartHitStop(0.025f);

            if (shotEffectCoroutine == null)
            {
                shotEffectCoroutine = StartCoroutine(IEnumerator_ShotEffect());
            }
            StartCoroutine(PlayerGamepadHaptics.IEnumerator_GamepadHaptics(0.15f, 0.01f, 0.05f));
            var tmp_bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.Euler(0, 0, 0)).GetComponent<PlayerBullet>();
            tmp_bullet.direction  = Vector2.right * (int)lastFacing;
            tmp_bullet.damage *= 2;
            tmp_bullet.speed *= 1.5f;
            tmp_bullet.transform.localScale = new Vector3( (float)lastFacing, 1.25f , 1.25f );
            tmp_bullet.quickFire = true;
            chargeCompleteEffect.Stop();
            chargeEffect.SetActive( false );
            attackChargingCoroutine = null;
            chargeAmout = 0;
            quickFireBrustEffect.Stop();
            quickFireBrustEffect.Play();
            StopCoroutine(quickFireCoroutine);
            quickFireCoroutine = null;
            quickFireCount = 0;
        }
        else if ( bulletAmount >= 1 && chargeAmout > 2)
        {
            if (shotEffectCoroutine == null)
            {
                shotEffectCoroutine = StartCoroutine(IEnumerator_ShotEffect());
            }
            var tmp_bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.Euler(0, 0, 0)).GetComponent<PlayerBullet>();
            tmp_bullet.direction  = Vector2.right * (int)lastFacing;
            if (chargeAmout >= 120 && bulletAmount >= 2)
            {
                anim.SetTrigger("ChargeShoot");
                FindFirstObjectByType<Camera>().DOShakePosition(0.25f,1.5f*((float)bulletAmount/6f));
                StartCoroutine(PlayerGamepadHaptics.IEnumerator_GamepadHaptics(0.15f, 0.5f, 0.5f));
                chargeAmout = 0;
                chargeCompleteEffect.Stop();
                chargeEffect.SetActive( false );
                attackChargingCoroutine = null;
                tmp_bullet.damage *= (int) (bulletAmount * 1.5f);
                tmp_bullet.chargeAttack = true;
                tmp_bullet.transform.localScale = new Vector3(1.5f * (float)lastFacing, 1f *  bulletAmount, 1f * bulletAmount);
                tmp_bullet.speed *= 1.05f;
                bulletAmount = 0;
                reloadingCoroutine = StartCoroutine(IEnumerator_Reloading());
                StartCoroutine(MeshTrail.IEnumerator_Trail(tmp_bullet.gameObject, 600, 3,0.35f,trailMaterial));
                for (int i = 0; i < damageTime; i++)
                {
                    rb2d.linearVelocity =new Vector2( -(float)lastFacing* speed* (((damageTime-i)*3 / damageTime)+1)*0.25f  ,0 )  * Time.smoothDeltaTime;
                    yield return new WaitForFixedUpdate();
                }

            }
            else
            {
                StartCoroutine(PlayerGamepadHaptics.IEnumerator_GamepadHaptics(0.1f, 0.0f, 0.01f));
                chargeAmout = 0;
                chargeCompleteEffect.Stop();
                chargeEffect.SetActive( false );
                attackChargingCoroutine = null;
                tmp_bullet.transform.localScale = new Vector3( (float)lastFacing, 1f , 1f );
                playerUI.BulletIncreaseAndDecrease(-1);
                bulletAmount --;
            }
            tmp_bullet.player = this;
        }
        else
        {
            chargeAmout = 0;
            chargeCompleteEffect.Stop();
            chargeEffect.SetActive( false );
            attackChargingCoroutine = null;
        }
    }
    Coroutine subWeapon_InfinityBulletCoroutine = null;
    IEnumerator IEnumerator_SubWeapon_InfinityBullet()
    {
        if (shotEffectCoroutine == null)
        {
            shotEffectCoroutine = StartCoroutine(IEnumerator_ShotEffect());
        }
        StartCoroutine(PlayerGamepadHaptics.IEnumerator_GamepadHaptics(0.15f, 0, 0.01f));
        var tmp_bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.Euler(0, 0, 0)).GetComponent<PlayerBullet>();
        tmp_bullet.direction  = Vector2.right * (int)lastFacing;
        tmp_bullet.damage = 3;
        tmp_bullet.speed *= 0.95f;
        tmp_bullet.transform.localScale = new Vector3( (float)lastFacing*0.25f, 0.75f , 0.25f );
        for (int i = 0; i < 8; i++)
        {
            yield return new WaitForFixedUpdate();
        }
        subWeapon_InfinityBulletCoroutine = null;
    }

    Coroutine shotEffectCoroutine = null;

    IEnumerator IEnumerator_ShotEffect()
    {
        shotEffect.Play(true);
        shotLightEffect.enabled = true;
        for (int i = 0; i < 5; i++)
        {
            yield return new WaitForFixedUpdate();
        }
        shotLightEffect.enabled = false;
        shotEffectCoroutine = null;
    }

    Coroutine quickFireCoroutine = null;
    IEnumerator IEnumerator_QuickFire()
    {
        quickFireEffect.Play();
        for (quickFireCount =0; quickFireCount < 60; quickFireCount++)
        {
            if (quickFireCount == 25)
            {
                StartCoroutine(PlayerGamepadHaptics.IEnumerator_GamepadHaptics(0.1f, 0.0f, 0.05f));
            }
            yield return new WaitForFixedUpdate();
        }
        quickFireEffect.Stop();
        quickFireCoroutine = null;
    }
    #endregion
    #region Method

    void Dash()
    {
        if (dashAction.WasPressedThisFrame()　&& (!reloading && bulletAmount > 0))
        {
            dashingCoroutine = StartCoroutine(IEnumerator_Dashing());
            Destroy(Instantiate(brustEffect,transform.position,Quaternion.Euler(0,90 * (float)lastFacing,0)),1f);
            playerUI.BulletIncreaseAndDecrease(-1);
            bulletAmount--;
        }
    }

    void Jump()
    {
        if (gounding )
        {

            if (jumpAndFallCoroutine == null)
            {
                if (jumpAction.WasPerformedThisFrame()  && playeCanInput )
                {
                    jumpPressing =  true;

                    jumpAndFallCoroutine = StartCoroutine(IEnumerator_JumpAndFall(false));
                }
            }

            canDoubleJump = true;
        }
        else
        {
            if (jumpAndFallCoroutine == null)
            {

                jumpAndFallCoroutine = StartCoroutine(IEnumerator_JumpAndFall(true));
            }
            else
            {

                if (canDoubleJump && jumpAction.WasPerformedThisFrame()
                                  && jumpCount < jumpCountMax - 10 && bulletAmount > 0 && !reloading && playeCanInput)
                {
                    if (shotEffectCoroutine == null)
                    {
                        shotEffectCoroutine = StartCoroutine(IEnumerator_ShotEffect());
                    }
                    StopCoroutine(jumpAndFallCoroutine);
                    jumpAndFallCoroutine = StartCoroutine(IEnumerator_JumpAndFall(false ));
                    canDoubleJump = false;
                    playerUI.BulletIncreaseAndDecrease(-1);
                    bulletAmount--;
                    anim.SetTrigger("DoubleJump");

                   Destroy(Instantiate(brustEffect,new Vector3(triggerCollider.bounds.center.x,triggerCollider.bounds.min.y,transform.position.z),Quaternion.Euler(-90,0,0)),1f);
                   StartCoroutine(PlayerGamepadHaptics.IEnumerator_GamepadHaptics(0.1f, 0.0f, 0.01f));
                   StartCoroutine(MeshTrail.IEnumerator_Trail(gameObject, 10, 2,0.25f,trailMaterial));
                }
            }
        }

        if (jumpAction.WasReleasedThisFrame())
        {
            jumpPressing = false;
        }
    }

    void Ground()
    {
        Ray2D groundRayCenter = new Ray2D( new Vector2(triggerCollider.bounds.center.x,triggerCollider.bounds.min.y), -Vector2.up);
        RaycastHit2D groundHitCenter = Physics2D.Raycast(groundRayCenter.origin, Vector2.down,0.1f, GroundLayer);
        Debug.DrawRay(groundRayCenter.origin, Vector2.down * 0.1f,Color.magenta);
        Ray2D groundRayL = new Ray2D( new Vector2(triggerCollider.bounds.min.x,triggerCollider.bounds.min.y), -Vector2.up);
        RaycastHit2D groundHitL = Physics2D.Raycast(groundRayL.origin, Vector2.down,0.1f, GroundLayer);
        Debug.DrawRay(groundRayL.origin, Vector2.down * 0.1f,Color.red);
        Ray2D groundRayR = new Ray2D( new Vector2(triggerCollider.bounds.max.x,triggerCollider.bounds.min.y), -Vector2.up);
        RaycastHit2D groundHitR = Physics2D.Raycast(groundRayR.origin, Vector2.down,0.1f, GroundLayer);
        Debug.DrawRay(groundRayR.origin, Vector2.down * 0.1f,Color.cyan);

        gounding = groundHitCenter || groundHitL || groundHitR;
        if (!gounding)
        {
            if (transform.parent != null)
            {
                transform.parent = null;
            }
        }
        anim.SetBool("Grounding",gounding);
    }

    void Movement()
    {
        if (!playeCanInput)
        {
            rb2d.linearVelocityX = 0;
            anim.SetBool("XvectorNotZero",false);
            return;
        }

        var moveVectorX = moveAction.ReadValue<Vector2>().x;
        if (moveVectorX < 0 )
        {
            lastFacing = Facing.L;
        }
        else if (moveVectorX > 0)
        {
            lastFacing = Facing.R;
        }
        else
        {
            moveVectorX = 0;
        }
        rb2d.linearVelocityX =  moveVectorX * speed * Time.fixedDeltaTime ;
        anim.SetBool("XvectorNotZero",moveVectorX!=0);

    }

    void Attack()
    {
        if (reloadingCoroutine == null)
        {
            switch (subWeapons)
            {
                case SubWeapons.None :
                    attackCharging =  attackAction.IsPressed() ;
                    if (attackCharging && attackChargingCoroutine == null && bulletAmount > 0)
                    {
                        attackChargingCoroutine = StartCoroutine(IEnumerator_AttackCharge());
                    }
                    if ((!attackCharging && attackChargingCoroutine != null)|| bulletAmount <= 0)
                    {
                        StopCoroutine(IEnumerator_AttackCharge());
                    }

                    if(
                        (
                            (  bulletAmount <= 0 && attackAction.WasPressedThisFrame() ) || reloadAction.WasPerformedThisFrame()
                        )
                        && gounding
                        && jumpAndFallCoroutine == null
                    )
                    {
                        anim.SetTrigger("Reload");
                        reloadingCoroutine = StartCoroutine(IEnumerator_Reloading());
                        attackCharging = false;
                        chargeAmout = 0;
                    }

                    if (subWeaponsAction.WasPressedThisFrame() && !attackCharging && attackChargingCoroutine == null)
                    {
                        subWeapons = SubWeapons.InfinityBullet;
                        playerUI.Infinity(true);
                        StartCoroutine(PlayerGamepadHaptics.IEnumerator_GamepadHaptics(0.1f, 0.0f, 0.01f));
                    }
                    break;
                case SubWeapons.InfinityBullet :
                    if (attackAction.IsPressed())
                    {
                        if (subWeapon_InfinityBulletCoroutine == null)
                        {
                            subWeapon_InfinityBulletCoroutine = StartCoroutine(IEnumerator_SubWeapon_InfinityBullet());
                        }
                    }
                    else
                    {
                        if (subWeapon_InfinityBulletCoroutine != null)
                        {
                            StopCoroutine(subWeapon_InfinityBulletCoroutine);
                            subWeapon_InfinityBulletCoroutine = null;
                        }
                    }

                    if((reloadAction.WasPerformedThisFrame()) && gounding && jumpAndFallCoroutine == null)
                    {
                        anim.SetTrigger("Reload");
                        reloadingCoroutine = StartCoroutine(IEnumerator_Reloading());
                    }

                    if (subWeaponsAction.WasPressedThisFrame() && !attackAction.IsPressed() && subWeapon_InfinityBulletCoroutine==null)
                    {
                        subWeapons = SubWeapons.None;
                        playerUI.Infinity(false);
                        StartCoroutine(PlayerGamepadHaptics.IEnumerator_GamepadHaptics(0.1f, 0.0f, 0.01f));
                    }
                    break;
            }
        }
        else
        {
            attackCharging = false;
            chargeAmout = 0;
        }
    }

    void Face()
    {
        if (lastFacing == Facing.L)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
    }

    public void BulletHittedEnemy()
    {
        if (quickFireCoroutine == null && bulletAmount > 0)
        {
            quickFireCoroutine = StartCoroutine(IEnumerator_QuickFire());
        }
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        if (gounding )
        {
            other.gameObject.TryGetComponent(out floatingPlatform tmp_floatingPlatform);
            if(tmp_floatingPlatform!=null)
                gameObject.transform.SetParent(tmp_floatingPlatform.transform);
        }
    }
    void OnTriggerEnter2D(Collider2D other)
    {

        other.gameObject.TryGetComponent(out iItem tmp_item);
        if (tmp_item != null)
        {
            tmp_item.GetItem(this);
            return;
        }

        if (dashingCoroutine != null || damageCoroutine != null　|| invicibleCoroutine != null || reloadingCoroutine != null)
        {
            return;
        }

        other.gameObject.TryGetComponent( out iCanDamegePlayer tmp_damagedPlayer);
        if (tmp_damagedPlayer != null)
        {
            if (lastFacing == Facing.R && other.transform.position.x < transform.position.x )
            {
                transform.localScale = new Vector3(1 * -(int)lastFacing, 1, 1);
                    lastFacing = Facing.L;
            }
            else if(lastFacing == Facing.L && other.transform.position.x > transform.position.x)
            {
                transform.localScale = new Vector3(1 * -(int)lastFacing, 1, 1);
                lastFacing = Facing.R;
            }
            tmp_damagedPlayer.DealDamageToPlayer(ref hp);
            damageCoroutine = StartCoroutine(IEnumerator_Damage());
            if (hp < 0)
            {
                hp = 0;
            }
            playerUI.HPUI(hp,true);
        }
    }
    void OnTriggerStay2D(Collider2D other)
    {

        if (dashingCoroutine != null || damageCoroutine != null　|| invicibleCoroutine != null|| reloadingCoroutine != null)
        {
            return;
        }

        other.gameObject.TryGetComponent( out iCanDamegePlayer tmp_damagedPlayer);
        if (tmp_damagedPlayer != null)
        {
            if (lastFacing == Facing.R && other.transform.position.x < transform.position.x )
            {
                transform.localScale = new Vector3(1 * -(int)lastFacing, 1, 1);
                lastFacing = Facing.L;
            }
            else if(lastFacing == Facing.L && other.transform.position.x > transform.position.x)
            {
                transform.localScale = new Vector3(1 * -(int)lastFacing, 1, 1);
                lastFacing = Facing.R;
            }
            tmp_damagedPlayer.DealDamageToPlayer(ref hp);
            damageCoroutine = StartCoroutine(IEnumerator_Damage());
            if (hp < 0)
            {
                hp = 0;
            }
            playerUI.HPUI(hp,true);
        }
    }
    #endregion
    #region Start & Update

    void Awake()
    {
    }

    void Start()
    {
        transform.position = FindFirstObjectByType<CheckPointSystem>().RespawnPosition();

        playerUI = GameObject.FindAnyObjectByType<PlayerUI>();
        playerInput = GameObject.FindAnyObjectByType<PlayerInput>();
        moveAction = playerInput.actions["Move"];
        jumpAction  = playerInput.actions["Jump"];
        attackAction= playerInput.actions["Attack"];
        reloadAction= playerInput.actions["Reload"];
        dashAction= playerInput.actions["Dash"];
        subWeaponsAction = playerInput.actions["SubWeapons"];
        bulletAmount = bulletAmountMax;
        rb2d = GetComponent<Rigidbody2D>();
    }
    void Update()
    {

        if (Keyboard.current.f8Key.wasPressedThisFrame)
        {
            transform.position = new Vector3(650, 70, 0);
        }

        if (hp <= 0)
        {
            if (deathCoroutine == null)
            {
                StopAllCoroutines();
                deathCoroutine =  StartCoroutine( IEnumerator_Death());
            }
        }
        else
        {
            Ground();
            if (dashingCoroutine == null   &&
                reloadingCoroutine == null &&
                damageCoroutine == null)
            {
                Jump();
                if (playeCanInput)
                {
                    Dash();
                    Attack();
                }
            }
        }
    }
    private void FixedUpdate()
    {
        if (dashingCoroutine == null   &&
            reloadingCoroutine == null &&
            damageCoroutine == null &&
            hp > 0 )
        {
            Movement();
        }
    }
    void LateUpdate()
    {
        if (hp > 0)
        {
            Face();

        }
    }
    #endregion
}
