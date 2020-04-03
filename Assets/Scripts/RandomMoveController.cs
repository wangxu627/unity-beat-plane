using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomMoveController : MonoBehaviour
{
    public float maxChangeDirTime = 1.0f;
    public float maxSpeed = 4.0f;

    private Vector3 currentMoveSpeed;
    private float currentChangeTime;
    // Start is called before the first frame update
    void Start()
    {
        this.GenerateRandomDir();
    }

    // Update is called once per frame
    void Update()
    {
        if(this.currentChangeTime > 0)
        {
            this.currentChangeTime -= Time.deltaTime;
            this.gameObject.transform.Translate(this.currentMoveSpeed * Time.deltaTime);
        } else
        {
            this.GenerateRandomDir();
        }
    }

    private void GenerateRandomDir()
    {
        this.currentChangeTime = this.maxChangeDirTime;
        this.currentMoveSpeed = new Vector3(Random.value - 0.5f, Random.value - 0.5f, 0.0f);
        this.currentMoveSpeed = this.currentMoveSpeed.normalized * this.maxSpeed;
    }
}
