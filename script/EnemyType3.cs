using System.Collections;
using DG.Tweening;
using UnityEngine;

public class EnemyType3 : EnemyMoveBetweenTwoPoint
{
    PlayerDetector playerDetector;
    Transform playerTransform;
    [SerializeField] private GameObject attackPrefab;
    [SerializeField] private Transform shootPosition;

    void Start()
    {
        playerDetector = GetComponentInChildren<PlayerDetector>();
        base.Start();
    }

    void Update()
    {
        base.Update();
        if (playerDetector.playerInRange)
        {
            playerTransform = playerDetector.player;
            if (attackCoroutine == null)
            {
                attackCoroutine = StartCoroutine(IEnumerator_Attack());
            }
        }
    }

    Coroutine attackCoroutine = null;
    IEnumerator IEnumerator_Attack()
    {
        tweenOnGoning.Pause();
        Debug.Log("Start Attack");

        for (int i = 0; i < 30; i++)
        {
            yield return new WaitForFixedUpdate();
        }
        var tmp_attack = Instantiate(attackPrefab,shootPosition.position, Quaternion.Euler(0,(float)facing>0?180:0,0)).GetComponent<EnemyProjectile>();
        tmp_attack.direction =new Vector2(1* (float)facing,0);

        for (int i = 0; i < 30; i++)
        {
            yield return new WaitForFixedUpdate();
        }
        Debug.Log("End Attack");
        tweenOnGoning.Play();
        StartCoroutine(IEnumerator_CoolDown());
    }
    IEnumerator IEnumerator_CoolDown()
    {
        Debug.Log("Start CoolDown");

        for (int i = 0; i < 60*6; i++)
        {
            yield return new WaitForFixedUpdate();
        }
        Debug.Log("End CoolDown");
        attackCoroutine = null;
    }

}
