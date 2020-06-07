using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using SunnyCat.Tools;
using Papae.UnitySDK.Managers;
using Random = UnityEngine.Random;

public class GameManager : Singleton<GameManager>
{
    public enum GameState
    {
        Menu,
        Intro,
        Playing,
        Paused,
        Failed,
        Win
    }

    struct Strategy
    {
        public GameObject[] bigEnemy;
    }

    [Header("Player Settings")]
    public PlayerController player;
    public int initialReviveCount = 1;

    [Header("Enemy Settings")]
    public GameObject littleEnemy;
    public GameObject bossEnemy;
    public GameObject[] bigEnemyL1;
    public GameObject[] bigEnemyL2;
    public GameObject[] bigEnemyL3;

    [Header("Level Settings")]
    public float introOffset = 100;
    public Transform introFrom;
    public Transform introTo;

    [Header("Audio Source")]
    public AudioClip menuClip;
    public AudioClip gameClip;

    [Header("Message")]
    public string[] messages = new string[]{
        "Fight alone until dawn comes",
        "Hell is not only one level",
        "Victory is not far from yuu",
        "Thanks for playing"
    };
    public string[] androidNames = new string[]{
        "Small Stubby",
        "Medium Biped",
        "Small Flyer",
        "Medium Flyer",
        "Multi-tier Type",
        "Small Biped",
        "Small Exploder",
        "Goliath Biped",
        "Reverse-jointed Goliath",
        "Goliath Biped: Enhanced Legpower",
        "Medium Exploder",
        "Linked-sphere Type",
        "Medium Quadruped",
        "Small Sphere",
        "Multi-leg Medium Model",
        "Rampaging Small Stubby",
        "Rampaging Small Biped",
        "Rampaging Medium Biped"
    };
    public string[] bossNames = new string[]{
        "Marx",
        "Adam",
        "Beauvoir",
        "Adam & Eve",
        "A2 Android",
        "Eve",
        "Engels",
        "EMP Generator",
        "Goliath Tank",
        "Goliath Flyer",
        "Grün",
        "So-Shi",
        "Boku-Shi",
        "Hegel",
        "Operator 21O",
        "Auguste",
        "Emil"
    };

    [Header("Debug Settings")]
    public bool notLaunchEnemy = false;
    public int LittleEnemiesCount {get;set;}
    public int BigEnemiesCount {get;set;}
    public GameState currentGameState {get;set;}
    public bool PausedLaunch {get;set;}
    public int CurrentMessageIndex {get;set;}
    public int CurrentReviveCount {get;private set;}
    
