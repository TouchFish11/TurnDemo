using Core.DI;
using Core.Reflection;
using Core.Time;
using HotUpdate.Game.Battle.Projectile;
using HotUpdate.Game.Battle.Status;

namespace HotUpdate.Game.Battle.Object.Monster.AbyssalMage.Projectile
{
    /// <summary>
    /// 渊禁技能弹射物
    /// </summary>
    public class AbyssLockSkillProjectile : InstantProjectile
    {
        protected override void AddStatusOnTrigger()
        {
            foreach (var target in projectileData.targets)
            {
                foreach (var statusId in statusIds)
                {
                    // 获取状态实例
                    var status = DIContainer.GetInstance<IFactoryManager>().GetFactory<IStatusFactory, StatusFactory>().
                        GetStatus(projectileData.caster, target, statusId);
                    // 添加状态
                    target.GetComponent<StatusComponent>().AddStatus(status);
                } 
            }
        }

        protected override void ApplyEffectOnTrigger()
        {

        }

        protected override void CreateVFXOnTrigger()
        {

        }

        protected override void HandleTiming()
        {
            // 计时器计时
            DIContainer.GetInstance<ITimerManager>().CreateTimer(false, 1500, () =>
            {
                vFXInfo.IsStop = true;
            });
        }
    }
}
