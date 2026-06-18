using Core.DI;
using Core.Pool;
using HotUpdate.Game.Battle.Skill.Base;
using HotUpdate.Game.Battle.Status;
using HotUpdate.Game.VFX;
using UnityEngine;

namespace HotUpdate.Game.Battle.Object.Role.Priest.Strategys
{
    public class PriestProjectileEventProcessStrategy : ProjectileEventProcessStrategy
    {
        public async void PriestNormalSkillEvent(SkillContext skillContext, HitResult hitResult)
        {
            var projectileData = skillContext.ProjectileData;
            foreach (var statusId in skillContext.StatusIds)
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
                    target, skillContext.SkillInfo, 
                    out var result);
                target.TakeDamage(result);
                // 恢复能量
                // ((PlayerSkill)projectileData.skill).RecoverEnergy();
            }
            
            foreach (var target in projectileData.targets)
            {
                var projectileTrans = new ProjectileTrans(target.GameObject.transform.position, Quaternion.identity);
                var newVFXInfo = new VFXInfo();
                await vfxManager.CreateVFX(AssetKeys.VFX_WindPropertySkill_Hit, projectileTrans, default, newVFXInfo);
                // 计时器计时
                timerManager.CreateTimer(false, 500, () =>
                {
                    newVFXInfo.IsStop = true;
                });
            }
        }
        
        public void PriestBattleSkillEvent(SkillContext skillContext, HitResult hitResult)
        {
            var projectileData = skillContext.ProjectileData;
            foreach (var target in projectileData.targets)
            {
                foreach (var statusId in skillContext.StatusIds)
                {
                    // 获取状态实例
                    var status = statusFactory.GetStatus(projectileData.caster, target, statusId);
                    // 添加状态
                    target.GetComponent<StatusComponent>().AddStatus(status);
                } 
            }
            
            // 回血
            foreach (var target in projectileData.targets)
            {
                target.TakeHeal(100);
            }
            // 恢复能量
            // ((PlayerSkill)projectileData.skill).RecoverEnergy();

            if (hitResult.IsFirstHit)
            {
                timerManager.CreateTimer(false, 500, () =>
                {
                    skillContext.VFXInfo.IsStop = true;
                });
            }
        }
        
        public async void PriestUltimateSkillEvent(SkillContext skillContext, HitResult hitResult)
        {
            var projectileData = skillContext.ProjectileData;
            foreach (var statusId in skillContext.StatusIds)
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
                    target, skillContext.SkillInfo, 
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
                    skillContext.VFXInfo.IsStop = true;
                });
            }
        }
    }
}
