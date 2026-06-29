using System.Threading.Tasks;
using HotUpdate.Game.Battle.Skill.Base;
using HotUpdate.Game.Battle.Skill.Base.Flow;
using HotUpdate.Game.Battle.Statuses;
using HotUpdate.Game.VFX;
using UnityEngine;

namespace HotUpdate.Game.Battle.Object.Role.Wizard.Skill.Normal
{
    public class WizardNormalSkillEventProcessPhaseStrategy : SkillEventProcessPhaseStrategy
    {
        protected override async Task OnTrigger(HitResult hitResult)
        {
            var projectileData = SkillContext.ProjectileData;
            foreach (var target in projectileData.targets)
            {
                foreach (var statusId in SkillContext.StatusIds)
                {
                    // 获取状态实例
                    var status = statusFactory.GetStatus(projectileData.caster, target, statusId);
                    // 添加状态
                    target.GetComponent<StatusComponent>().AddStatus(status);
                }
            }
            
            foreach (var target in projectileData.targets)
            {
                damageCalcManager.CalcSkillDamage(
                    projectileData.caster, 
                    target, SkillContext.SkillInfo, 
                    out var result);
                target.TakeDamage(result);
                // 恢复终结技能量
                ((PlayerObject)projectileData.caster).RecoverUltimate(SkillContext.SkillInfo.f_recoveryEnergy);
            }
            
            foreach (var target in projectileData.targets)
            {
                var projectileTrans = new ProjectileTrans(target.GameObject.transform.position, Quaternion.identity);
                var newVFXInfo = poolManager.GetData<VFXInfo>();
                await vfxManager.CreateVFX(AssetKeys.VFX_IcePropertySkill_Hit, projectileTrans, default, newVFXInfo);
                // 计时器计时
                timerManager.CreateTimer(false, 500, () =>
                {
                    newVFXInfo.IsStop = true;
                });
            }
        }
    }
}
