using System.Collections;
using UnityEngine;

namespace GameHotUpdate.Battle.Projectile
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
        protected override IEnumerator PlayingVFX()
        {
            AddStatusOnTrigger();
            float nowTime = 0;
            var index = 0;
            while (particleSystem.IsAlive() && index < triggerTimes.Length)
            {
                nowTime += Time.deltaTime;
                if (nowTime >= triggerTimes[index])
                {
                    CauseDamageOnTrigger();
                    CreateVFXOnTrigger();
                    index++;
                }
                yield return null;
            }
            HandleOtherOnTrigger();
        }
    }
}
