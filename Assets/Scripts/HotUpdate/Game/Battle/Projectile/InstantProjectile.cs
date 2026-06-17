using System.Collections;
using UnityEngine;

namespace HotUpdate.Game.Battle.Projectile
{
    /// <summary>
    /// 瞬时弹射物基类
    /// </summary>
    public abstract class InstantProjectile : Projectile
    {
        /// <summary>
        /// 播放特效
        /// 子类可覆盖
        /// </summary>
        /// <returns></returns>
        protected override IEnumerator ExecuteVFX()
        {
            AddStatusOnTrigger();
            HandleTiming();
            float nowTime = 0;
            var index = 0;
            while (particleSystem.IsAlive() && index < triggerTimes.Length)
            {
                nowTime += Time.deltaTime;
                if (nowTime >= triggerTimes[index])
                {
                    ApplyEffectOnTrigger();
                    CreateVFXOnTrigger();
                    index++;
                }
                yield return null;
            }
        }
    }
}
