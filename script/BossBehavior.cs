using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using DG.Tweening;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class BossBehavior : EnemyBase
{
    [SerializeField] GameObject bossUiObj;
    [SerializeField] Image bossHpBar;
    [SerializeField] Color bossHpBarOriginalColor;
    [SerializeField] TextMeshProUGUI bossHpText;
    void Start()
    {
        base.Start();
        bossUiObj.transform.parent = GameObject.FindGameObjectWithTag("BossUI").transform;
        bossUiObj.GetComponent<RectTransform>().localScale = Vector3.one;
        bossUiObj.GetComponent<RectTransform>().anchoredPosition = new Vector3(0,-250,0);
        bossUiObj.GetComponent<RectTransform>().DOLocalMove(Vector3.zero, 0.8f)
            .OnComplete(()=>bossHpBar.DOFillAmount(1,1.5f));
        bossHpText.text = hp.ToString() + "/" + hpMax.ToString();
        bossHpBarOriginalColor = bossHpBar.color;
    }

    void Update()
    {

        if (Keyboard.current.f6Key.wasPressedThisFrame)
        {
            hp = 50;
        }
    }

    override protected  IEnumerator IEnumerator_Damage()
    {
        bossHpBar.DOComplete();
        bossHpBar.DOFillAmount((float)hp / (float)hpMax, 0.2f);
        bossHpBar.DOColor(Color.red, 0.1f).
            OnComplete(()=>bossHpBar.DOColor(bossHpBarOriginalColor, 0.1f));
        StartCoroutine(PlayerGamepadHaptics.IEnumerator_GamepadHaptics(0.15f, 0.0f, 0.02f));
        if (damageFlashCoroutine == null && hp > 0)
        {
            damageFlashCoroutine = StartCoroutine(IEnumerator_DamageFlash());
        }
        bossHpText.text = hp.ToString() + "/" + hpMax.ToString();
        damageCoroutine = null;
        yield return 0;
    }
    protected override void OnTriggerEnter2D(Collider2D other)
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
                Destroy(GetComponent<Collider2D>());
                Death();
            }
            damageCoroutine = StartCoroutine(IEnumerator_Damage());
        }
    }
    protected override void Death()
    {
        DOTween.Kill(this,false);
        StartCoroutine(PlayerGamepadHaptics.IEnumerator_GamepadHaptics(0.2f, 0.3f, 0.3f));
        var tmp_meshRenderer = GetComponentsInChildren<MeshRenderer>();
        for (int i = 0; i < tmp_meshRenderer.Length; i++)
        {
            tmp_meshRenderer[i].materials = new Material[1]{damageMaterial};
        }
        HitStop.instance.StartHitStop(1.5f);
        transform.DOScale(Vector3.one * 1.15f, 0.15f).
            OnComplete(()=>transform.DOScale(Vector3.zero, 0.005f));
        Destroy(Instantiate(deathEffect,transform.position+ new Vector3(0,3,0),Quaternion.identity),3f);
        Destroy(GetComponent<Animator>());
        var tmp_InGameSceneManager = FindFirstObjectByType<InGameSceneManager>();
        StartCoroutine(tmp_InGameSceneManager.loadSceneRef(tmp_InGameSceneManager.TitleScene,90));
        Destroy(CheckPointSystem.instance.gameObject);
    }

    [Serializable]
    enum AttackDirtion
    {
        Left,
        Right,
        Up,
        Down
    }

    [SerializeField] Transform shotPosition;
    [SerializeField] GameObject projectilePrefab;
    void Animator_Attack(AttackDirtion _direction)
    {
        Vector2 tmp_direction = Vector2.zero;
        Quaternion tmp_rot = Quaternion.identity;
        int _amout = 1;
        switch (_direction)
        {
            case AttackDirtion.Left:
                tmp_direction = Vector2.left;
                tmp_rot = Quaternion.Euler(0,0,0);
                _amout = 2;
                break;
            case AttackDirtion.Right:
                tmp_direction = Vector2.right;
                tmp_rot = Quaternion.Euler(0,180,0);
                _amout = 2;
                break;
            case AttackDirtion.Up:
                tmp_direction = Vector2.up;
                tmp_rot = Quaternion.Euler(0,0,-90);
                _amout = 1;
                break;
            case AttackDirtion.Down:
                tmp_direction = Vector2.down;
                tmp_rot = Quaternion.Euler(0,0,90);
                _amout = 1;
                break;
        }

        for (int i = 0; i < _amout ; i++)
        {
            var tmp_attack = Instantiate(projectilePrefab,shotPosition.position, tmp_rot).GetComponent<EnemyProjectile>();
            tmp_attack.speed *= (i+1);
            tmp_attack.direction =tmp_direction;
        }

    }

    void Animator_Random()
    {
        GetComponent<Animator>().SetBool("Random",Random.value < 0.5f); ;
    }

}
