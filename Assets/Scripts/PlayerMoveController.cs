using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveController : MonoBehaviour
{
    public float moveSpeed = 10;

    private RestrainPositionInScreen restain;
    // Start is called before the first frame update
    void Start()
    {
        this.restain = GetComponent<RestrainPositionInScreen>();
    }
    // Update is called once per frame
    void Update()
    {
        float xInput = 0, yInput = 0;
        if(InputManager.Instance.controllerType == InputManager.ControllerType.Mouse || 
           InputManager.Instance.controllerType == InputManager.ControllerType.Joystick)
        {
            xInput = Input.GetAxis("Horizontal");
            yInput = Input.GetAxis("Vertical");
        }
        else if(InputManager.Instance.controllerType == InputManager.ControllerType.VirtualJoystick)
        {
            xInput = ETCInput.GetAxis("Horizontal");
		    yInput = ETCInput.GetAxis("Vertical");
        }
        Move(xInput, yInput);
    }

    private void Move(float x, float y)
    {
        float xMovement = x * this.moveSpeed * Time.deltaTime;
        float yMovement = y * this.moveSpeed * Time.deltaTime;
        Vector3 newPosition = new Vector3(this.transform.position.x + xMovement,
                                        this.transform.position.y + yMovement,
                                        this.transform.position.z);
        if(this.restain.enabled)
        {
            this.transform.position = this.restain.GetRestrainedPosition(newPosition);
        }
        else
        {
            this.transform.position = newPosition;
        }
    }
}
