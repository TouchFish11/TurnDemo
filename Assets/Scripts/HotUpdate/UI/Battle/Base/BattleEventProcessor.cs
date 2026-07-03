using Core.DI;
using HotUpdate.Game.Battle.Core;
using HotUpdate.Game.Battle.Event;
using HotUpdate.Game.Battle.Event.General;
using HotUpdate.Game.Battle.Event.Skill;
using HotUpdate.Game.Battle.Event.Turn;
using HotUpdate.Game.Battle.Event.UI;
using HotUpdate.Game.Battle.Object.Monster;
using HotUpdate.Game.Battle.Object.Role;
using HotUpdate.Game.Battle.Operation;
using HotUpdate.Game.Battle.Skill;
using HotUpdate.Game.Battle.UI;

namespace HotUpdate.UI.Battle.Base
{
    /// <summary>
    /// 战斗事件处理器
    /// 负责监听并处理各类战斗相关事件，同步更新战斗UI表现
    /// </summary>
    public class BattleEventProcessor : IBattleEventProcessor
    {
        [Inject] private BattleCoordinator _battleCoordinator;
        
        // 战斗控制器，用于获取战斗核心逻辑相关数据和操作
        private readonly BattleController _battleController;
        // 战斗UI管理器，用于统一管理战斗界面的各类UI操作
        private readonly IBattleUIManager _uiManager;
        // 战斗UI初始化器，用于初始化战斗中各类UI组件
        private readonly IBattleUIInitializer _uiInitializer;
        private IBattleEventBus _eventBus;
        
        /// <summary>
        /// 战斗事件处理器构造函数
        /// </summary>
        /// <param name="battleController">战斗控制器实例</param>
        /// <param name="uiManager">战斗UI管理器实例</param>
        /// <param name="uiInitializer">战斗UI初始化器实例</param>
        public BattleEventProcessor(BattleController battleController, IBattleUIManager uiManager, IBattleUIInitializer uiInitializer)
        {
            _battleController = battleController;
            _uiManager = uiManager;
            _uiInitializer = uiInitializer;
        }

        /// <summary>
        /// 统一注册所有战斗事件
        /// 将各类战斗事件与对应的处理方法绑定到事件总线
        /// </summary>
        /// <param name="eventBus">战斗事件总线</param>
        public void RegisterBattleEvents(IBattleEventBus eventBus)
        {
            eventBus.AddListener<CommandExecuteEvent>(OnCommandExecuteEvent);     // 当前指令执行事件
            eventBus.AddListener<SwitchEntityTurnEvent>(OnSwitchEntityTurnEvent);   // 切换实体回合事件
            eventBus.AddListener<TurnEndEvent>(OnTurnEnd);                   // 回合结束事件
            eventBus.AddListener<OnBattlePointCountChangedEvent>(OnBattlePointCountChanged); // 战斗点数变化事件
            eventBus.AddListener<SelectTargetEvent>(OnTargetSelectionChanged);      // 目标选择事件
            eventBus.AddListener<PostCastEvent>(OnPostCastDispatch);    // 监听技能释放后通用逻辑事件
            eventBus.AddListener<UpdateWaitUiEvent>(OnUpdateWaitContentDispatch);  // 监听更新等待队列事件
            
            eventBus.AddListener<ApplyDamageEvent>(ApplyTakeDamage);            // 应用伤害事件
            eventBus.AddListener<ApplyShieldEvent>(ApplyShieldChanged);            // 提供护盾事件
            eventBus.AddListener<ApplyHealEvent>(ApplyHealChanged);            // 提供治疗事件
            eventBus.AddListener<ShieldChangedEvent>(OnShieldChanged);       // 护盾值变化事件
            eventBus.AddListener<HpChangedEvent>(OnHpChangedEvent);       // 血量变化事件
            
            eventBus.AddListener<ClearCumulativeDamageEvent>(OnClearCumulativeDamageEvent);     // 清空累计伤害显示事件
            eventBus.AddListener<PlayerReleaseSkillEvent>(OnPlayerReleaseSkillEvent); // 玩家释放技能事件
            eventBus.AddListener<ActionBarSortPostEvent>(OnActionBarSortPostEvent); // 行动条排序完成事件
            eventBus.AddListener<TurnStartStatusChangedEvent>(OnTurnStartStatusChangedEvent); // 回合开始状态变化事件
            eventBus.AddListener<StatusAddedEvent>(OnStatusAddedEvent);       // 状态添加事件
            eventBus.AddListener<BattleOverEvent>(OnBattleOverEvent);         // 战斗结束事件
            eventBus.AddListener<EntityDeadEvent>(OnEntityDeadEvent);       // 怪物死亡事件

            _eventBus = eventBus;
        }

