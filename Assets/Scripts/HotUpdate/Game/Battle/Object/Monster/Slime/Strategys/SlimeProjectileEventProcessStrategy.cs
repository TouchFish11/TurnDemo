using HotUpdate.Game.Battle.Skill.Base;
using HotUpdate.Game.Battle.Status;
using HotUpdate.Game.VFX;
using UnityEngine;

namespace HotUpdate.Game.Battle.Object.Monster.Slime.Strategys
{
    public class SlimeProjectileEventProcessStrategy : ProjectileEventProcessStrategy
    {
        public override async void PriestNormalSkillEvent(SkillContext skillContext, HitResult hitResult)
        {
            var projectileData = skillContext.ProjectileData;
            
            // 先添加且只添加一次buff
            if (hitResult.IsFirstHit)
            {
                // 添加buff
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
            }
            
            // 这里可以直接移除特效
            skillContext.VFXInfo.IsStop = true;

            // 每段的伤害计算
            foreach (var target in projectileData.targets)
            {
                damageCalcManager.CalcSkillDamage(projectileData.caster, target, skillContext.SkillInfo, out var result);
                target.TakeDamage(result);
            }
            
            // 每段的命中特效创建
            foreach (var target in projectileData.targets)
            {
                var projectileTrans = new ProjectileTrans(target.GameObject.transform.position + Vector3.up * 0.5f, Quaternion.identity);
                var vfxInfo = new VFXInfo();
                await vfxManager.CreateVFX(AssetKeys.VFX_MonsterHit, projectileTrans, default, vfxInfo);
            }
        }
    }
}
