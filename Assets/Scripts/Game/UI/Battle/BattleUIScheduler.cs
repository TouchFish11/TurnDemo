using Framework;
using Game.Battle;
using System;
using System.Collections.Generic;

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
    /// 显示终结技角色立绘
    /// </summary>
    /// <param name="caster"></param>
    /// <param name="skillInfo"></param>
    public void ShowUltimatePaiting(IBattleEntityObject caster, SkillInfo skillInfo)
    {
        // 显示角色立绘
        battleController.GetBattleUI().ShowPaiting(skillInfo);
        // 更新终结技UI显示
        battleController.GetBattleUI().UpdateOperator(caster, IFactory.GetTypeInstance<SkillKeyUIDataProviderFactory, UltimateSkillKeyUIDataProvider>());
    }

    /// <summary>
    /// 更新命令排队显示
    /// </summary>
    /// <param name="iconPaths"></param>
    public void UpdateWaitingCommmand(List<string> iconPaths)
    {
        battleController.GetBattleUI().UpdateWaitingCommmand(iconPaths);
    }

    /// <summary>
    /// 更新累计伤害UI
    /// </summary>
    /// <param name="isShow"></param>
    /// <param name="dmg"></param>
    public void UpdateCumulativeDamage(bool isShow, int dmg)
    {
        battleController.GetBattleUI().UpdateCumulativeDamage(isShow, dmg);
    }

    /// <summary>
    /// 清理活跃的伤害文本
    /// </summary>
    public void ClearActiveDamageTextUI()
    {
        battleController.GetBattleUI().ClearActiveDamageTextUI();
    }

    /// <summary>
    /// 更新相机和标记和怪物UI
    /// </summary>
    /// <param name="context"></param>
    /// <param name="battleEntity"></param>
    /// <param name="skillInfo"></param>
    public async void UpdateCameraAndMarkerAndMonsterUI(IBattleContext context, IBattleEntityObject battleEntity, SkillInfo skillInfo)
    {
        // 激活玩家相机
        BattlePoint.Instance.ActiveCamera(battleEntity);
        // 看向攻击的玩家
        battleEntity.Context.GetTurnManager().UpdateEntityLookAt(battleEntity);
        // 更新目标选择
        ServiceLocator.Get<ITargetSelectManager>().ReSelectTarget(context, battleEntity, skillInfo);
        // 更新怪物血条位置
        await ServiceLocator.Get<IUIManager>().GetView<BattleController>().GetUIInitializer().InitMonsterUI(context.GetMonsterObjects());
    }

    /// <summary>
    /// 更新相机和隐藏标记、怪物UI
    /// </summary>
    /// <param name="context"></param>
    /// <param name="target"></param>
    public async void UpdateCameraAndHideMarkerAndMonsterUI(IBattleContext context, IBattleEntityObject target)
    {
        // 激活相机
        BattlePoint.Instance.ActiveCamera(target);
        // 相互看向、看向攻击的玩家
        context.GetTurnManager().UpdateEntityLookAt(target);
        // 隐藏怪物UI
        await ServiceLocator.Get<IUIManager>().GetView<BattleController>().GetUIInitializer().InitMonsterUI(null);
        // 禁用选择
        ServiceLocator.Get<ITargetSelectManager>().InActiveSelectTarget();
        // 清理标记
        await ServiceLocator.Get<IUIManager>().GetView<BattleController>().GetBattleUI().UpdateTargetMarker(null);
    }

    /// <summary>
    /// 战斗界面控制器
    /// </summary>
    //public BattleController BattleController => battleController;
}
