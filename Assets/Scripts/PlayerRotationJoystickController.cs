using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerMoveController))]
public class PlayerRotationJoystickController : MonoBehaviour
{
    public float rotationSpeed = 15.0f;
    
    void Start() {
        if(InputManager.Instance.controllerType != InputManager.ControllerType.Joystick &&
           InputManager.Instance.controllerType != InputManager.ControllerType.VirtualJoystick)
        {
            Destroy(this);
        }
    }
    // Update is called once per frame
    void Update()
    {
        float xInput = 0, yInput = 0; 
        if(InputManager.Instance.controllerType == InputManager.ControllerType.Joystick)
        {
            xInput = Input.GetAxis("RightHorizontal");
            yInput = Input.GetAxis("RightVertical");
        }
        else if(InputManager.Instance.controllerType == InputManager.ControllerType.VirtualJoystick)
        {
            xInput = ETCInput.GetAxis("RightHorizontal");
		    yInput = ETCInput.GetAxis("RightVertical");
        }
        Debug.Log("xxxxxxx xxxxxxxxxxxxxxxxxxx: " + xInput + "   :   " + yInput + "         " + InputManager.Instance.controllerType);
        if(xInput != 0 || yInput != 0)
        {
            Debug.Log("=================xxxxxxx : " + xInput + "   :   " + yInput);
            Rotate(xInput, yInput);
        }
    }

    private void Rotate(float x, float y)
    {
        Vector3 dir = new Vector3(x, -y, 0);
        dir = dir.normalized * 10;
        Quaternion toRotation = Quaternion.LookRotation(new Vector3(dir.x, dir.y, 0), Vector3.back);
        this.transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, this.rotationSpeed * Time.deltaTime);
    }
}
