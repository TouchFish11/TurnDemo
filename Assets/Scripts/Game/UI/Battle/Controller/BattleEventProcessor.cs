using Framework;
using Game;
using Game.Battle;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class BattleEventProcessor
{
    private BattleController _battleController;
    private BattleUIManager _uiManager;
    private BattleUIInitializer _uiInitializer;

    private IBattleEntityObject currentActObject;

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
        eventBus.AddListener<CommandWaitEvent>(OnCommandWaitEvent);
        eventBus.AddListener<OnBattlePointCountChangedEvent>(OnBattlePointCountChanged);
        eventBus.AddListener<SelectTargetEvent>(OnTargetSelectionChanged);
        eventBus.AddListener<HpChangedEvent>(OnHpChanged);
        eventBus.AddListener<ShieldChangedEvent>(OnShieldChanged);
        eventBus.AddListener<ApplyDamageEvent>(OnTakeDamage);
        eventBus.AddListener<PlayerTriggerSkillEvent>(OnPlayerTriggerSkill);
        eventBus.AddListener<ShowUltimateUIEvent>(OnShowUltimateUIEvent);
        eventBus.AddListener<UltimateReleaseOverEvent>(OnUltimateReleaseOverEvent);
        eventBus.AddListener<ActionBarSortPostEvent>(OnActionBarSortPostEvent);
    }

    /// <summary>
    /// 回合开始事件监听
    /// </summary>
    /// <param name="turnStartEvent"></param>
    private async void OnTurnStart(TurnStartEvent turnStartEvent)
    {
        // 记录当前行动角色
        currentActObject = turnStartEvent.CurrentBattleEntity;

        if (currentActObject is MonsterObject)
        {
            // TODO：暂时写在这里，隐藏怪物UI，后续优化调用逻辑
            await _uiInitializer.InitMonsterUI(new IBattleEntityObject[0]);
        }
        else
        {
            // 玩家相机位置不同，需要每回合开始时更新怪物UI位置
            await _uiInitializer.InitMonsterUI(turnStartEvent.Context.GetMonsterObjects());
        }

        if (turnStartEvent.CurrentBattleEntity is PlayerObject)
        {
            // 更新当前操作UI
            _uiManager.UpdateOperator(turnStartEvent.CurrentBattleEntity, SkillKeyUIDataProviderFactory.GetProvider<BaseSkillKeyUIDataProvider>());
        }
        else if (turnStartEvent.CurrentBattleEntity is MonsterObject)
        {
            _uiManager.HideOperator(true);
        }
    }

    /// <summary>
    /// 回合结束事件监听
    /// </summary>
    /// <param name="turnEndEvent"></param>
    private void OnTurnEnd(TurnEndEvent turnEndEvent)
    {
        // TODO：不是回合结束，而是造成伤害的指令结束后清空
        _uiManager.UpdateCumulativeDamage(false, 0);
    }

    /// <summary>
    /// 行动轴排序后事件回调
    /// </summary>
    /// <param name="actionBarSortPostEvent"></param>
    private async void OnActionBarSortPostEvent(ActionBarSortPostEvent actionBarSortPostEvent)
    {
        await _uiManager.UpdateActionBar(actionBarSortPostEvent.battleEntities);
    }

    /// <summary>
    /// 命令等待事件
    /// 更新命令排队UI
    /// </summary>
    /// <param name="commandWaitEvent"></param>
    private async void OnCommandWaitEvent(CommandWaitEvent commandWaitEvent)
    {
        _uiManager.UpdateWaitingCommmand(commandWaitEvent.WaitingSkills);
    }

    /// <summary>
    /// 受到伤害事件回调
    /// 显示伤害文本
    /// </summary>
    /// <param name="applyDamageEvent"></param>
    private async void OnTakeDamage(ApplyDamageEvent applyDamageEvent)
    {
        _uiManager.ShowDamageText(applyDamageEvent.DamageResult);
    }

    /// <summary>
    /// 显示终结技界面事件回调
    /// </summary>
    /// <param name="showUltimateUIEvent"></param>
    private void OnShowUltimateUIEvent(ShowUltimateUIEvent showUltimateUIEvent)
    {
        ServiceLocator.Instance.Get<IMonoManager>().StartCoroutine(WaitForPaitingOver(showUltimateUIEvent.Skill.SkillInfo));
        // 更新终结技UI显示
        _uiManager.UpdateOperator(showUltimateUIEvent.Caster, SkillKeyUIDataProviderFactory.GetProvider<UltimateSkillKeyUIDataProvider>());
    }

    private IEnumerator WaitForPaitingOver(SkillInfo skillInfo)
    {
        // 显示角色立绘
        _uiManager.SetUltimatePaitingActive(true, null, skillInfo.f_name);
        // 显示一秒后隐藏
        yield return new WaitForSeconds(1f);
        _uiManager.SetUltimatePaitingActive(false, null, string.Empty);
    }


    /// <summary>
    /// 玩家触发技能事件回调
    /// </summary>
    /// <param name="playerTriggerSkillEvent"></param>
    private void OnPlayerTriggerSkill(PlayerTriggerSkillEvent playerTriggerSkillEvent)
    {
        _uiManager.HideOperator(false);
    }

    /// <summary>
    /// 血量变化回调事件
    /// 显示恢复文本
    /// </summary>
    /// <param name="hpChangedEvent"></param>
    private async void OnHpChanged(HpChangedEvent hpChangedEvent)
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
    /// 终结技释放结束事件回调
    /// 用于恢复当前行动角色操作UI
    /// </summary>
    /// <param name="ultimateReleaseOverEvent"></param>
    private void OnUltimateReleaseOverEvent(UltimateReleaseOverEvent ultimateReleaseOverEvent)
    {
        if (currentActObject is not PlayerObject)
        {
            return;
        }

        _uiManager.UpdateOperator(currentActObject, SkillKeyUIDataProviderFactory.GetProvider<BaseSkillKeyUIDataProvider>());
    }

    /// <summary>
    /// 目标选择变化事件回调
    /// </summary>
    /// <param name="selectTargetEvent"></param>
    private async void OnTargetSelectionChanged(SelectTargetEvent selectTargetEvent)
    {
        if (selectTargetEvent.MainTarget is PlayerObject)
        {
            return;
        }

        // 更新目标标记UI显示
        await _uiManager.UpdateTargetMarker(selectTargetEvent.SelectedTargets);
        // 更新行动轴目标高亮显示
        _uiManager.UpdateActionGridHighlight(selectTargetEvent.SelectedTargets);
    }

    /// <summary>
    /// 战技点变化事件
    /// </summary>
    /// <param name="battlePointCountChanged"></param>
    private async void OnBattlePointCountChanged(OnBattlePointCountChangedEvent battlePointCountChanged)
    {
        await _uiManager.UpdateBattlePointCount(battlePointCountChanged.CurentBattlePointCount, battlePointCountChanged.MaxBattlePointCount);
    }
}
