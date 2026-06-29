using System.Threading.Tasks;
using HotUpdate.Game.Battle.Skill.Base;
using HotUpdate.Game.Battle.Skill.Base.Flow;
using HotUpdate.Game.Battle.Statuses;

namespace HotUpdate.Game.Battle.Object.Monster.AbyssalMage.Skill.AbyssGift
{
    public class AbyssalMageAbyssGiftSkillEventProcessPhaseStrategy : SkillEventProcessPhaseStrategy
    {
        protected override Task OnTrigger(HitResult hitResult)
        {
            var projectileData = SkillContext.ProjectileData;
            foreach (var statusId in SkillContext.StatusIds)
            {
                // 获取状态实例
                var status = statusFactory.GetStatus(projectileData.caster, projectileData.caster, statusId);
                // 添加状态
                projectileData.caster.GetComponent<StatusComponent>().AddStatus(status);
            } 
        
            timerManager.CreateTimer(false, 1500, () =>
            {
                SkillContext.VFXInfo.IsStop = true;
            });
            
            return Task.CompletedTask;
        }
    }
}
