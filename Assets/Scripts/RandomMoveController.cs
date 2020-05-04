using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class RandomMoveController : MonoBehaviour
{
    public enum EnemyMoveType
    {
        EnemyLittle,
        EnemyBig
    };
    public EnemyMoveType enemyMoveType;
    public enum MoveState 
    {
        Entering,
        RandomMoving,
    };
    public float enteringTime = 2.0f;
    [Range(0.1f, 5.0f)]
    public float maxChangeDirTime = 1.0f;
    public float minMoveDistance = 0.0f;
    public float maxMoveDistance = 0.0f;
    public MoveState moveState = MoveState.Entering;
    public ParticleSystem showParticle;
    // private EnemyController enemyController;
    // Start is called before the first frame update
    void Start()
    {
        // enemyController = GetComponent<EnemyController>();
        Move();
    }

    private void Move()
    {
        if(moveState == MoveState.Entering) 
        {
            NewEnteringMovement();
        } 
        else if(moveState == MoveState.RandomMoving)
        {
            NewRandomMovement();
        }
    }
    private void NewEnteringMovement()
    {
        Vector2 position = GetRandomScreenPosition();
        // position.y = Mathf.Max(position.y, Screen.height - 100);
        StartCoroutine(EnteringMove(position.x, position.y));
    }

    private IEnumerator EnteringMove(float x, float y)
    {
        Vector3 position = Utils.ScreenPosition2WorldPosition(new Vector2(x, y));
        transform.position = new Vector3(position.x, transform.position.y, transform.position.z);
        Tween moveTween = transform.DOMove(position, enteringTime);
        yield return moveTween.WaitForCompletion();
        moveState = MoveState.RandomMoving;
        showParticle.Play();
        if(enemyMoveType == EnemyMoveType.EnemyLittle)
        {
            position = GetRandomScreenPositionBasedAngle();
        }
        else if(enemyMoveType == EnemyMoveType.EnemyBig)
        {
            position = GetRandomScreenPosition();
            position.x = x;
            position.y = Screen.height / 2;
        }
        position = Utils.ScreenPosition2WorldPosition(position);
        moveTween = transform.DOMove(position, 1.0f);
        yield return new WaitForSeconds(1.0f);
        Move();
    }

    private void NewRandomMovement()
    {
        Vector2 position = GetRandomScreenPositionBasedAngle();
        StartCoroutine(RandomMove(position.x, position.y));
    }

    private IEnumerator RandomMove(float x, float y)
    {
        float duration = Random.Range(0.5f, maxChangeDirTime);
        Vector3 position = Utils.ScreenPosition2WorldPosition(new Vector2(x, y));
        if(enemyMoveType == EnemyMoveType.EnemyLittle)
        {
            Vector3 offset = (position - transform.position);
            offset.x = Random.Range(-offset.x, offset.x);
            offset.y = Random.Range(0, offset.y);
            Vector3 middlePosition = transform.position + offset;
            Tween moveTween = transform.DOPath(new Vector3[] { middlePosition, position }, duration, PathType.CatmullRom);
            yield return new WaitForSeconds(duration * Random.Range(0.0f, 1.0f));
        }
        else if(enemyMoveType == EnemyMoveType.EnemyBig)
        {
            Tween moveTween = transform.DOMove(position, duration);
            yield return new WaitForSeconds(duration);
        }
        NewRandomMovement();
    }

    private Vector2 GetRandomScreenPosition()
    {
        int w = Random.Range(0, Screen.width);
        int h = Random.Range(Screen.height / 2, Screen.height - 100);
        return new Vector2(w, h);
    }

    private Vector2 GetRandomScreenPositionBasedAngle()
    {
        Vector2 nextScreenPosition;
        do
        {
            Vector2 currentScreenPosition = Utils.WorldPosition2ScreenPosition(transform.position);
            float angle = Random.Range(0, 360.0f);
            float moveDistance = Random.Range(minMoveDistance, maxMoveDistance);
            float xOffset = Mathf.Cos(angle) * moveDistance;
            float yOffset = Mathf.Sin(angle) * moveDistance;
            if(enemyMoveType == EnemyMoveType.EnemyBig)
            {
                yOffset = 0;
            }
            nextScreenPosition = currentScreenPosition + new Vector2(xOffset, yOffset);
        } while (nextScreenPosition.x < 0 ||
                nextScreenPosition.x > Screen.width ||
                nextScreenPosition.y < Screen.height / 2 ||
                nextScreenPosition.y > Screen.height);
        return nextScreenPosition;
    }
}
