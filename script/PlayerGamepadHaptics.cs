using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerGamepadHaptics : MonoBehaviour
{
   public static IEnumerator IEnumerator_GamepadHaptics(float _time, float _lowfreq, float _highfreq )
    {
       var  gamepad = Gamepad.current;
        if (gamepad == null )
        {
            yield break;
        }
        gamepad.SetMotorSpeeds(_lowfreq, _highfreq);
        yield return new WaitForSecondsRealtime(_time);
        gamepad.SetMotorSpeeds(0.0f, 0.0f);
    }

}
