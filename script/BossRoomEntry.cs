using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class BossRoomEntry : MonoBehaviour
{
   [SerializeField]Transform cameratarget;
   [SerializeField] private CinemachineCamera camera;
   [SerializeField] private GameObject bossIntroPrefab;
   [SerializeField] private GameObject bossPrefab;
   [SerializeField] private Transform door;
   [SerializeField] private Transform doorClosePos;

    void Start()
    {
        camera = FindFirstObjectByType<CinemachineCamera>();
    }

    IEnumerator IEnumerator_StartBossFight()
    {
        GetComponent<Collider2D>().enabled = false;
        PlayerMainBahvior.playeCanInput = false;
        var tmp_bossIntro = Instantiate(bossIntroPrefab, GameObject.FindGameObjectWithTag("UICameraEffect").transform);
        tmp_bossIntro.transform.DOScale(1, 0.5f);
        var tmp_introImage = tmp_bossIntro.GetComponent<Image>();
        tmp_introImage.DOFade(1, 0.45f).SetLoops(10, LoopType.Yoyo);
        tmp_introImage.DOFillAmount(1,0.7f);
        door.DOMoveY(doorClosePos.position.y, 2f);
        camera.transform.position = cameratarget.position;

        camera.Target.TrackingTarget = cameratarget;
        Instantiate(bossPrefab,cameratarget.position,Quaternion.identity);
        for (int i = 0; i < 180; i++)
        {
            yield return new WaitForFixedUpdate();
        }

        tmp_introImage.fillOrigin = 1;
        tmp_introImage.DOFillAmount(0,0.7f);
        tmp_bossIntro.transform.DOScale(1.3f, 0.5f).OnComplete(()=>Destroy(tmp_bossIntro));
        PlayerMainBahvior.playeCanInput = true;

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(IEnumerator_StartBossFight());
        }
    }
}
