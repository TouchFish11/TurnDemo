using Framework;
using System.Collections;
using UnityEngine;

public class MonsterDeadProjectile : InstantProjectile
{
    private float destroyTime = 1.01f;

    protected override void Trigger()
    {
        // ÏÈÒÆ³ýÌØÐ§
        vFXInfo.IsStop = true;
    }

    protected override IEnumerator PlayingVFX()
    {
        float nowTime = 0;
        while (particleSystem.IsAlive())
        {
            nowTime += Time.deltaTime;
            if (nowTime >= destroyTime)
            {
                Trigger();
            }
            yield return null;
        }
    }
}
