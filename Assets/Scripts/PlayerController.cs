using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public enum ControlMode
    {
        AI,
        Player
    } 
    public ControlMode controlMode = ControlMode.Player;
    public GameObject playerDieParticlePrefab;
    public GameObject playerHitParticlePrefab;
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
        rotationMouseController.ResetRotation();
    }
    public void OnHit()
    {
        Instantiate(playerHitParticlePrefab, transform.position, Quaternion.identity);
    }

    public void OnDeath()
    {
        Instantiate(playerDieParticlePrefab, transform.position, Quaternion.identity);
        gameObject.SetActive(false);

        GameManager.Instance.ChangeGameState(GameManager.GameState.Failed);
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
        rotationMouseController.enabled = false;
        // rotationJoystickController.enabled = false;
        simpleFireController.enabled = false;
        restain.enabled = false;
        health.enabled = false;
    }

    private void ChangeToPlayerMode()
    {
        moveController.enabled = true;
        rotationMouseController.enabled = true;
        // rotationJoystickController.enabled = true;
        simpleFireController.enabled = true;
        restain.enabled = true;
        health.enabled = true;
    }
}
