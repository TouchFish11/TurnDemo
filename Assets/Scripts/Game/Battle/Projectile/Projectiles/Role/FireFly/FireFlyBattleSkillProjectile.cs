using Framework;
using Game.Battle;

/// <summary>
/// FireFly战技技能弹射物
/// </summary>
public class FireFlyBattleSkillProjectile : InstantProjectile
{
    protected override void OnInit()
    {
        dmgTimes = new float[] { 0.1f };
        // 添加Buff
        foreach (int id in statusIds)
        {
            IStatus status = ServiceLocator.Get<IFactoryManager>().GetFactory<StatusFactory>().GetStatus(id);
            status.InitStatus(projectileData.caster, projectileData.caster, id);
            projectileData.caster.GetComponent<StatusComponent>().AddStatus(status);
        }

        base.OnInit();
    }
}
