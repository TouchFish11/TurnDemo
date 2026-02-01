using Core.Reflection;
using Core.Service;
using Game.Battle.Status;
using GameHotUpdate.Status;

namespace GameHotUpdate.Battle.Projectile.Projectiles.Role.FireFly
{
    /// <summary>
    /// FireFlyս�����ܵ�����
    /// </summary>
    public class FireFlyBattleSkillProjectile : InstantProjectile
    {
        protected override void OnInit()
        {
            dmgTimes = new float[] { 0.1f };
            // ����Buff
            foreach (int id in statusIds)
            {
                IStatus status = ServiceLocator.Get<IFactoryManager>().GetFactory<IStatusFactory, StatusFactory>().GetStatus(id);
                status.InitStatus(projectileData.caster, projectileData.caster, id);
                projectileData.caster.GetComponent<StatusComponent>().AddStatus(status);
            }

            base.OnInit();
        }
    }
}
