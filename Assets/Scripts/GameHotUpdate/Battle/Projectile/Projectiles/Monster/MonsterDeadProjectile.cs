using System.Collections;
using UnityEngine;

namespace GameHotUpdate.Battle.Projectile.Projectiles.Monster
{
    public class MonsterDeadProjectile : InstantProjectile
    {
        private float destroyTime = 1.01f;

        protected override void Trigger()
        {
            // ���Ƴ���Ч
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
}
