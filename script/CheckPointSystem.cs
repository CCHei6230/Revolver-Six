using System;
using UnityEngine;

public class CheckPointSystem : MonoBehaviour
{
    public Vector3 lastCheckPointPosition;
    public int lastCheckPointSlot = -1;
    static public CheckPointSystem instance = null;

    private void Awake()
    {
        if (instance && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public Vector3 RespawnPosition()
    {
        if (lastCheckPointSlot != -1)
        {
            FindFirstObjectByType<CheckPointList>().checkPoints[lastCheckPointSlot].SetActive(false);
        }
        return lastCheckPointPosition;
    }
}
