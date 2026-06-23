using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Eflatun.SceneReference;
using UnityEngine.UI;
public class InGameSceneManager : MonoBehaviour
{
    static InGameSceneManager instance;

    private void Awake()
    {

        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }

        instance = this;
        Application.targetFrameRate = 165;
    }

    public SceneReference IngameScene;
    public SceneReference TitleScene;

    public IEnumerator loadSceneRef(SceneReference _scene , int _time , bool _ingame = true)
    {
        for (int i = 0; i < _time; i++)
        {
            yield return new WaitForFixedUpdate();
        }
        if (_ingame)
        {
            FindFirstObjectByType<PlayerUI>().BlackScreen();
        }
        for (int i = 0; i < _time; i++)
        {
            yield return new WaitForFixedUpdate();
        }
        SceneManager.LoadScene(_scene.Name);
        DontDestroyOnLoad(gameObject);
    }
}
