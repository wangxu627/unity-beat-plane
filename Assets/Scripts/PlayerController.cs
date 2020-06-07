using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DarkTonic.CoreGameKit;
using Papae.UnitySDK.Managers;

public class PlayerController : MonoBehaviour
{
    public enum ControlMode
    {
        AI,
        Player
    } 
    public ControlMode controlMode = ControlMode.Player;
    public Transform playerDieParticlePrefab;
    public Transform playerHitParticlePrefab;
    public GameObject reviveParticle;
    private PlayerMoveController moveController;
    private PlayerRotationMouseController rotationMouseController;
    private PlayerRotationJoystickController rotationJoystickController;
    private PlayerSimpleFireController simpleFireController;
    private RestrainPositionInScreen restain;
    private Health health;

    void Awake() {
        moveController = GetComponent<PlayerMoveController>();
        rotationMouseController = GetComponent<PlayerRotationMouseController>();
        rotationJoystickController = GetComponent<PlayerRotationJoystickController>();
        simpleFireController = GetComponent<PlayerSimpleFireController>();
        restain = GetComponent<RestrainPositionInScreen>();
        
        health = GetComponent<Health>();
        if(health)
        {
            health.OnDeath += OnDeath;
            health.OnHit += OnHit;
        }
    }
    public void Initialization()
    {
        health.Revive();
        reviveParticle.SetActive(false);
        if(InputManager.Instance.controllerType == InputManager.ControllerType.Mouse)
        {
            rotationMouseController.ResetRotation();
        }
        else
        {
            rotationJoystickController.ResetRotation();
        }
    }

    public void Revive()
    {
        gameObject.SetActive(true);
        reviveParticle.SetActive(true);
        health.Revive();
        health.DamageDisabled();

        float immortalTime = 3.0f;
        StartCoroutine(health.DamageEnabled(immortalTime));
        StartCoroutine(Utils.DelayCall(() => {
            reviveParticle.SetActive(false);
        }, immortalTime));
    }

    public void OnHit()
    {
        // Instantiate(playerHitParticlePrefab, transform.position, Quaternion.identity);
        Transform trans = PoolBoss.SpawnInPool(playerHitParticlePrefab, transform.position, Quaternion.identity);
        trans.gameObject.GetComponent<AutoDestroy>().AutoDestroyMe();
    }

    public void OnDeath()
    {
        // Instantiate(playerDieParticlePrefab, transform.position, Quaternion.identity);
        Transform trans = PoolBoss.SpawnInPool(playerDieParticlePrefab, transform.position, Quaternion.identity);
        trans.gameObject.GetComponent<AutoDestroy>().AutoDestroyMe();

        gameObject.SetActive(false);
        GameManager.Instance.ChangeGameState(GameManager.GameState.Failed);

        AudioManager.Instance.PlayOneShot(AudioManager.Instance.GetClipFromPlaylist("Player Explosion"));
    }

    public void SetControlMode(ControlMode newMode)
    {
        controlMode = newMode;
        switch(controlMode)
        {
            case ControlMode.AI:
            ChangeToAIMode();
            break;
            case ControlMode.Player:
            ChangeToPlayerMode();
            break;
        }
    }

    private void ChangeToAIMode()
    {
        moveController.enabled = false;
        simpleFireController.enabled = false;
        restain.enabled = false;
        health.enabled = false;
        if(rotationMouseController)
        {
            rotationMouseController.enabled = false;
        }
        if(rotationJoystickController)
        {
            rotationJoystickController.enabled = false;
        }
    }

    private void ChangeToPlayerMode()
    {
        moveController.enabled = true;
        simpleFireController.enabled = true;
        restain.enabled = true;
        health.enabled = true;
        if(rotationMouseController)
        {
            rotationMouseController.enabled = true;
        }
        if(rotationJoystickController)
        {
            rotationJoystickController.enabled = true;
        }
    }
}
