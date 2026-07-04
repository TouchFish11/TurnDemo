using System;
using Core.DI;
using Core.Serialize.Binary;
using HotUpdate.Base.Enums;
using HotUpdate.Base.UI;
using HotUpdate.Common.Config.ExcelInfo.Container;
using HotUpdate.Game.Animation.Component;
using HotUpdate.Game.Battle.Context;
using HotUpdate.Game.Battle.Core;
using HotUpdate.Game.Battle.Event.Skill;
using HotUpdate.Game.Battle.Event.Turn;
using HotUpdate.Game.Battle.Event.UI;
using HotUpdate.Game.Battle.Object.Monster;
using HotUpdate.Game.Battle.Object.Role;
using HotUpdate.Game.Battle.Operation;
using HotUpdate.Game.Battle.Operation.Provider;
using HotUpdate.Game.Battle.Skill;
using HotUpdate.Game.Battle.UI;
using Logger = Core.Log.Logger;

namespace HotUpdate.Game.Battle.Event
{
    /// <summary>
    /// 战斗事件逻辑调度器
    /// 监听复杂战斗事件，执行其它模块的统一调用
    /// </summary>
    public class BattleEventScheduler : IBattleEventScheduler
    {
        [Inject] private IBinaryDataManager _binaryDataManager;
        [Inject] private BattleCoordinator _battleCoordinator;
        [Inject] private ISkillKeyUIDataProviderFactory _skillKeyUIDataProviderFactory;
        [Inject] private IUIService _uiService;
        
        private IBattleContext _context;

        private BattleEventScheduler()
        {

        }

        public void Init(IBattleContext context)
        {
            context.EventBus.AddListener<QuitBattleEvent>(OnQuitBattleEvent);
            // 监听回合开始事件
            context.EventBus.AddListener<TurnStartEvent>(OnTurnStartDispatch);
            // 监听角色技能选择事件
            context.EventBus.AddListener<SelectSkillEvent>(SelectSkillEventScheduler);
            // 监听玩家角色终结技释放后通用逻辑事件
            context.EventBus.AddListener<UltimateCastEvent>(OnUltimateCastDispatch);
            // 监听玩家操作技能触发事件
            context.EventBus.AddListener<RoleTriggerSkillEvent>(OnRoleTriggerSkillEvent);
            _context = context;
        }

        /// <summary>
        /// 监听玩家技能按键触发操作
        /// </summary>
        /// <param name="roleTriggerSkillEvent"></param>
        private void OnRoleTriggerSkillEvent(RoleTriggerSkillEvent roleTriggerSkillEvent)
        {
            roleTriggerSkillEvent.Caster.CastSkill(roleTriggerSkillEvent.SkillId);
        }
        
        /// <summary>
        /// 终结技释放调度逻辑
        /// 处理终结释放时的通用逻辑
        /// </summary>
        private void OnUltimateCastDispatch(UltimateCastEvent ultimateCastEvent)
        {
            // 关闭目标选择（终结技释放时不再允许手动选择目标）
            _battleCoordinator.IsActiveTargetSelect = false;
            // 禁用输入
            _battleCoordinator.IsActiveInput = false;
            var controller = (IBattleController)_uiService.GetPanel(EUIPanelId.BattlePanel);
            controller.BattleUiManager.ClearSelectMarker();
            controller.BattleUiManager.ClearOperator();
            controller.BattleUiManager.SetActTipActive(EActTipType.Hide);
        }
        
