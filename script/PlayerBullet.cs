using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerBullet : ProjectileBase , iCanDamegeEnemy
{
    public bool chargeAttack = false;
    public PlayerMainBahvior player;
    public bool quickFire = false;
    public TrailRenderer quickFireEffect ;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] Collider2D collider2D;
    [SerializeField] GameObject bulletEffect;
    void Start()
    {
        if (quickFire)
        {
            quickFireEffect.enabled = true;
        }
        base.Start();
    }
    void  Update()
    {
        if (!chargeAttack && !quickFire)
        {
            Ray2D groundRayCenter = new Ray2D( new Vector2(collider2D.bounds.center.x,collider2D.bounds.min.y), direction);
            RaycastHit2D groundHitCenter = Physics2D.Raycast(groundRayCenter.origin, direction ,0.1f, groundLayer);
            if (groundHitCenter)
            {
                Destroy(Instantiate(bulletEffect,transform.position,Quaternion.identity),1f);
                Destroy(gameObject);
            }
        }
        base.Update();
    }

    public void DealDamageToEnemy(ref int _enemyHP)
    {
        _enemyHP -= damage;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (chargeAttack)
        {
            return;
        }
        other.gameObject.TryGetComponent<EnemyBase>
            ( out EnemyBase tmp_Enemy);
        if (tmp_Enemy !=null)
        {
            if (player != null)
            {
                player.BulletHittedEnemy();
            }
            Destroy(Instantiate(bulletEffect,transform.position,Quaternion.identity),1f);
            Destroy(gameObject);
        }
    }
}
