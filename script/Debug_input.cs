using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class Debug_input : MonoBehaviour
{
    [SerializeField] PlayerInput playerInput;
    [SerializeField] Image Icon;
    [SerializeField] Sprite KeyboardIcon;
    [SerializeField] Sprite GamepadIcon;
    [SerializeField] Sprite TouchIcon;
    void Start()
    {
    }
    void Update()
    {
        switch (playerInput.currentControlScheme)
        {
            case "Keyboard&Mouse" :
                Icon.sprite = KeyboardIcon;
                break;
            case "Gamepad" :
                Icon.sprite = GamepadIcon;
                break;
            case  "Touch" :
                Icon.sprite = TouchIcon;
                  break;
        }
    }
}
