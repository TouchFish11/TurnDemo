using Core.Pool;
using Core.Reflection;
using Core.Service;
using Core.Time;
using HotUpdate.Battle.Projectile;
using HotUpdate.Battle.Status;
using HotUpdate.Config;
using HotUpdate.VFX;
using UnityEngine;

namespace HotUpdate.Battle.Object.Monster.AbyssalMage.Projectile
{
    /// <summary>
    /// 烬陨技能弹射物
    /// </summary>
    public class AshfallSkillProjectile : InstantProjectile
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
            }
        }

        protected override async void CreateVFXOnTrigger()
        {
            foreach (var target in projectileData.targets)
            {
                var projectileTrans = new ProjectileTrans(target.GameObject.transform.position, Quaternion.identity);
                var newVFXInfo = ServiceLocator.Get<IPoolManager>().GetData<VFXInfo>();
                await ServiceLocator.Get<IVFXManager>().CreateVFX(ResKeyCollection.VFX_FirePropertySkill_Hit, projectileTrans, default, newVFXInfo);
                // 计时器计时
                ServiceLocator.Get<ITimerManager>().CreateTimer(false, 500, () =>
                {
                    newVFXInfo.IsStop = true;
                });
            }
        }

        protected override void HandleTiming()
        {
            // 计时器计时
            ServiceLocator.Get<ITimerManager>().CreateTimer(false, 1020, () =>
            {
                vFXInfo.IsStop = true;
            });
        }
    }
}
