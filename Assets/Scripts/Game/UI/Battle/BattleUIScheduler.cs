using Framework;
using Game;
using Game.Battle;

/// <summary>
/// 战斗UI调度器
/// 非多个组合调用的可以使用事件通信，组合调用的使用调度器通信
/// </summary>
public class BattleUIScheduler : SingletonAutoMono<BattleUIScheduler>
{
    private BattleController battleController;

    private void Awake()
    {
        battleController = ServiceLocator.Get<IUIManager>().GetView<BattleController>();
    }

    /// <summary>
    /// 终结技触发时UI变化
    /// 显示终结技角色立绘、隐藏行动提示
    /// </summary>
    /// <param name="caster"></param>
    /// <param name="skillInfo"></param>
    public void UltimateTriggerChangeUI(IBattleEntityObject caster, SkillInfo skillInfo)
    {
        // 显示角色立绘
        battleController.GetBattleUI().ShowPaiting((caster as PlayerObject).RoleInfo, skillInfo);
        // 隐藏行动提示
        battleController.GetBattleUI().SetActTipActive(BattleUIManager.E_ActTipType.Hide);
        // 更新终结技UI显示
        battleController.GetBattleUI().UpdateOperator(caster, IFactory.GetTypeInstance<SkillKeyUIDataProviderFactory, UltimateSkillKeyUIDataProvider>());
        // 更新怪物血条位置
        battleController.GetUIInitializer().InitMonsterUI(caster.Context.GetMonsterObjects());
    }

    /// <summary>
    /// 更新相机和隐藏标记、怪物UI
    /// 怪物行动前调用
    /// </summary>
    /// <param name="context"></param>
    /// <param name="target"></param>
    public void UpdateCameraAndHideMarkerAndMonsterUI(IBattleContext context, IBattleEntityObject target)
    {
        // 激活被攻击的玩家相机
        BattlePoint.Instance.ActiveCamera(target);
        // 相互看向、看向攻击的玩家
        context.GetTurnManager().UpdateEntityLookAt(target);
        // 隐藏怪物UI
        ServiceLocator.Get<IUIManager>().GetView<BattleController>().GetUIInitializer().InitMonsterUI(null);
        // 禁用目标选择
        ServiceLocator.Get<ITargetSelectManager>().InActiveSelectTarget();
        // 清除标记UI
        battleController.GetBattleUI().ClearSelectMarker();
        // 隐藏玩家UI
        battleController.GetBattleUI().SetOperator(null);
        // 设置为怪物行动提示
        battleController.GetBattleUI().SetActTipActive(BattleUIManager.E_ActTipType.Monster);
    }

    /// <summary>
    /// 终结技释放时
    /// </summary>
    public void UltimateCasting()
    {
        // 清除标记UI
        battleController.GetBattleUI().ClearSelectMarker();
        battleController.GetBattleUI().SetOperator(null);
        battleController.GetBattleUI().SetActTipActive(BattleUIManager.E_ActTipType.Hide);
    }

    public BattleController BattleController => battleController;
}
