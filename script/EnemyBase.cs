using System;
using UnityEngine;
using System.Collections;

public  class EnemyBase : MonoBehaviour , iCanDamegePlayer
{
    public int damage;
    public int hpMax = 25;
    public int hp = 25;

    protected MeshRenderer meshRenderer;
    [SerializeField] protected GameObject damageEffect;
    [SerializeField] protected GameObject deathEffect;
    [SerializeField] protected Material deathMaterial;
    [SerializeField] protected Material damageMaterial;

    protected void Start()
    {
        hp = hpMax;
        meshRenderer = GetComponentInChildren<MeshRenderer>();
    }
    void Update()
    {
    }

    protected Coroutine damageFlashCoroutine = null;
    protected IEnumerator IEnumerator_DamageFlash()
    {
        var meshrender = GetComponentsInChildren<MeshRenderer>();
        Material[] tmp_mats  =  new Material[meshrender.Length];
        for (int x = 0; x < 4; x++)
        {
            for (int i = 0; i < meshrender.Length; i++)
            {
                tmp_mats[i] = meshrender[i].material;
                meshrender[i].material =  damageMaterial;
            }
            yield return new WaitForFixedUpdate();
            for (int i = 0; i < meshrender.Length; i++)
            {
                meshrender[i].material =  tmp_mats[i];
            }
            yield return new WaitForFixedUpdate();
        }
        damageFlashCoroutine = null;
    }

    virtual protected  IEnumerator IEnumerator_Damage()
    {

        if (damageFlashCoroutine == null && hp > 0)
        {
            damageFlashCoroutine = StartCoroutine(IEnumerator_DamageFlash());
        }

        {

        }

        {

            yield return new WaitForFixedUpdate();
        }
        damageCoroutine = null;
    }

    protected Coroutine damageCoroutine = null;

      protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        other.gameObject.TryGetComponent<iCanDamegeEnemy>
            ( out iCanDamegeEnemy tmp_damagedEnemy);
        if (tmp_damagedEnemy !=null)
        {
            tmp_damagedEnemy.DealDamageToEnemy(ref hp);
            Destroy(Instantiate(damageEffect, transform.position, Quaternion.identity),3f);
            if (hp <= 0)
            {
                hp = 0;

                Death();
            }
            damageCoroutine = StartCoroutine(IEnumerator_Damage());
        }
    }

    protected virtual void Death()
    {
        Destroy(gameObject);
    }

    public void DealDamageToPlayer(ref int _playerHP )
    {
        _playerHP -= damage;
    }

}
