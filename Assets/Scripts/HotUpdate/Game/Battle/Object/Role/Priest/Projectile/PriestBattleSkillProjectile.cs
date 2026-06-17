using Core.DI;
using Core.Time;
using HotUpdate.Game.Battle.Projectile;
using HotUpdate.Game.Battle.Skill.Base;
using HotUpdate.Game.Battle.Status;

namespace HotUpdate.Game.Battle.Object.Role.Priest.Projectile
{
    /// <summary>
    /// 牧师战技弹射物
    /// </summary>
    public class PriestBattleSkillProjectile : InstantProjectile
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
        
        protected override void CreateVFXOnTrigger()
        {
            
        }
        
        protected override void ApplyEffectOnTrigger()
        {
            // 回血
            foreach (var target in projectileData.targets)
            {
                target.TakeHeal(100);
            }
            // 恢复能量
            ((PlayerSkill)projectileData.skill).RecoverEnergy();
        }

        protected override void HandleTiming()
        {
            DIContainer.GetInstance<ITimerManager>().CreateTimer(false, 500, () =>
            {
                vFXInfo.IsStop = true;
            });
        }
    }
}
