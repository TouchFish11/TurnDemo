using System.Collections;
using GameHotUpdate.Battle.Projectile;
using UnityEngine;

namespace GameHotUpdate.Battle.Object.Monster.Slime.Projectile
{
    /// <summary>
    /// TODO：更像特效
    /// 怪物死亡弹射物
    /// </summary>
    public class MonsterDeadProjectile : InstantProjectile
    {
        private const float destroyTime = 1.01f;

        protected override IEnumerator PlayingVFX()
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

        protected override void AddStatusOnTrigger()
        {
            
        }
        
        protected override void CauseDamageOnTrigger()
        {

        }

        protected override void CreateVFXOnTrigger()
        {
            
        }

        protected override void HandleOtherOnTrigger()
        {
            
        }
    }
}