    private Strategy strategy = new Strategy();
    protected override void Awake()
    {
        base.Awake();
        Application.targetFrameRate = 60;
        DOTween.defaultTimeScaleIndependent = true;
        DOTween.Init();
        DOTween.defaultTimeScaleIndependent = true;

    }
    // Start is called before the first frame update
    void Start()
    {
        ChangeGameState(GameState.Menu);
    }
    public void ChangeGameState(GameState newState)
    {
        switch(newState)
        {
            case GameState.Menu:
                ChangeStateToMenu();
            break;
            case GameState.Intro:
                ChangeStateToIntro();
            break;
            case GameState.Playing:
                ChangeStateToPlaying();
            break;
            case GameState.Paused:
                ChangeStateToPaused();
            break;
            case GameState.Failed:
                ChangeStateToFailed();
            break;
            case GameState.Win:
                ChangeStateToWin();
            break;
        }
        currentGameState = newState;
    }
    private void ChangeStateToMenu()
    {
        Time.timeScale = 1;
        UIManager.Instance.OpenMenuGroup(true);
        UIManager.Instance.CloseGameGroup();
        InputManager.Instance.SetVirutalJoystickActive(false);
        player.gameObject.SetActive(false);
        ClearAllEnemies();
        ClearAllBullets();

        AudioManager.Instance.PlayBGM(menuClip, MusicTransition.LinearFade);
    }
    private void ChangeStateToIntro()
    {
        Vector2 positionIntroFrom = new Vector2(Screen.width / 2, -introOffset);
        Vector2 positionIntroTo = new Vector2(Screen.width / 2, introOffset);
        introFrom.position = Utils.ScreenPosition2WorldPosition(positionIntroFrom);
        introTo.position = Utils.ScreenPosition2WorldPosition(positionIntroTo);

        player.SetControlMode(PlayerController.ControlMode.AI);
        player.gameObject.SetActive(true);
        player.gameObject.transform.position = introFrom.position;

        CurrentReviveCount = initialReviveCount;

        var s = DOTween.Sequence();
        s.Append(player.gameObject.transform.DOMove(introTo.position, 1.0f));
        s.AppendCallback(()=>{
            player.SetControlMode(PlayerController.ControlMode.Player);

            CountdownController.Instance.Restart();
            UIManager.Instance.OpenGameGroupOnlyMessage();
            CurrentMessageIndex = 0;
            StartCoroutine(ShowMessage(() => {
                UpdateStrategy();
                ChangeGameState(GameState.Playing);
            }));
        });
        player.Initialization();
        
        AudioManager.Instance.PlayBGM(gameClip, MusicTransition.LinearFade);
    }
    private void ChangeStateToPlaying()
    {
        InputManager.Instance.SetVirutalJoystickActive(true);
        if(currentGameState == GameState.Intro)
        {
            LittleEnemiesCount = 0;
            BigEnemiesCount = 0;
            PausedLaunch = false;
            CountdownController.Instance.Restart();
            UIManager.Instance.OpenGameGroup();
            StartToLaunchEnemy();
        }
        player.Initialization();
        Time.timeScale = 1;
        CountdownController.Instance.Continue();
    }
    private void ChangeStateToPaused()
    {
        Time.timeScale = 0;
        CountdownController.Instance.Pause();
    }
    private void ChangeStateToFailed()
    {
        CountdownController.Instance.Pause();
        UIManager.Instance.OpenFailedPanel();
    }
    private void ChangeStateToWin()
    {
        ChangeGameState(GameState.Menu);
    }
    public void StartToLaunchEnemy()
    {
        if(notLaunchEnemy)
        {
            return;
        }

        InstantiateBatchLittleEnemy();
        InstantiateOneBigEnemy();
    }

    private void InstantiateBatchLittleEnemy()
    {
        int count = Random.Range(4, 7);
        for(int i = 0;i < count;i++)
        {
            string info = GetNextAndroidInfo<string>();

            GameObject enemyIns = Instantiate(littleEnemy);
            EnemyController ec = enemyIns.GetComponent<EnemyController>();
            ec.SetDisplayName(info);
            ec.UpdateBulletConfiguration();
            float x = Random.Range(0, Screen.width);
            float y = Screen.height + 100;
            enemyIns.transform.position = Utils.ScreenPosition2WorldPosition(new Vector2(x, y));
            Health health = enemyIns.GetComponent<Health>();
            if(health)
            {
                health.OnDeath += OnLittleEnemyDeath;
            }
            LittleEnemiesCount++;
        }

        // new logic
        if(Random.Range(0, 100) < 10)
        {
            InstantiateOneBigEnemy();
        }
    }

    private void InstantiateOneBigEnemy()
    {
        string info = GetNextBossInfo<string>();

        GameObject enemyIns = Instantiate(bossEnemy);
        EnemyController ec = enemyIns.GetComponent<EnemyController>();
        ec.enemyType = GetEnemyTypeByLevel();
        ec.SetDisplayName(info);
        ec.SetDisplayColor(CountdownController.Instance.CurrentColor);
        EnemyWeaponController ewc = enemyIns.GetComponent<EnemyWeaponController>();
        ewc.EquipWeapon();
        ec.UpdateBulletConfiguration();
        float x = Random.Range(0, Screen.width);
        float y = Screen.height + 100;
        enemyIns.transform.position = Utils.ScreenPosition2WorldPosition(new Vector2(x, y));
        Health health = enemyIns.GetComponent<Health>();
        if(health)
        {
            health.OnDeath += OnBigEnemyDeath;
        }
        BigEnemiesCount++;
    }

