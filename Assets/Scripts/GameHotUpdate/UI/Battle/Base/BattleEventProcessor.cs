using Core.Service;
using Game.Battle.Event;
using Game.Battle.TargetSelect;
using GameHotUpdate.Battle.Event.General;
using GameHotUpdate.Battle.Event.Turn;
using GameHotUpdate.Battle.Event.UI;
using GameHotUpdate.Objects;

namespace GameHotUpdate.UI.Battle.Base
{
    /// <summary>
    /// 战斗事件处理器
    /// 负责监听并处理各类战斗相关事件，同步更新战斗UI表现
    /// </summary>
    public class BattleEventProcessor
    {
        // 战斗控制器，用于获取战斗核心逻辑相关数据和操作
        private BattleController _battleController;
        // 战斗UI管理器，用于统一管理战斗界面的各类UI操作
        private readonly BattleUIManager _uiManager;
        // 战斗UI初始化器，用于初始化战斗中各类UI组件
        private readonly BattleUIInitializer _uiInitializer;

        /// <summary>
        /// 战斗事件处理器构造函数
        /// </summary>
        /// <param name="battleController">战斗控制器实例</param>
        /// <param name="uiManager">战斗UI管理器实例</param>
        /// <param name="uiInitializer">战斗UI初始化器实例</param>
        public BattleEventProcessor(BattleController battleController, BattleUIManager uiManager, BattleUIInitializer uiInitializer)
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
            eventBus.AddListener<TurnStartEvent>(OnTurnStart);               // 回合开始事件
            eventBus.AddListener<TurnEndEvent>(OnTurnEnd);                   // 回合结束事件
            eventBus.AddListener<OnBattlePointCountChangedEvent>(OnBattlePointCountChanged); // 战斗点数变化事件
            eventBus.AddListener<SelectTargetEvent>(OnTargetSelectionChanged); // 目标选择事件
            eventBus.AddListener<HpChangedEvent>(OnHpChanged);               // 生命值变化事件
            eventBus.AddListener<ShieldChangedEvent>(OnShieldChanged);       // 护盾值变化事件
            eventBus.AddListener<ApplyDamageEvent>(OnTakeDamage);            // 受到伤害事件
            eventBus.AddListener<PlayerReleaseSkillEvent>(OnPlayerReleaseSkillEvent); // 玩家释放技能事件
            eventBus.AddListener<ActionBarSortPostEvent>(OnActionBarSortPostEvent); // 行动条排序完成事件
            eventBus.AddListener<TurnStartStatusChangedEvent>(OnTurnStartStatusChangedEvent); // 回合开始状态变化事件
            eventBus.AddListener<StatusAddedEvent>(OnStatusAddedEvent);       // 状态添加事件
            eventBus.AddListener<BattleOverEvent>(OnBattleOverEvent);         // 战斗结束事件
            eventBus.AddListener<MonsterDeadEvent>(OnMonsterDeadEvent);       // 怪物死亡事件
        }

