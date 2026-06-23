using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour
{
    PlayerInput playerInput;

    void Start()
    {
        playerInput = FindFirstObjectByType<PlayerInput>();
    }

    void Update()
    {
        if (playerInput.actions["Start"].WasPressedThisFrame())
        {
            GetComponent<Animator>().Play("TitleOut");

        }
    }

    void Animation_TitleOut()
    {
        var tmp_inGameSceneManager = FindFirstObjectByType<InGameSceneManager>();
        StartCoroutine(tmp_inGameSceneManager.loadSceneRef(tmp_inGameSceneManager.IngameScene,10,false));
    }
}
