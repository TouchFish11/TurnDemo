using HotUpdate.Game.Battle.Skill.Base;
using HotUpdate.Game.Battle.Status;
using HotUpdate.Game.VFX;
using UnityEngine;

namespace HotUpdate.Game.Battle.Object.Role.Warrior.Strategys
{
    public class WarriorProjectileEventProcessStrategy : ProjectileEventProcessStrategy
    {
        public async void NormalSkillEvent(SkillContext skillContext, HitResult hitResult)
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
                await vfxManager.CreateVFX(AssetKeys.VFX_FirePropertySkill_Hit, projectileTrans, default, newVFXInfo);
                // 计时器计时
                timerManager.CreateTimer(false, 500, () =>
                {
                    newVFXInfo.IsStop = true;
                });
            }
        }
        
        public async void BattleSkillEvent(SkillContext skillContext, HitResult hitResult)
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
                await vfxManager.CreateVFX(AssetKeys.VFX_FirePropertySkill_Hit, projectileTrans, default, newVFXInfo);
                // 计时器计时
                timerManager.CreateTimer(false, 500, () =>
                {
                    newVFXInfo.IsStop = true;
                });
            }
        }
        
        public async void UltimateSkillEvent(SkillContext skillContext, HitResult hitResult)
        {
            var projectileData = skillContext.ProjectileData;

            if (hitResult.IsFirstHit)
            {
                foreach (var statusId in skillContext.StatusIds)
                {
                    // 获取状态实例
                    var status = statusFactory.GetStatus(projectileData.caster, projectileData.caster, statusId);
                    // 添加状态
                    projectileData.caster.GetComponent<StatusComponent>().AddStatus(status);
                }
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
                await vfxManager.CreateVFX(AssetKeys.VFX_FirePropertySkill_Hit, projectileTrans, default, newVFXInfo);
                // 计时器计时
                timerManager.CreateTimer(false, 500, () =>
                {
                    newVFXInfo.IsStop = true;
                });
            }
        }
    }
}
