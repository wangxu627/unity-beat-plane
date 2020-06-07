using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DarkTonic.CoreGameKit;

public class AutoDestroy : MonoBehaviour
{
    public float time = 3;
    public bool pooled = false;
    // Start is called before the first frame update
    public void AutoDestroyMe()
    {
        StartCoroutine(DestroyMe());
    }

    private IEnumerator DestroyMe()
    {
        yield return new WaitForSeconds(time);
        if(pooled)
        {
            PoolBoss.Despawn(this.gameObject.transform);
        }
        else
        {
            // Destroy(this.gameObject, this.time);
            Debug.Assert(false);
        }
    }
}
