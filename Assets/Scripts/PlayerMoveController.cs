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
        float xInput = Input.GetAxis("Horizontal");
        float yInput = Input.GetAxis("Vertical");

        float xMovement = xInput * this.moveSpeed * Time.deltaTime;
        float yMovement = yInput * this.moveSpeed * Time.deltaTime;
        Vector3 newPosition = new Vector3(this.transform.position.x + xMovement,
                                        this.transform.position.y + yMovement,
                                        this.transform.position.z);
        this.transform.position = this.restain.GetRestrainedPosition(newPosition);
        //this.transform.Translate(new Vector3(xInput, yInput, 0) * this.moveSpeed * Time.deltaTime, Space.World);
    }
}
