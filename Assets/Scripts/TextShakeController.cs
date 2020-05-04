using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextShakeController : MonoBehaviour
{
    public float shakeDuration = 0.02f;
    public float shakeOffset = 0.05f;

    public bool isShaking = false;
    
    public void Shake()
    {
        if(isShaking)
        {
            return;
        }

        StartCoroutine(ShakeCoroutine());
    }

    IEnumerator ShakeCoroutine()
    {
        isShaking = true;
        Vector3 offset = new Vector3(shakeOffset, shakeOffset, shakeOffset);
        offset = Random.rotation * offset;
        transform.AddPosition(offset.x, offset.y, offset.z);
        yield return new WaitForSeconds(shakeDuration);
        transform.AddPosition(-offset.x, -offset.y, -offset.z);
        isShaking = false;
    }
}
