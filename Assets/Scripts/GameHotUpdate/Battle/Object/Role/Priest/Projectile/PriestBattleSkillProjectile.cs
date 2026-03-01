using Core.Reflection;
using Core.Service;
using Core.Time;
using GameHotUpdate.Battle.Projectile;
using GameHotUpdate.Battle.Status;

namespace GameHotUpdate.Battle.Object.Role.Priest.Projectile
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
                    var status = ServiceLocator.Get<IFactoryManager>().GetFactory<IStatusFactory, StatusFactory>().
                        GetStatus(projectileData.caster, target, statusId);
                    // 添加状态
                    target.GetComponent<StatusComponent>().AddStatus(status);
                } 
            }
        }
        
        protected override void CreateVFXOnTrigger()
        {
            
        }
        
        protected override void CauseDamageOnTrigger()
        {
            // 回血
            foreach (var target in projectileData.targets)
            {
                target.TakeHeal(100);
            }
        }

        protected override void HandleTiming()
        {
            ServiceLocator.Get<ITimerManager>().CreateTimer(false, 500, () =>
            {
                vFXInfo.IsStop = true;
            });
        }
    }
}
