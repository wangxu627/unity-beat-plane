using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcceleratedBulletController : MonoBehaviour
{
    public float finalMoveSpeed = 30.0f;
    public float initMoveSpeed = 0.0f;
    public float accelerateTime = 1.0f;

    private float currMoveSpeed;
    private float timeElasped;
    // Start is called before the first frame update
    void Start()
    {
        this.currMoveSpeed = this.initMoveSpeed;
        this.timeElasped = 0;
    }

    // Update is called once per frame
    void Update()
    {
        this.timeElasped += Time.deltaTime;
        this.currMoveSpeed = Mathf.Lerp(this.currMoveSpeed, this.finalMoveSpeed, this.timeElasped / this.accelerateTime);
        this.transform.Translate(this.transform.forward * this.currMoveSpeed * Time.deltaTime, Space.World);
    }
}
