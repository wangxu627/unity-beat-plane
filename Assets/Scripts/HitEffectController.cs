using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitEffectController : MonoBehaviour
{
    public GameObject hit;
    // Start is called before the first frame update
    public void PlayHit()
    {
        //this.particleSystem.Play();
        GameObject particle = Instantiate(this.hit, this.transform.position, Quaternion.identity);
        particle.transform.localScale = Vector3.one * 0.3f;
    }
}
