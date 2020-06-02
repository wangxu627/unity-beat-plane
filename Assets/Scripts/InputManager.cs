using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SunnyCat.Tools;

public class InputManager : Singleton<InputManager>
{
    public enum ControllerType {
        Mouse,
        Joystick,
        VirtualJoystick
    }

    public ControllerType controllerType;
    public GameObject virtualJoystickUI;
    // Start is called before the first frame update

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        virtualJoystickUI.active = (controllerType == ControllerType.VirtualJoystick);
    }

}
