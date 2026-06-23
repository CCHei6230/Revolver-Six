using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;
using DG.Tweening;

public class EnemyNonBossBase : EnemyBase
{
    [SerializeField] protected GameObject hpBarObject;
    [SerializeField] protected Image hpBar;
    [SerializeField]protected Gradient gradient;

    protected void Start()
    {
        base.Start();
        hpBarObject.transform.SetParent(GameObject.FindWithTag("EnemyUI").transform);
        hpBar.fillAmount = 1;
        hpBarObject.GetComponent<UIObjectBase>().Target = transform;
        hpBarObject.transform.localScale = new Vector3(1, 0.1f, 0.2f);
        hpBar.color = gradient.Evaluate(0);
    }

    void Update()
    {

    }
    Coroutine damageFlashCoroutine = null;
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

    override protected  IEnumerator IEnumerator_Damage()
    {
        hpBar.DOFillAmount((float)hp / (float)hpMax, 0.1f).SetEase(Ease.InOutQuint);
        hpBar.color = gradient.Evaluate(1-((float)hp / (float)hpMax));

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
    protected override void Death()
    {
        Destroy(Instantiate(deathEffect, transform.position, Quaternion.identity),5);
        var tmp_meshRenderer = GetComponentsInChildren<MeshRenderer>();
        for (int i = 0; i < tmp_meshRenderer.Length; i++)
        {
            tmp_meshRenderer[i].materials = new Material[1]{deathMaterial};
        }
        FindFirstObjectByType<Camera>().DOShakePosition(0.2f,0.5f);
        Destroy(hpBarObject);
        DOTween.Kill(this,false);
        HitStop.instance.StartHitStop(0.04f);
        Destroy(gameObject,0.075f);
        Destroy(this);

    }

}
