using Core.Config;
using Core.Reflection;
using Core.Service;
using Game.Battle.Status;
using Game.VFX;
using GameHotUpdate.Battle.Projectile;
using GameHotUpdate.Battle.Status;
using UnityEngine;

namespace GameHotUpdate.Battle.Object.Role.Wizard.Projectile
{
    /// <summary>
    /// 法师终结技弹射物
    /// </summary>
    public class WizardUltimateSkillProjectile : InstantProjectile
    {
        protected override void AddStatusOnTrigger()
        {
            foreach (var target in projectileData.targets)
            {
                foreach (var statusId in statusIds)
                {
                    // 获取状态实例
                    var status = ServiceLocator.Get<IFactoryManager>().GetFactory<IStatusFactory, StatusFactory>().
                        GetStatus(projectileData.caster, target, statusId);
                    // 添加状态
                    target.GetComponent<StatusComponent>().AddStatus(status);
                }
            }
        }
        
        protected override void CauseDamageOnTrigger()
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
        
        protected override void CreateVFXOnTrigger()
        {
            foreach (var target in projectileData.targets)
            {
                var projectileTrans = new ProjectileTrans(target.GameObject.transform.position, Quaternion.identity);
                vFXInfo = new VFXInfo();
                ServiceLocator.Get<IVFXManager>().CreateVFX(ResKeyCollection.VFX_BlueHit, projectileTrans, default, vFXInfo);
            }
        }

        protected override void HandleOtherOnTrigger()
        {
            
        }
    }
}
