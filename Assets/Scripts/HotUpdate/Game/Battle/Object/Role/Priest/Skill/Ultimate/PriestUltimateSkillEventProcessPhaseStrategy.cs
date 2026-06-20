using System.Threading.Tasks;
using Core.DI;
using Core.Pool;
using HotUpdate.Game.Battle.Skill.Base;
using HotUpdate.Game.Battle.Skill.Base.Flow;
using HotUpdate.Game.Battle.Status;
using HotUpdate.Game.VFX;
using UnityEngine;

namespace HotUpdate.Game.Battle.Object.Role.Priest.Skill.Ultimate
{
    public class PriestUltimateSkillEventProcessPhaseStrategy : SkillEventProcessPhaseStrategy
    {
        protected override async Task OnTrigger(HitResult hitResult)
        {
            var projectileData = SkillContext.ProjectileData;
            foreach (var statusId in SkillContext.StatusIds)
            {
                // 获取状态实例
                var status = statusFactory.GetStatus(projectileData.caster, projectileData.caster, statusId);
                // 添加状态
                projectileData.caster.GetComponent<StatusComponent>().AddStatus(status);
            }
            
            foreach (var target in projectileData.targets)
            {
                damageCalcManager.CalcSkillDamage(
                    projectileData.caster, 
                    target, SkillContext.SkillInfo, 
                    out var result);
                target.TakeDamage(result);
                // 恢复能量
                // ((PlayerSkill)projectileData.skill).RecoverEnergy();
            }
            
            foreach (var target in projectileData.targets)
            {
                var projectileTrans = new ProjectileTrans(target.GameObject.transform.position, Quaternion.identity);
                var newVFXInfo = DIContainer.GetInstance<IPoolManager>().GetData<VFXInfo>();
                await DIContainer.GetInstance<IVFXManager>().CreateVFX(AssetKeys.VFX_WindPropertySkill_Hit, projectileTrans, default, newVFXInfo);
                // 计时器计时
                timerManager.CreateTimer(false, 500, () =>
                {
                    newVFXInfo.IsStop = true;
                });
            }
            
            if (hitResult.IsFirstHit)
            {
                timerManager.CreateTimer(false, 1600, () =>
                {
                    SkillContext.VFXInfo.IsStop = true;
                });
            }
        }
    }
}
