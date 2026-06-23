using System;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
public class Debug_FPS : MonoBehaviour
{
    public TMP_Text text;

    private void Start()
    {
        Application.targetFrameRate = 120;
    }

    void Update()
    {
        text.text = "FPS: " + (int) (1.0f/Time.smoothDeltaTime);
        if (Keyboard.current.f1Key.wasPressedThisFrame)
        {
            Application.targetFrameRate = 30;
        }
        if (Keyboard.current.f2Key.wasPressedThisFrame)
        {
            Application.targetFrameRate = 60;
        }
        if (Keyboard.current.f3Key.wasPressedThisFrame)
        {
            Application.targetFrameRate = 90;
        }
        if (Keyboard.current.f4Key.wasPressedThisFrame)
        {
            Application.targetFrameRate = 120;
        }
    }
}
