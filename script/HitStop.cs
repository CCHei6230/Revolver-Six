using UnityEngine;
using System.Collections;

public class HitStop : MonoBehaviour
{

    public static HitStop instance;

    private void Start()
    {
        instance = this;
    }

    public void StartHitStop(float duration)
    {
        instance.StartCoroutine(instance.HitStopCoroutine(duration));
    }

    private IEnumerator HitStopCoroutine(float duration)
    {

        Time.timeScale = 0.1f;

        yield return new WaitForSecondsRealtime(duration);

        Time.timeScale = 1f;
    }
}
