using Framework;
using Game;
using Game.Battle;
using static UnityEditor.Timeline.TimelinePlaybackControls;

/// <summary>
/// 战斗界面事件处理器
/// </summary>
public class BattleEventProcessor
{
    private BattleController _battleController;
    private BattleUIManager _uiManager;
    private BattleUIInitializer _uiInitializer;

    public BattleEventProcessor(BattleController battleController, BattleUIManager uiManager, BattleUIInitializer uiInitializer)
    {
        _battleController = battleController;
        _uiManager = uiManager;
        _uiInitializer = uiInitializer;
    }

    /// <summary>
    /// 统一注册所有战斗事件
    /// </summary>
    public void RegisterBattleEvents(IBattleEventBus eventBus)
    {
        eventBus.AddListener<TurnStartEvent>(OnTurnStart);
        eventBus.AddListener<TurnEndEvent>(OnTurnEnd);
        eventBus.AddListener<OnBattlePointCountChangedEvent>(OnBattlePointCountChanged);
        eventBus.AddListener<SelectTargetEvent>(OnTargetSelectionChanged);
        eventBus.AddListener<HpChangedEvent>(OnHpChanged);
        eventBus.AddListener<ShieldChangedEvent>(OnShieldChanged);
        eventBus.AddListener<ApplyDamageEvent>(OnTakeDamage);
        eventBus.AddListener<PlayerReleaseSkillEvent>(OnPlayerReleaseSkillEvent);
        eventBus.AddListener<ActionBarSortPostEvent>(OnActionBarSortPostEvent);
        eventBus.AddListener<TurnStartStatusChangedEvent>(OnTurnStartStatusChangedEvent);
        eventBus.AddListener<StatusAddedEvent>(OnStatusAddedEvent);
        eventBus.AddListener<BattleOverEvent>(OnBattleOverEvent);
        eventBus.AddListener<MonsterDeadEvent>(OnMonsterDeadEvent);
    }

    /// <summary>
    /// 回合开始事件监听
    /// 更新玩家/怪物UI
    /// TODO：可优化为通过外部传入逻辑类来实现逻辑
    /// </summary>
    /// <param name="turnStartEvent"></param>
    private void OnTurnStart(TurnStartEvent turnStartEvent)
    {
        //if (turnStartEvent.CurrentBattleEntity is MonsterObject)
        //{
        //    // TODO：暂时写在这里，隐藏怪物UI，后续优化调用逻辑

        //    // 怪物攻击时才去隐藏怪物UI

        //}
        //else
        //{
        //    // 玩家相机位置不同，需要每回合开始时更新怪物UI位置
        //    await _uiInitializer.InitMonsterUI(turnStartEvent.Context.GetMonsterObjects());
        //}

        // 显示怪物UI
        _uiInitializer.InitMonsterUI(turnStartEvent.Context.GetMonsterObjects());

        if (turnStartEvent.CurrentBattleEntity is PlayerObject)
        {
            // 启用目标选择
            TargetSelectManager.Instance.ActiveSelectTarget();
            // 隐藏行动提示
            _uiManager.SetActTipActive(BattleUIManager.E_ActTipType.Hide);
            // 更新当前操作UI
            _uiManager.UpdateOperator(turnStartEvent.CurrentBattleEntity, IFactory.GetTypeInstance<SkillKeyUIDataProviderFactory, BaseSkillKeyUIDataProvider>());
        }
        else if (turnStartEvent.CurrentBattleEntity is MonsterObject)
        {
            // 禁用目标选择
            ServiceLocator.Get<ITargetSelectManager>().InActiveSelectTarget();
            // 清除UI
            _uiManager.ClearSelectMarker();
            // 隐藏玩家UI
            _uiManager.SetOperator(null);
            // 更新玩家行动提示
            _uiManager.SetActTipActive(BattleUIManager.E_ActTipType.Monster);
        }
    }

    /// <summary>
    /// 回合结束事件监听
    /// </summary>
    /// <param name="turnEndEvent"></param>
    private void OnTurnEnd(TurnEndEvent turnEndEvent)
    {

    }

    /// <summary>
    /// 回合开始状态变化事件回调
    /// </summary>
    /// <param name="turnStartStatusChangedEvent"></param>
    private void OnTurnStartStatusChangedEvent(TurnStartStatusChangedEvent turnStartStatusChangedEvent)
    {
        // 更新指定玩家状态栏
        _uiManager.UpdatePlayerStatuebar(turnStartStatusChangedEvent.CurrentBattleEntity);
    }