        private void UnsubscribeBattleEvents()
        {
            _eventBus.RemoveListener<CommandExecuteEvent>(OnCommandExecuteEvent);     // 当前指令执行事件
            _eventBus.RemoveListener<SwitchEntityTurnEvent>(OnSwitchEntityTurnEvent);   // 切换实体回合事件
            _eventBus.RemoveListener<TurnEndEvent>(OnTurnEnd);                   // 回合结束事件
            _eventBus.RemoveListener<OnBattlePointCountChangedEvent>(OnBattlePointCountChanged); // 战斗点数变化事件
            _eventBus.RemoveListener<SelectTargetEvent>(OnTargetSelectionChanged);      // 目标选择事件
            _eventBus.RemoveListener<PostCastEvent>(OnPostCastDispatch);    // 监听技能释放后通用逻辑事件
            _eventBus.RemoveListener<UpdateWaitUiEvent>(OnUpdateWaitContentDispatch);  // 监听更新等待队列事件
            
            _eventBus.RemoveListener<ApplyDamageEvent>(ApplyTakeDamage);            // 应用伤害事件
            _eventBus.RemoveListener<ApplyShieldEvent>(ApplyShieldChanged);            // 提供护盾事件
            _eventBus.RemoveListener<ApplyHealEvent>(ApplyHealChanged);            // 提供治疗事件
            _eventBus.RemoveListener<ShieldChangedEvent>(OnShieldChanged);       // 护盾值变化事件
            _eventBus.RemoveListener<HpChangedEvent>(OnHpChangedEvent);       // 血量变化事件
            
            _eventBus.RemoveListener<ClearCumulativeDamageEvent>(OnClearCumulativeDamageEvent);     // 清空累计伤害显示事件
            _eventBus.RemoveListener<PlayerReleaseSkillEvent>(OnPlayerReleaseSkillEvent); // 玩家释放技能事件
            _eventBus.RemoveListener<ActionBarSortPostEvent>(OnActionBarSortPostEvent); // 行动条排序完成事件
            _eventBus.RemoveListener<TurnStartStatusChangedEvent>(OnTurnStartStatusChangedEvent); // 回合开始状态变化事件
            _eventBus.RemoveListener<StatusAddedEvent>(OnStatusAddedEvent);       // 状态添加事件
            _eventBus.RemoveListener<BattleOverEvent>(OnBattleOverEvent);         // 战斗结束事件
            _eventBus.RemoveListener<EntityDeadEvent>(OnEntityDeadEvent);       // 怪物死亡事件
        }

        /// <summary>
        /// 当前指令执行事件
        /// </summary>
        /// <param name="commandExecuteEvent"></param>
        private void OnCommandExecuteEvent(CommandExecuteEvent commandExecuteEvent)
        {
            // 更新UI显示，设置当前执行指令的对象的Icon
            _uiManager.SetCurrentCommanderDisplayUI(commandExecuteEvent.CurrentCommand.Sender);
        }
        
        /// <summary>
        /// 切换实体回合事件
        /// </summary>
        /// <param name="switchEntityTurnEvent"></param>
        private void OnSwitchEntityTurnEvent(SwitchEntityTurnEvent switchEntityTurnEvent)
        {
            _uiManager.SetCurrentCommanderDisplayUI(switchEntityTurnEvent.CurrentBattleEntityObject);
            // 移除持有当前回合的对象对应的行动格子
            _uiManager.RemoveActionGrid(switchEntityTurnEvent.CurrentBattleEntityObject);
        }
        
