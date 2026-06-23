using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Debug_loadScene : MonoBehaviour
{

    PlayerInput playerInput;
    void Start()
    {
        playerInput = FindFirstObjectByType<PlayerInput>();
    }
    void Update()
    {
        if (playerInput.actions["Restart"].WasPressedThisFrame())
        {
            var tmp_ingameSceneManager = FindFirstObjectByType<InGameSceneManager>();
            StartCoroutine(tmp_ingameSceneManager.loadSceneRef(tmp_ingameSceneManager.TitleScene, 1));
        }
    }
}
