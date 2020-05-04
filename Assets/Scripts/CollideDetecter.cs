using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollideDetecter : MonoBehaviour
{
    private TextShakeController textShakeController;

    void Start()
    {
        textShakeController = GetComponent<TextShakeController>();
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collide!!!!!!!!!!!!!!!");
    }

    void OnTriggerEnter(Collider other)
    {
        // Debug.Log("===========>>> : " + other.tag);
        switch(this.gameObject.tag)
        {
            case "Enemy":
                this.HandleEnemyObjectCollision(other);
                break;
            case "EnemyBullet":
                this.HandleEnemyBulletCollision(other);
                break;
            case "EnemyBulletHurt":
                this.HandleEnemyBulletHurtCollision(other);
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
        else if(other.tag == "Player")
        {
            other.gameObject.GetComponent<PlayerController>().Hit();
        }
        this.gameObject.GetComponent<HitEffectController>().PlayHit();
        if(textShakeController) 
        {
            textShakeController.Shake();
        }
        // Debug.Log("Descrease enemy object life");
    }

    private void HandleEnemyBulletCollision(Collider other)
    {
        if (other.tag == "PlayerBullet")
        {
            GameObject realGameObject = other.gameObject.transform.parent.gameObject;
            Destroy(realGameObject);
        }
        else if(other.tag == "Player")
        {
            other.gameObject.GetComponent<PlayerController>().Hit();
        }
        RemoveSelf();
    }

    private void HandleEnemyBulletHurtCollision(Collider other)
    {
        if(other.tag == "Player")
        {
            other.gameObject.GetComponent<PlayerController>().Hit();
            RemoveSelf();
        }
        else if(other.tag == "BulletClear")
        {
            RemoveSelf();
        }
    }

    private void RemoveSelf()
    {
        this.gameObject.GetComponent<HitEffectController>().PlayHit();
        Destroy(this.gameObject);
    }
}