        /// <summary>
        /// 回合结束事件处理方法
        /// 预留回合结束后的逻辑扩展点
        /// </summary>
        /// <param name="turnEndEvent">回合结束事件数据</param>
        private void OnTurnEnd(TurnEndEvent turnEndEvent)
        {

        }

        /// <summary>
        /// 回合开始状态变化事件处理方法
        /// 更新玩家状态栏UI展示
        /// </summary>
        /// <param name="turnStartStatusChangedEvent">回合开始状态变化事件数据</param>
        private void OnTurnStartStatusChangedEvent(TurnStartStatusChangedEvent turnStartStatusChangedEvent)
        {
            // 更新当前战斗实体（玩家/怪物）的状态栏UI
            _uiManager.UpdatePlayerStatuebar(turnStartStatusChangedEvent.CurrentBattleEntity);
        }

        /// <summary>
        /// 实体死亡事件处理方法
        /// </summary>
        /// <param name="entityDeadEvent">怪物死亡事件数据</param>
        private void OnEntityDeadEvent(EntityDeadEvent entityDeadEvent)
        {
            if (entityDeadEvent.DeadEntity is MonsterObject monsterObject)
            {
                // 调用控制器维护的怪物状态UI管理器对象隐藏死亡的怪物的UI
                _battleController.MonsterStateUIManager.RemoveNormalMonsterStateUI(entityDeadEvent.DeadEntity);
                _uiManager.RemoveActionGrid(monsterObject);
            }
        }

        /// <summary>
        /// 状态添加事件处理方法
        /// 显示新添加状态的文本提示效果
        /// </summary>
        /// <param name="statusAddedEvent">状态添加事件数据</param>
        private void OnStatusAddedEvent(StatusAddedEvent statusAddedEvent)
        {
            _uiManager.ShowStatusText(statusAddedEvent.NewStatus);
        }

        /// <summary>
        /// 行动条排序完成事件处理方法
        /// 根据排序后的战斗实体列表更新行动条UI
        /// </summary>
        /// <param name="actionBarSortPostEvent">行动条排序完成事件数据</param>
        private void OnActionBarSortPostEvent(ActionBarSortPostEvent actionBarSortPostEvent)
        {
            _uiManager.UpdateActionBar(actionBarSortPostEvent.Context, actionBarSortPostEvent.battleEntities);
        }

        /// <summary>
        /// 清空累计伤害显示事件回调
        /// </summary>
        /// <param name="clearCumulativeDamageEvent"></param>
        private void OnClearCumulativeDamageEvent(ClearCumulativeDamageEvent clearCumulativeDamageEvent)
        {
            _uiManager.UpdateCumulativeDamage(false, 0);
        }

        /// <summary>
        /// 受到伤害事件处理方法
        /// 显示伤害数值文本提示
        /// </summary>
        /// <param name="applyDamageEvent">受到伤害事件数据</param>
        private void ApplyTakeDamage(ApplyDamageEvent applyDamageEvent)
        {
            _uiManager.ShowDamageText(applyDamageEvent.DamageResult);
        }

        /// <summary>
        /// 提供护盾事件处理方法
        /// </summary>
        /// <param name="applyShieldEvent"></param>
        private void ApplyShieldChanged(ApplyShieldEvent applyShieldEvent)
        {
            _uiManager.ShowShieldText(applyShieldEvent.Target, applyShieldEvent.ShieldAmount);
        }
        
        /// <summary>
        /// 提供治疗事件处理方法
        /// </summary>
        /// <param name="applyHealEvent"></param>
        private void ApplyHealChanged(ApplyHealEvent applyHealEvent)
        {
            _uiManager.ShowHealText(applyHealEvent.Target, applyHealEvent.HealAmount);
        }
        
        /// <summary>
        /// 护盾量变化事件回调
        /// </summary>
        /// <param name="onShieldChangedEvent">护盾值变化事件数据</param>
        private void OnShieldChanged(ShieldChangedEvent onShieldChangedEvent)
        {
            if (onShieldChangedEvent.DeltaShield < 0)
            {
                // 护盾扣除提示显示
                _uiManager.ShowShieldText(onShieldChangedEvent.Target, onShieldChangedEvent.DeltaShield);
            }
        }

