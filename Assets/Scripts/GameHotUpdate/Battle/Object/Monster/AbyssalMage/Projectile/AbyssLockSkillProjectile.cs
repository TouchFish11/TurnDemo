using Core.Reflection;
using Core.Service;
using Game.Battle.Status;
using GameHotUpdate.Battle.Projectile;
using GameHotUpdate.Battle.Status;

namespace GameHotUpdate.Battle.Object.Monster.AbyssalMage.Projectile
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
                    var status = ServiceLocator.Get<IFactoryManager>().GetFactory<IStatusFactory, StatusFactory>().
                        GetStatus(projectileData.caster, target, statusId);
                    // 添加状态
                    target.GetComponent<StatusComponent>().AddStatus(status);
                } 
            }
        }

        protected override void CauseDamageOnTrigger()
        {

        }

        protected override void CreateVFXOnTrigger()
        {

        }

        protected override void HandleOtherOnTrigger()
        {

        }
    }
}
