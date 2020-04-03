using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float moveSpeed = 30.0f;
    
    // Update is called once per frame
    void Update()
    {
        this.transform.Translate(this.transform.forward * this.moveSpeed * Time.deltaTime, Space.World);
    }
}