    /// <summary>
    /// 怪物死亡事件回调
    /// </summary>
    /// <param name="monsterDeadEvent"></param>
    private void OnMonsterDeadEvent(MonsterDeadEvent monsterDeadEvent)
    {
        _uiManager.HideNormalMonsterStateUI(monsterDeadEvent.DeadMonster);
    }

    /// <summary>
    /// 状态添加事件回调
    /// 显示状态浮动文本效果
    /// </summary>
    /// <param name="statusAddedEvent"></param>
    private void OnStatusAddedEvent(StatusAddedEvent statusAddedEvent)
    {
        _uiManager.ShowStatusText(statusAddedEvent.NewStatus);
    }

    /// <summary>
    /// 行动轴排序后事件回调
    /// </summary>
    /// <param name="actionBarSortPostEvent"></param>
    private void OnActionBarSortPostEvent(ActionBarSortPostEvent actionBarSortPostEvent)
    {
        _uiManager.UpdateActionBar(actionBarSortPostEvent.battleEntities);
    }

    /// <summary>
    /// 受到伤害事件回调
    /// 显示伤害文本
    /// </summary>
    /// <param name="applyDamageEvent"></param>
    private void OnTakeDamage(ApplyDamageEvent applyDamageEvent)
    {
        _uiManager.ShowDamageText(applyDamageEvent.DamageResult);
    }

    /// <summary>
    /// 玩家释放技能事件回调
    /// </summary>
    /// <param name="playerReleaseSkillEvent"></param>
    private void OnPlayerReleaseSkillEvent(PlayerReleaseSkillEvent playerReleaseSkillEvent)
    {
        // 禁用目标选择
        ServiceLocator.Get<ITargetSelectManager>().InActiveSelectTarget();
        // 清除标记UI
        _uiManager.ClearSelectMarker();
        // 隐藏玩家UI
        _uiManager.SetOperator(null);
        // 更新玩家行动提示
        _uiManager.SetActTipActive(BattleUIManager.E_ActTipType.Player);
    }

    /// <summary>
    /// 血量变化回调事件
    /// 显示恢复文本
    /// </summary>
    /// <param name="hpChangedEvent"></param>
    private void OnHpChanged(HpChangedEvent hpChangedEvent)
    {
        if (hpChangedEvent.DeltaHp < 0)
        {
            _uiManager.ShowHealText(hpChangedEvent.Target, hpChangedEvent.DeltaHp);
        }
    }

    /// <summary>
    /// 护盾变化事件回调
    /// 显示护盾变化文本
    /// </summary>
    /// <param name="onShieldChangedEvent"></param>
    private void OnShieldChanged(ShieldChangedEvent onShieldChangedEvent)
    {
        if (onShieldChangedEvent.DeltaShield < 0)
        {
            // 护盾增加显示

        }
        else
        {
            // 护盾减少显示

        }
    }

    /// <summary>
    /// 目标选择变化事件回调
    /// </summary>
    /// <param name="selectTargetEvent"></param>
    private void OnTargetSelectionChanged(SelectTargetEvent selectTargetEvent)
    {
        if (selectTargetEvent.MainTarget is PlayerObject)
        {
            return;
        }

        // 更新目标标记UI显示
        _uiManager.UpdateTargetMarker(selectTargetEvent.SelectedTargets);
        // 更新行动轴目标高亮显示
        _uiManager.UpdateActionGridHighlight(selectTargetEvent.SelectedTargets);
    }

    /// <summary>
    /// 战技点变化事件回调
    /// </summary>
    /// <param name="battlePointCountChanged"></param>
    private async void OnBattlePointCountChanged(OnBattlePointCountChangedEvent battlePointCountChanged)
    {
        await _uiManager.UpdateBattlePointCount(battlePointCountChanged.CurentBattlePointCount, battlePointCountChanged.MaxBattlePointCount);
    }

    /// <summary>
    /// 战斗结束事件回调
    /// 显示战斗结束UI
    /// </summary>
    private void OnBattleOverEvent(BattleOverEvent battleOverEvent)
    {
        _uiManager.ShowBattleOver(battleOverEvent.Context);
    }
}
