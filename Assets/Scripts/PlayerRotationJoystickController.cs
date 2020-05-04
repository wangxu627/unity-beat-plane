using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerMoveController))]
public class PlayerRotationJoystickController : MonoBehaviour
{
    public float rotationSpeed = 15.0f;
    private PlayerMoveController.ControllerType controllerType;
    void Start() {
        PlayerMoveController c = GetComponent<PlayerMoveController>();
        controllerType = c.controllerType;
        if(controllerType != PlayerMoveController.ControllerType.Joystick &&
           controllerType != PlayerMoveController.ControllerType.VirtualJoystick)
        {
            Destroy(this);
        }
    }
    // Update is called once per frame
    void Update()
    {
        float xInput = 0, yInput = 0; 
        if(controllerType == PlayerMoveController.ControllerType.Joystick)
        {
            xInput = Input.GetAxis("RightHorizontal");
            yInput = Input.GetAxis("RightVertical");
        }
        else if(controllerType == PlayerMoveController.ControllerType.VirtualJoystick)
        {
            xInput = ETCInput.GetAxis("RightHorizontal");
		    yInput = ETCInput.GetAxis("RightVertical");
        }
        Debug.Log("xxxxxxx xxxxxxxxxxxxxxxxxxx: " + xInput + "   :   " + yInput + "         " + controllerType);
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
