using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLifeController : MonoBehaviour
{
    public int lifeCount;
    public GameObject dieParticle;
    public GameObject playerHitPrefab;


    public void DecreaseLife(int amount)
    {
        lifeCount -= amount;
        if(lifeCount <= 0)
        {
            Instantiate(dieParticle, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
        else
        {
            Instantiate(playerHitPrefab, transform.position, Quaternion.identity);
        }
    }
}
