using System.Threading.Tasks;
using HotUpdate.Game.Battle.Skill.Base;
using HotUpdate.Game.Battle.Skill.Base.Flow;
using HotUpdate.Game.Battle.Statuses;

namespace HotUpdate.Game.Battle.Object.Role.Priest.Skill.Battle
{
    public class PriestBattleSkillEventProcessPhaseStrategy : SkillEventProcessPhaseStrategy
    {
        protected override Task OnTrigger(HitResult hitResult)
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
            
            // 回血
            foreach (var target in projectileData.targets)
            {
                target.TakeHeal(100);
            }
            // 恢复终结技能量
            ((PlayerObject)projectileData.caster).RecoverUltimate(SkillContext.SkillInfo.f_recoveryEnergy);

            if (hitResult.IsFirstHit)
            {
                timerManager.CreateTimer(false, 500, () =>
                {
                    SkillContext.VFXInfo.IsStop = true;
                });
            }
            
            return Task.CompletedTask;
        }
    }
}
