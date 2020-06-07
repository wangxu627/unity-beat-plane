using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyController))]
public class EnemyWeaponController : MonoBehaviour
{
    public Transform weaponHandle;
    public GameObject[] weaponsLevel1;
    public GameObject[] weaponsLevel2;
    public GameObject[] weaponsLevel3;

    private EnemyController enemyController;
    
    void Start()
    {
        enemyController = GetComponent<EnemyController>();
    }
    public void EquipWeapon()
    {
        if(enemyController == null)
        {
            enemyController = GetComponent<EnemyController>();
        }
        GameObject gameObject = null;
        if(enemyController.enemyType == EnemyController.EnemyType.EnemyLevel1)
        {
            gameObject = Instantiate(weaponsLevel1[Random.Range(0, weaponsLevel1.Length)]);
        }
        else if(enemyController.enemyType == EnemyController.EnemyType.EnemyLevel2)
        {
            gameObject = Instantiate(weaponsLevel2[Random.Range(0, weaponsLevel2.Length)]);
        }
        else if(enemyController.enemyType == EnemyController.EnemyType.EnemyLevel3)
        {
            gameObject = Instantiate(weaponsLevel3[Random.Range(0, weaponsLevel3.Length)]);
        }
        gameObject.transform.parent = weaponHandle;
    }
}