    private T GetNextAndroidInfo<T>()
    {
        int rndIdx = Random.Range(0, androidNames.Length);
        return (T)(object)androidNames[rndIdx];
    }

    private T GetNextBossInfo<T>()
    {
        int rndIdx = Random.Range(0, bossNames.Length);
        return (T)(object)bossNames[rndIdx];
    }

    private void OnLittleEnemyDeath()
    {
        LittleEnemiesCount--;
        if(LittleEnemiesCount == 0 && !PausedLaunch)
        {
            InstantiateBatchLittleEnemy();
        }

        if(PausedLaunch && (LittleEnemiesCount + BigEnemiesCount) == 0)
        {
            StartCoroutine(ShowMessage(NextRound));
        }
    }

    private void OnBigEnemyDeath()
    {
        BigEnemiesCount--;
        Debug.Log("==========>> OnBigEnemyDeath : " + BigEnemiesCount);
        if(BigEnemiesCount == 0 && !PausedLaunch)
        {
            InstantiateOneBigEnemy();
        }

        if(PausedLaunch && (LittleEnemiesCount + BigEnemiesCount) == 0)
        {
            StartCoroutine(ShowMessage(NextRound));
        }
    }
    private void ClearAllEnemies()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach(var e in enemies)
        {
            Destroy(e);
        }
    }
    private void ClearAllBullets()
    {
        // Debug.Log("!!!!!!!!!!!!!!!! : " + UbhObjectPool.Instance.GetActivePooledObjectCount());
        UbhObjectPool.Instance.DisableAllObjects();
    }
    public void OnRoundEnded()
    {
        PausedLaunch = true;
    }
    private IEnumerator ShowMessage(Action action)
    {
        player.SetControlMode(PlayerController.ControlMode.AI);
        UIManager.Instance.ShowMessage(messages[CurrentMessageIndex]);
        CurrentMessageIndex++;
        yield return new WaitForSeconds(3);
        UIManager.Instance.HideMessage();
        yield return new WaitForSeconds(1);
        player.SetControlMode(PlayerController.ControlMode.Player);
        
        if(CountdownController.Instance.HasNextRound())
        {
            action();
        }
        else
        {
            ChangeGameState(GameState.Win);
        }
    }

    private void NextRound()
    {
        PausedLaunch = false;
        CountdownController.Instance.NextRound();
        UpdateStrategy();

        if(!notLaunchEnemy)
        {
            InstantiateOneBigEnemy();
            InstantiateBatchLittleEnemy();
        }
    }
    private EnemyController.EnemyType GetEnemyTypeByLevel()
    {
        switch(CountdownController.Instance.CurrentRount)
        {
            case 0:
            return EnemyController.EnemyType.EnemyLevel1;
            case 1:
            return EnemyController.EnemyType.EnemyLevel2;
            case 2:
            return EnemyController.EnemyType.EnemyLevel3;
        }
        return EnemyController.EnemyType.EnemyLevel1; 
    }
    private void UpdateStrategy()
    {
        switch(CountdownController.Instance.CurrentRount)
        {
            case 0:
            strategy.bigEnemy = bigEnemyL1;
            break;
            case 1:
            strategy.bigEnemy = bigEnemyL2;
            break;
            case 2:
            strategy.bigEnemy = bigEnemyL3;
            break;
        }
    }

    public void Revive()
    {
        ChangeGameState(GameState.Playing);
        CurrentReviveCount = CurrentReviveCount - 1;
        player.Revive();
    }
}
