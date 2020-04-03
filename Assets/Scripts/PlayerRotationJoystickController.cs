using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRotationJoystickController : MonoBehaviour
{
    public float rotationSpeed = 15.0f;

    // Update is called once per frame
    void Update()
    {
        float xInput = Input.GetAxis("RightHorizontal");
        float yInput = Input.GetAxis("RightVertical");
        if(xInput != 0 || yInput != 0)
        {
            Vector3 dir = new Vector3(xInput, -yInput, 0);
            dir = dir.normalized * 10;
            Quaternion toRotation = Quaternion.LookRotation(new Vector3(dir.x, dir.y, 0), Vector3.back);
            this.transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, this.rotationSpeed * Time.deltaTime);
        }

    }
}
