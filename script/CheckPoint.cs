using System;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    [SerializeField] ParticleSystem effect;
    [SerializeField] Transform SpawnPosition;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            effect.Play();
            var tmp_checkPointSys = FindFirstObjectByType<CheckPointSystem>();
            tmp_checkPointSys.lastCheckPointPosition = SpawnPosition.position;
            tmp_checkPointSys.lastCheckPointSlot = FindFirstObjectByType<CheckPointList>().CheckPointSlot(gameObject);
            Destroy(GetComponent<Collider2D>());
        }
    }
}