        private void OnHpChangedEvent(HpChangedEvent hpChangedEvent)
        {
            if (!hpChangedEvent.Target.IsDead && hpChangedEvent.CurrentHp <= 0)
            {
                _uiManager.RemoveActionGrid(hpChangedEvent.Target);
            }
        }
        
        /// <summary>
        /// 玩家释放技能事件处理方法
        /// 释放技能后关闭目标选择、清空选中标记和操作面板，显示玩家行动提示
        /// </summary>
        /// <param name="playerReleaseSkillEvent">玩家释放技能事件数据</param>
        private void OnPlayerReleaseSkillEvent(PlayerReleaseSkillEvent playerReleaseSkillEvent)
        {
            // 关闭目标选择功能
            _battleCoordinator.IsActiveTargetSelect = false;
            // 禁用输入
            _battleCoordinator.IsActiveInput = false;
            // 清除选中目标的标记UI
            _uiManager.ClearSelectMarker();
            // 清空操作面板
            _uiManager.ClearOperator();
            // 显示玩家行动提示
            _uiManager.SetActTipActive(EActTipType.Player);
        }

        /// <summary>
        /// 目标选择变化事件处理方法
        /// 跳过玩家目标的选择标记更新，更新选中目标的标记UI和行动网格高亮状态
        /// </summary>
        /// <param name="selectTargetEvent">目标选择变化事件数据</param>
        private void OnTargetSelectionChanged(SelectTargetEvent selectTargetEvent)
        {
            if (selectTargetEvent.Selecter is PlayerObject)
            {
                // 玩家选择玩家：更新相机看向玩家，显示蓝色的标记；玩家选择怪物：更新相机看向怪物，显示红色的标记
                var skillTargetType = selectTargetEvent.MainTarget is PlayerObject ? E_SkillTargetType.Friend : E_SkillTargetType.Enemy;
                // 设置选中目标的标记UI显示
                _uiManager.SetTargetMarkers(selectTargetEvent.SelectedTargets, skillTargetType);
            }
            else if(selectTargetEvent.MainTarget is MonsterObject)
            {
                if (selectTargetEvent.MainTarget is PlayerObject)
                {
                    // 怪物选择玩家：只需更新行动格子显示
                }
                else
                {
                    // 怪物选择怪物：只需更新行动格子显示
                }
            }
            
            // 更新行动网格中选中目标的高亮状态
            _uiManager.SetActionGridHighlights(selectTargetEvent.SelectedTargets);
        }
        
        /// <summary>
        /// 技能释放后处理逻辑
        /// </summary>
        /// <param name="postCastEvent"></param>
        private void OnPostCastDispatch(PostCastEvent postCastEvent)
        {
            // 更新累计伤害UI
            _uiManager.UpdateCumulativeDamage(false, 0);
        }
        
        /// <summary>
        /// 更新等待队列事件回调
        /// </summary>
        /// <param name="updateWaitUiEvent"></param>
        private void OnUpdateWaitContentDispatch(UpdateWaitUiEvent updateWaitUiEvent)
        {
            _uiManager.UpdateWaitingContent(updateWaitUiEvent.Context, updateWaitUiEvent.DisplayPendingCommands);
        }

        /// <summary>
        /// 战斗点数变化事件处理方法
        /// 异步更新战斗点数的UI展示（当前点数/最大点数）
        /// </summary>
        /// <param name="battlePointCountChanged">战斗点数变化事件数据</param>
        private async void OnBattlePointCountChanged(OnBattlePointCountChangedEvent battlePointCountChanged)
        {
            await _uiManager.UpdateBattlePointCount(battlePointCountChanged.CurentBattlePointCount, battlePointCountChanged.MaxBattlePointCount);
        }

        /// <summary>
        /// 战斗结束事件处理方法
        /// 显示战斗结束的结果UI
        /// </summary>
        /// <param name="battleOverEvent">战斗结束事件数据</param>
        private void OnBattleOverEvent(BattleOverEvent battleOverEvent)
        {
            _uiManager.ShowBattleOver(battleOverEvent.Context);
        }

        public void Dispose()
        {
            UnsubscribeBattleEvents();
        }
    }
}