using Core.DI;
using Core.Reflection;
using Core.Time;
using HotUpdate.Base.Battle.Status;
using HotUpdate.Game.Battle.Projectile;
using HotUpdate.Game.Battle.Status;

namespace HotUpdate.Game.Battle.Object.Monster.AbyssalMage.Projectile
{
    /// <summary>
    /// 深渊之赐技能弹射物
    /// </summary>
    public class AbyssGiftSkillProjectile : InstantProjectile
    {
        protected override void AddStatusOnTrigger()
        {
            foreach (var statusId in statusIds)
            {
                // 获取状态实例
                var status = DIContainer.GetInstance<IFactoryManager>().GetFactory<IStatusFactory, StatusFactory>().
                    GetStatus(projectileData.caster, projectileData.caster, statusId);
                // 添加状态
                projectileData.caster.GetComponent<StatusComponent>().AddStatus(status);
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
            DIContainer.GetInstance<ITimerManager>().CreateTimer(false, 1500, () =>
            {
                vFXInfo.IsStop = true;
            });
        }
    }
}
