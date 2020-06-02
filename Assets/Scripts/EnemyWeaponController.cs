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
        Debug.Log("KKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKK");
        Debug.Log(enemyController);
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
            gameObject = Instantiate(weaponsLevel1[0]);
        }
        else if(enemyController.enemyType == EnemyController.EnemyType.EnemyLevel2)
        {
            gameObject = Instantiate(weaponsLevel2[0]);
        }
        else if(enemyController.enemyType == EnemyController.EnemyType.EnemyLevel3)
        {
            gameObject = Instantiate(weaponsLevel3[0]);
        }
        gameObject.transform.parent = weaponHandle;
    }
}