        /// <summary>
        /// 回合开始事件处理方法
        /// 初始化怪物UI、切换目标选择状态、更新操作面板和行动提示等
        /// </summary>
        /// <param name="turnStartEvent">回合开始事件数据</param>
        private void OnTurnStart(TurnStartEvent turnStartEvent)
        {
            // // 初始化存活怪物的UI展示
            // _uiInitializer.InitMonsterUI(turnStartEvent.Context.GetAliveMonsterEntitys());
            //
            // if (turnStartEvent.CurrentBattleEntity is PlayerObject)
            // {
            //     // 玩家回合：激活目标选择功能
            //     TargetSelectManager.Instance.ActiveSelectTarget();
            //     // 隐藏行动提示
            //     _uiManager.SetActTipActive(E_ActTipType.Hide);
            //     // 获取技能按键UI数据提供器，更新玩家操作面板
            //     var provider = ServiceLocator.Get<IFactoryManager>().
            //         GetFactory<ISkillKeyUIDataProviderFactory, SkillKeyUIDataProviderFactory>().
            //         GetCastSkillCondition<BaseSkillKeyUIDataProvider>();
            //     _uiManager.UpdateOperator(turnStartEvent.CurrentBattleEntity, provider);
            // }
            // else if (turnStartEvent.CurrentBattleEntity is MonsterObject)
            // {
            //     // 怪物回合：关闭目标选择功能
            //     ServiceLocator.Get<ITargetSelectManager>().InActiveSelectTarget();
            //     // 清除选中目标的标记UI
            //     _uiManager.ClearSelectMarker();
            //     // 清空操作面板
            //     _uiManager.SetOperator(null);
            //     // 显示怪物行动提示
            //     _uiManager.SetActTipActive(E_ActTipType.Monster);
            // }
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
        /// 怪物死亡事件处理方法
        /// 隐藏死亡怪物的常规状态UI
        /// </summary>
        /// <param name="monsterDeadEvent">怪物死亡事件数据</param>
        private void OnMonsterDeadEvent(MonsterDeadEvent monsterDeadEvent)
        {
            _uiManager.HideNormalMonsterStateUI(monsterDeadEvent.DeadMonster);
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
            _uiManager.UpdateActionBar(actionBarSortPostEvent.battleEntities);
        }

        /// <summary>
        /// 受到伤害事件处理方法
        /// 显示伤害数值文本提示
        /// </summary>
        /// <param name="applyDamageEvent">受到伤害事件数据</param>
        private void OnTakeDamage(ApplyDamageEvent applyDamageEvent)
        {
            _uiManager.ShowDamageText(applyDamageEvent.DamageResult);
        }

        /// <summary>
        /// 玩家释放技能事件处理方法
        /// 释放技能后关闭目标选择、清空选中标记和操作面板，显示玩家行动提示
        /// </summary>
        /// <param name="playerReleaseSkillEvent">玩家释放技能事件数据</param>
        private void OnPlayerReleaseSkillEvent(PlayerReleaseSkillEvent playerReleaseSkillEvent)
        {
            // 关闭目标选择功能
            ServiceLocator.Get<ITargetSelectManager>().InActiveSelectTarget();
            // 清除选中目标的标记UI
            _uiManager.ClearSelectMarker();
            // 清空操作面板
            _uiManager.SetOperator(null);
            // 显示玩家行动提示
            _uiManager.SetActTipActive(E_ActTipType.Player);
        }

        /// <summary>
        /// 生命值变化事件处理方法
        /// 当生命值减少时，显示治疗/伤害相关的文本提示
        /// </summary>
        /// <param name="hpChangedEvent">生命值变化事件数据</param>
        private void OnHpChanged(HpChangedEvent hpChangedEvent)
        {
            // if (hpChangedEvent.DeltaHp < 0)
            // {
            //     _uiManager.ShowHealText(hpChangedEvent.Target, hpChangedEvent.DeltaHp);
            // }
        }

        /// <summary>
        /// 护盾值变化事件处理方法
        /// 根据护盾值增减，分别处理护盾扣除和护盾增加的文本提示逻辑（预留实现）
        /// </summary>
        /// <param name="onShieldChangedEvent">护盾值变化事件数据</param>
        private void OnShieldChanged(ShieldChangedEvent onShieldChangedEvent)
        {
            if (onShieldChangedEvent.DeltaShield < 0)
            {
                // 护盾扣除提示显示（预留实现）

            }
            else
            {
                // 护盾增加提示显示（预留实现）

            }
        }

        /// <summary>
        /// 目标选择变化事件处理方法
        /// 跳过玩家目标的选择标记更新，更新选中目标的标记UI和行动网格高亮状态
        /// </summary>
        /// <param name="selectTargetEvent">目标选择变化事件数据</param>
        private void OnTargetSelectionChanged(SelectTargetEvent selectTargetEvent)
        {
            // TODO：若主目标是玩家，不处理目标标记更新，暂时不处理更新
            if (selectTargetEvent.MainTarget is PlayerObject)
            {
                return;
            }

            // 更新选中目标的标记UI显示
            _uiManager.UpdateTargetMarker(selectTargetEvent.SelectedTargets);
            // 更新行动网格中选中目标的高亮状态
            _uiManager.UpdateActionGridHighlight(selectTargetEvent.SelectedTargets);
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
    }
}