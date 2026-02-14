using Core.Reflection;
using Core.Service;
using Game.Battle.Status;
using Game.Battle.Status.Enum;
using GameHotUpdate.Status;

namespace GameHotUpdate.Battle.Projectile.Projectiles.Role.FireFly
{
    /// <summary>
    /// FireFlyս战技弹射物
    /// </summary>
    public class FireFlyBattleSkillProjectile : InstantProjectile
    {
        protected override void OnInit()
        {
            dmgTimes = new float[] { 0.1f };
            // ����Buff
            foreach (int id in statusIds)
            {
                // TODO：暂时这样处理
                var status = ServiceLocator.Get<IFactoryManager>().GetFactory<IStatusFactory, StatusFactory>().GetStatus(id);
                status.InitStatus(projectileData.caster, projectileData.caster, id);
                if (status.StatusProperty.StatusInfo.f_statusType == (byte)E_StatusType.Positive)
                {
                    projectileData.caster.GetComponent<StatusComponent>().AddStatus(status);
                }
            }

            base.OnInit();
        }
    }
}
