using System.Threading.Tasks;
using HotUpdate.Game.Battle.Skill.Base;
using HotUpdate.Game.Battle.Skill.Base.Flow;
using HotUpdate.Game.Battle.Statuses;

namespace HotUpdate.Game.Battle.Object.Monster.AbyssalMage.Skill.AbyssLock
{
    public class AbyssalMageAbyssLockSkillEventProcessPhaseStrategy : SkillEventProcessPhaseStrategy
    {
        protected override Task OnTrigger(HitResult hitResult)
        {
            var projectileData = SkillContext.ProjectileData;
            foreach (var target in projectileData.targets)
            {
                // 计时器计时
                timerManager.CreateTimer(false, 1500, () =>
                {
                    SkillContext.VFXInfo.IsStop = true;
                });
                
                foreach (var statusId in SkillContext.StatusIds)
                {
                    // 获取状态实例
                    var status = statusFactory.GetStatus(projectileData.caster, target, statusId);
                    // 添加状态
                    target.GetComponent<StatusComponent>().AddStatus(status);
                } 
            }
            
            return Task.CompletedTask;
        }
    }
}
