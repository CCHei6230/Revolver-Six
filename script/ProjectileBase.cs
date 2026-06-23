using System;
using UnityEngine;
using System.Collections;

public class ProjectileBase :MonoBehaviour
{

    protected MeshRenderer meshRenderer;
    protected Rigidbody2D rb2d;
    public int damage;
    public Vector2 direction;
    public float speed;
    protected void Start()
    {

        rb2d = GetComponent<Rigidbody2D>();
        rb2d.linearVelocity = direction * speed * Time.fixedDeltaTime;

        meshRenderer = GetComponentInChildren<MeshRenderer>();
    }

    protected Coroutine DestroyBulletCoroutine = null;
    protected IEnumerator IEnumerator_DestroyBullet(float _seconds)
    {

        for (int i = 0; i < 60*_seconds; i++)
        {
            yield return new WaitForFixedUpdate();
        }
        Destroy(gameObject);
    }
    protected  virtual void Update()
    {

        if (!meshRenderer.isVisible )
        {

            if (DestroyBulletCoroutine == null)
                DestroyBulletCoroutine = StartCoroutine(IEnumerator_DestroyBullet(0.25f));
        }

        else
        {

            if (DestroyBulletCoroutine != null)
            {
                StopCoroutine(DestroyBulletCoroutine);
                DestroyBulletCoroutine = null;
            }
        }
    }
}
