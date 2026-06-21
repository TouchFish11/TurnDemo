using System.Collections;
using HotUpdate.Game.Battle.Skill.Base;
using UnityEngine;

namespace HotUpdate.Game.Battle.Projectile
{
    /// <summary>
    /// 轨迹弹射物基类
    /// </summary>
    public abstract class TrajectProjectile : Projectile
    {
        protected override IEnumerator ExecuteVFX()
        {
            float nowTime = 0;
            var index = 0;
            while (particleSystem.IsAlive() && index < triggerTimes.Length)
            {
                nowTime += Time.deltaTime;
                if (nowTime >= triggerTimes[index])
                {
                    InvokeOnTrigger(new HitResult(index == 0, index));
                    index++;
                }
                yield return null;
            }
        }
    }
}
