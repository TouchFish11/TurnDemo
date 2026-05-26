using Core.DI;
using Core.Pool;
using Core.Time;
using HotUpdate.Common.Generated;
using HotUpdate.Game.Battle.Projectile;
using HotUpdate.Game.Battle.Status;
using HotUpdate.Game.VFX;
using UnityEngine;

namespace HotUpdate.Game.Battle.Object.Role.Priest.Projectile
{
    /// <summary>
    /// 牧师终结技弹射物
    /// </summary>
    public class PriestUltimateProjectile : InstantProjectile
    {
        protected override void AddStatusOnTrigger()
        {
            foreach (var statusId in statusIds)
            {
                // 获取状态实例
                var status = statusFactory.GetStatus(projectileData.caster, projectileData.caster, statusId);
                // 添加状态
                projectileData.caster.GetComponent<StatusComponent>().AddStatus(status);
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
                // 恢复能量
                projectileData.skill.RecoverEnergy();
            }
        }
        
        protected override async void CreateVFXOnTrigger()
        {
            foreach (var target in projectileData.targets)
            {
                var projectileTrans = new ProjectileTrans(target.GameObject.transform.position, Quaternion.identity);
                var newVFXInfo = DIContainer.GetInstance<IPoolManager>().GetData<VFXInfo>();
                await DIContainer.GetInstance<IVFXManager>().CreateVFX(AssetKeys.VFX_WindPropertySkill_Hit, projectileTrans, default, newVFXInfo);
                // 计时器计时
                DIContainer.GetInstance<ITimerManager>().CreateTimer(false, 500, () =>
                {
                    newVFXInfo.IsStop = true;
                });
            }
        }

        protected override void HandleTiming()
        {
            DIContainer.GetInstance<ITimerManager>().CreateTimer(false, 1600, () =>
            {
                vFXInfo.IsStop = true;
            });
        }
    }
}
