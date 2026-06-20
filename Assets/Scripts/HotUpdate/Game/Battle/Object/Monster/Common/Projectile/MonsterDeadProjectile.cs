using System.Collections;
using HotUpdate.Game.Battle.Projectile;
using UnityEngine;

namespace HotUpdate.Game.Battle.Object.Monster.Common.Projectile
{
    /// <summary>
    /// 怪物死亡弹射物
    /// </summary>
    public class MonsterDeadProjectile : InstantProjectile
    {
        private const float destroyTime = 1.01f;

        protected override IEnumerator ExecuteVFX()
        {
            float nowTime = 0;
            while (particleSystem.IsAlive())
            {
                nowTime += Time.deltaTime;
                if (nowTime >= destroyTime)
                {
                    vFXInfo.IsStop = true;
                    break;
                }
                yield return null;
            }
        }
    }
}
