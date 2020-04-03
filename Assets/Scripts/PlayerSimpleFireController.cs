using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSimpleFireController : MonoBehaviour
{
    public enum FireMode
    {
        Click,
        Hold
    };

    public FireMode fireMode = FireMode.Hold;
    public GameObject bullet;
    public float fireSpeed = 0.5f;

    private float toFireLeft;
    private Transform firePoint;
    void Start()
    {
        this.firePoint = this.transform.Find("FirePoint");
        this.toFireLeft = 0;
    }
    // Update is called once per frame
    void Update()
    {
        if(this.fireMode == FireMode.Hold)
        {
            this.toFireLeft -= Time.deltaTime;
            if (Input.GetButton("Fire1") && this.toFireLeft < 0)
            {
                Fire();
                this.toFireLeft = this.fireSpeed;
            }
        } 
        else if(this.fireMode == FireMode.Click)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                Fire();
            }
        }
        
    }

    private void Fire()
    {
        Vector3 rotation = this.transform.forward;
        GameObject obj = Instantiate(this.bullet, this.firePoint.position, Quaternion.identity);
        obj.transform.forward = this.transform.forward;
        obj.transform.localRotation = Quaternion.Euler(this.transform.localRotation.eulerAngles.x, this.transform.localRotation.eulerAngles.y, 90);
        //obj.transform.Rotate(obj.transform.forward, 90);
        //obj.transform.up = this.transform.up;
        //obj.transform.localRotation = Quaternion.Euler(rotation.x, rotation.y, rotation.z);
    }
}
