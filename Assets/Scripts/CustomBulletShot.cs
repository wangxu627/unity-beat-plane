using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("UniBulletHell/Custom Shot Pattern/CustomBulletShot")]
public class CustomBulletShot : UbhBaseShot
{
    // "Set a angle of shot. (0 to 360)"
    [Range(0f, 360f)]
    public float _Offset = 0f;

    public int _NWays = 4;
    // "Set a delay time between bullet and next bullet. (sec)"
    public float _BetweenDelay = 0.1f;

    protected override void Awake()
    {
        base.Awake();
    }

    public override void Shot()
    {
        StartCoroutine(ShotCoroutine());
    }

    private void LaunchSingle(float angle)
    {
        var bullet = GetBullet(transform.position, transform.rotation);
        ShotBullet(bullet, _BulletSpeed, angle);
        AutoReleaseBulletGameObject(bullet.gameObject);
    }

    IEnumerator ShotCoroutine()
    {
        if (_BulletNum <= 0 || _BulletSpeed <= 0f)
        {
            Debug.LogWarning("Cannot shot because BulletNum or BulletSpeed is not set.");
            yield break;
        }
        if (_Shooting)
        {
            yield break;
        }
        _Shooting = true;

        float step = 360 / _NWays;
        for (int i = 0; i < _BulletNum; i++)
        {
            if (0 < i && 0f < _BetweenDelay)
            {
                yield return StartCoroutine(UbhUtil.WaitForSeconds(_BetweenDelay));
            }

            for(int j = 0;j < _NWays;j++)
            {
                LaunchSingle(j * step + _Offset);
            }
        }

        FinishedShot();
    }


}