        /// <summary>
        /// 回合开始事件调度逻辑
        /// </summary>
        /// <param name="turnStartEvent"></param>
        private async void OnTurnStartDispatch(TurnStartEvent turnStartEvent)
        {
            try
            {
                if (turnStartEvent.CurrentBattleEntity == null)
                {
                    Logger.LogError($"{nameof(BattleEventScheduler)}: CurrentBattleEntity is null");
                    return;
                }

                if (turnStartEvent.CurrentBattleEntity is PlayerObject playerObject)
                {
                    // 先执行战斗点位置变化
                    _battleCoordinator.UpdateMonsterPos(turnStartEvent.CurrentBattleEntity);
                    // 更新相机显示
                    await _battleCoordinator.UpdateCamera(playerObject);
                }
            
                var controller = (IBattleController)_uiService.GetPanel(EUIPanelId.BattlePanel);
                switch (turnStartEvent.CurrentBattleEntity)
                {
                    case PlayerObject:
                    {
                        // 角色行动才激活怪物UI显示
                        controller.MonsterStateUIManager.ActiveMonsterUIs();
                        // 启用输入
                        _battleCoordinator.IsActiveInput = true;
                        // 玩家回合：激活目标选择功能
                        _battleCoordinator.IsActiveTargetSelect = true;
                        // 隐藏行动提示
                        controller.BattleUiManager.SetActTipActive(EActTipType.Hide);
                        // 获取技能按键UI数据提供器
                        var provider = _skillKeyUIDataProviderFactory.GetProvider<BaseSkillKeyUIDataProvider>();
                        // 根据数据更新玩家操作按键，按键触发技能选择事件
                        controller.BattleUiManager.UpdateOperator(turnStartEvent.CurrentBattleEntity, provider);
                        break;
                    }
                    case MonsterObject:
                        // 怪物回合：关闭目标选择功能
                        _battleCoordinator.IsActiveTargetSelect = false;
                        // 清除选中目标的标记UI
                        controller.BattleUiManager.ClearSelectMarker();
                        // 清空操作面板
                        controller.BattleUiManager.ClearOperator();
                        // 显示怪物行动提示
                        controller.BattleUiManager.SetActTipActive(EActTipType.Monster);
                        break;
                }
            }
            catch (Exception e)
            {
                Logger.LogError($"[{nameof(BattleEventScheduler)}]: Round start event logic scheduling error,{e.Message}");
            }
        }

        /// <summary>
        /// 角色技能选择事件调度逻辑
        /// </summary>
        /// <param name="selectSkillEvent"></param>
        private async void SelectSkillEventScheduler(SelectSkillEvent selectSkillEvent)
        {
            try
            {
                if (selectSkillEvent.Caster is not PlayerObject playerObject)
                    return;
                
                // 读取技能信息
                var skillInfo = _binaryDataManager.GetConfig<SkillInfoContainer>(EConfigLoadType.Excel).dataDic[selectSkillEvent.SkillId];
                // 设置保存当前角色选择的技能信息
                _battleCoordinator.SetSelectSkillInfo(skillInfo);
                // 更新目标选择相关逻辑
                _battleCoordinator.SelectTargets(selectSkillEvent.Caster, selectSkillEvent.TargetSelectStrategy);
                // 更新相机视角
                await _battleCoordinator.UpdateCamera((E_SkillTargetType)skillInfo.f_SkillTargetType, playerObject);

                // 根据技能类型切换前置动画
                var battleAnimationComponent = playerObject.GetComponent<BattleAnimationComponent>();
                switch ((E_SkillType)skillInfo.f_SkillType)
                {
                    case E_SkillType.Monster: // 怪物技能 → 播放通用攻击动画
                        battleAnimationComponent.SetAnimationState((int)E_AnimationType.Attack);
                        break;
                    case E_SkillType.NormalAttack: // 普通攻击 → 播放预普通攻击动画
                        battleAnimationComponent.SetAnimationState((int)E_AnimationType.PreNormalAttack);
                        break;
                    case E_SkillType.CombatSkill: // 战斗技能 → 播放预战斗技能攻击动画
                        battleAnimationComponent.SetAnimationState((int)E_AnimationType.PreBattleAttack);
                        break;
                    case E_SkillType.EnhancedNormalAttack: // 强化普通攻击 → 暂未处理
                    case E_SkillType.EnhancedCombatSkill: // 强化战斗技能 → 暂未处理
                        break;
                }
            }
            catch (Exception e)
            {
                Logger.LogError($"{nameof(BattleEventScheduler)}: Character skill selection event scheduling logic execution error,{e.Message}");
            }
        }
        
        private void OnQuitBattleEvent(QuitBattleEvent quitBattleEvent)
        {
            _context.EventBus.RemoveListener<QuitBattleEvent>(OnQuitBattleEvent);
            _context.EventBus.RemoveListener<TurnStartEvent>(OnTurnStartDispatch);
            _context.EventBus.RemoveListener<SelectSkillEvent>(SelectSkillEventScheduler);
            _context.EventBus.RemoveListener<UltimateCastEvent>(OnUltimateCastDispatch);
            _context.EventBus.RemoveListener<RoleTriggerSkillEvent>(OnRoleTriggerSkillEvent);
            _context = null;
        }
    }
}
