using System.Collections;
using UnityEngine;

namespace GameHotUpdate.Battle.Projectile
{
    /// <summary>
    /// 轨迹弹射物基类
    /// </summary>
    public abstract class TrajectProjectile : Projectile
    {
        protected override IEnumerator PlayingVFX()
        {
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
            AddStatusOnTrigger();
            HandleTiming();
        }
    }
}
