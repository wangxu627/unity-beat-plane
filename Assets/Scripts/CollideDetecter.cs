using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollideDetecter : MonoBehaviour
{
    // Start is called before the first frame update
    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collide!!!!!!!!!!!!!!!");
    }

    void OnTriggerEnter(Collider other)
    {
        switch(this.gameObject.tag)
        {
            case "Enemy":
                this.HandleEnemyObjectCollision(other);
                break;
            case "EnemyBullet":
                this.HandleEnemyBulletCollision(other);
                break;
        }
    }

    private void HandleEnemyObjectCollision(Collider other)
    {
        if(other.tag == "PlayerBullet")
        {
            GameObject realGameObject = other.gameObject.transform.parent.gameObject;
            Destroy(realGameObject);
        }
        this.gameObject.GetComponent<HitEffectController>().PlayHit();
        Debug.Log("Descrease enemy object life");
    }

    private void HandleEnemyBulletCollision(Collider other)
    {
        if (other.tag == "PlayerBullet")
        {
            GameObject realGameObject = other.gameObject.transform.parent.gameObject;
            Destroy(realGameObject);
        }
        this.gameObject.GetComponent<HitEffectController>().PlayHit();
        Destroy(this.gameObject);
    }
}
