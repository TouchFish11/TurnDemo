using Core.DI;
using Core.Pool;
using Core.Time;
using HotUpdate.Game.Battle.Projectile;
using HotUpdate.Game.Battle.Status;
using HotUpdate.Game.VFX;
using UnityEngine;

namespace HotUpdate.Game.Battle.Object.Monster.AbyssalMage.Skill.Frostfall
{
    /// <summary>
    /// 霜陨技能弹射物
    /// </summary>
    public class FrostfallSkillProjectile : InstantProjectile
    {
        protected override void AddStatusOnTrigger()
        {
            foreach (var target in projectileData.targets)
            {
                foreach (var statusId in statusIds)
                {
                    // 获取状态实例
                    var status = statusFactory.GetStatus(projectileData.caster, target, statusId);
                    // 添加状态
                    target.GetComponent<StatusComponent>().AddStatus(status);
                } 
            }
        }

        protected override void ApplyEffectOnTrigger()
        {
            foreach (var target in projectileData.targets)
            {
                damageCalcManager.CalcSkillDamage(
                    projectileData.caster, 
                    target, projectileData.skill.SkillInfo, 
                    out var result);
                target.TakeDamage(result);
            }
        }

        protected override async void CreateVFXOnTrigger()
        {
            foreach (var target in projectileData.targets)
            {
                var projectileTrans = new ProjectileTrans(target.GameObject.transform.position, Quaternion.identity);
                var newVFXInfo = DIContainer.GetInstance<IPoolManager>().GetData<VFXInfo>();
                await DIContainer.GetInstance<IVFXManager>().CreateVFX(AssetKeys.VFX_IcePropertySkill_Hit, projectileTrans, default, newVFXInfo);
                DIContainer.GetInstance<ITimerManager>().CreateTimer(false, 500, () =>
                {
                    newVFXInfo.IsStop = true;
                });
            }
        }

        protected override void HandleTiming()
        {
            // 计时器计时
            DIContainer.GetInstance<ITimerManager>().CreateTimer(false, 3200, () =>
            {
                vFXInfo.IsStop = true;
            });
        }
    }
}
