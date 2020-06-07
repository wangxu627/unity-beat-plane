using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DarkTonic.CoreGameKit;

public class HitEffectController : MonoBehaviour
{
    public GameObject hit;
    // Start is called before the first frame update
    public void PlayHit(Vector3? position = null)
    {
        // GameObject particle = Instantiate(hit, transform.position, Quaternion.identity);
        Vector3 realPosition = transform.position;
        if(position.HasValue) {
            realPosition = position.Value;
        }
        Transform particle = PoolBoss.SpawnInPool(hit.transform, realPosition, Quaternion.identity);
        particle.transform.localScale = Vector3.one * 0.3f;
        particle.transform.gameObject.GetComponent<AutoDestroy>().AutoDestroyMe();
    }
}
