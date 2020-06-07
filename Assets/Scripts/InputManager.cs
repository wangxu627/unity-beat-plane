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
    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        if(controllerType != ControllerType.VirtualJoystick)
        {
            virtualJoystickUI.SetActive(false);
        }
    }

    public void SetVirutalJoystickActive(bool active)
    {
        if(controllerType != ControllerType.VirtualJoystick)
        {
            return;
        }
        virtualJoystickUI.SetActive(active);
    }

}
