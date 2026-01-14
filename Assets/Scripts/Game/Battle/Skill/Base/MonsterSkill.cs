using Framework;
using Game.Battle;
using static UnityEngine.GraphicsBuffer;

/// <summary>
/// 怪物技能
/// 怪物角色技能继承
/// </summary>
public abstract class MonsterSkill : Skill
{
    protected MonsterSkill(IBattleEntityObject caster, int skillId, ISkillCastPostHandler postHandler, IStatusAddStrategy statusAddStrategy) : base(caster, skillId, postHandler, statusAddStrategy)
    {

    }

    /// <summary>
    /// 怪物技能释放前执行
    /// 更新UI相关逻辑
    /// </summary>
    /// <param name="context"></param>
    protected override void OnPreCast(IBattleContext context)
    {
        base.OnPreCast(context);
        // 激活被攻击的玩家相机
        BattlePoint.Instance.ActiveCamera(MainTarget);
        // 相互看向、看向攻击的玩家
        context.GetTurnManager().UpdateEntityLookAt(MainTarget);
        // 禁用目标选择
        ServiceLocator.Get<ITargetSelectManager>().InActiveSelectTarget();
        // 隐藏怪物UI
        BattleController controller = ServiceLocator.Get<IUIManager>().GetView<BattleController>();
        controller.GetUIInitializer().InitMonsterUI(null);
        // 清除标记UI
        controller.GetBattleUI().ClearSelectMarker();
        // 隐藏玩家UI
        controller.GetBattleUI().SetOperator(null);
        // 设置为怪物行动提示
        controller.GetBattleUI().SetActTipActive(BattleUIManager.E_ActTipType.Monster);
    }
}
