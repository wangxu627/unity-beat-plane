using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletOffsetController : MonoBehaviour
{
    public float offsetSpeed = 0;
    private CustomBulletShot shot;
    // Start is called before the first frame update
    void Start()
    {
        shot = GetComponent<CustomBulletShot>();
    }

    // Update is called once per frame
    void Update()
    {
        shot._Offset += offsetSpeed * Time.deltaTime;
    }
}
