using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameObject littleEnemy;
    public GameObject bigEnemyL1;
    // Start is called before the first frame update
    void Start()
    {
        InstantiateBatchLittleEnemy(5);
        InstantiateOneBigEnemy();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void InstantiateBatchLittleEnemy(int count)
    {
        for(int i = 0;i < count;i++)
        {
            GameObject enemyIns = Instantiate(littleEnemy);
            EnemyController c = enemyIns.GetComponent<EnemyController>();
            c.SetDisplayName("111111111");
            float x = Random.Range(0, Screen.width);
            float y = Screen.height + 100;
            enemyIns.transform.position = Utils.ScreenPosition2WorldPosition(new Vector2(x, y));
        }
    }

    private void InstantiateOneBigEnemy()
    {
        GameObject enemyIns = Instantiate(bigEnemyL1);
        enemyIns.GetComponent<EnemyController>().SetDisplayName("111111111");
        float x = Random.Range(0, Screen.width);
        float y = Screen.height + 100;
        enemyIns.transform.position = Utils.ScreenPosition2WorldPosition(new Vector2(x, y));
    }
}
