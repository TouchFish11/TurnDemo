using System;
using System.Collections;
using Core.DI;
using Core.Mono.MonoFunction;
using Core.Serialize.Binary;
using Core.Utility;
using HotUpdate.Base;
using HotUpdate.Base.Manager;
using HotUpdate.Base.UI;
using HotUpdate.Common.Config.ExcelInfo.Container;
using HotUpdate.Common.Config.ExcelInfo.Info;
using HotUpdate.Game.Battle.Context;
using HotUpdate.Game.Battle.Event.General;
using HotUpdate.Game.Battle.Event.Skill;
using HotUpdate.Game.Battle.Event.Turn;
using HotUpdate.Game.Battle.Event.UI;
using HotUpdate.Game.Battle.Inputs;
using HotUpdate.Game.Battle.Layer;
using HotUpdate.Game.Battle.Object;
using HotUpdate.Game.Battle.Skill;
using HotUpdate.Game.Battle.TargetSelect;
using HotUpdate.Game.Battle.UI;
using HotUpdate.Game.Battle.UI.Provider;
using HotUpdate.Game.Point;
using UnityEngine;
using Logger = Core.Log.Logger;

namespace HotUpdate.Game.Battle.Event
{
    /// <summary>
    /// 战斗事件逻辑调度器
    /// 监听战斗事件，执行其它模块的统一调用
    /// </summary>
    public class BattleEventScheduler : IBattleEventScheduler, IDestroyable
    {
        [Inject] private ISkillKeyUIDataProviderFactory _skillKeyUIDataProviderFactory;
        [Inject] private IUIService _uiService;
        private IBattleContext _context;

        public BattleEventScheduler(IBattleContext context)
        {
            _context = context;
            // 监听战斗事件
            ListenerBattleEvent();
        }
        
        private void ListenerBattleEvent()
        {
            // 监听回合开始事件
            _context.GetEventBus().AddListener<TurnStartEvent>(OnTurnStartDispatch);
            // 监听角色技能选择事件
            _context.GetEventBus().AddListener<SelectSkillEvent>(SelectSkillEventScheduler);
            // 监听技能释放后通用逻辑事件
            _context.GetEventBus().AddListener<PostCastEvent>(OnPostCastDispatch);
            // 监听技能释放后通用逻辑事件
            _context.GetEventBus().AddListener<UltimateCastEvent>(OnUltimateCastDispatch);
            // 监听更新等待队列事件
            _context.GetEventBus().AddListener<UpdateWaitCmdEvent>(OnUpdateWaitCmdDispatch);
        }

        /// <summary>
        /// 终结技释放前调度逻辑
        /// </summary>
        /// <param name="caster"></param>
        /// <param name="skillInfo"></param>
        public IEnumerator PreUltimateCastDispatch(IBattleEntityObject caster, SkillInfo skillInfo)
        {
            // 先执行战斗点位置变化
            _context.GetProxy().UpdateMonsterPos(caster);
            // 更新相机显示
            yield return TaskUtility.WaitForTask(_context.GetProxy().UpdateCamera(caster));
            // 玩家回合：激活目标选择功能
            DIContainer.GetInstance<ITargetSelectManager>().ActiveSelectTarget();
            // 启用输入
            DIContainer.GetInstance<IBattleInputHandler>().SetInputState(true);
            // 更新UI
            var controller = _uiService.GetPanel(EUIPanelId.BattlePanel) as IBattleController;
            // 隐藏行动提示
            controller.BattleUiManager.SetActTipActive(E_ActTipType.Hide);
            // 激活怪物血量UI显示
            controller.MonsterStateUIManager.ActiveMonsterUIs();
            // 显示终结技立绘
            yield return controller.BattleUiManager.ShowPaiting((caster as PlayerObject)?.RoleInfo, skillInfo);
            // 获取终结技技能按键UI数据提供器
            var provider = _skillKeyUIDataProviderFactory.GetCastSkillCondition<UltimateSkillKeyUIDataProvider>();
            // 根据数据更新玩家操作按键，按键触发技能选择事件
            controller.BattleUiManager.UpdateOperator(caster, provider);
        }

        /// <summary>
        /// 技能释放后处理逻辑
        /// </summary>
        /// <param name="postCastEvent"></param>
        private void OnPostCastDispatch(PostCastEvent postCastEvent)
        {
            var controller = _uiService.GetPanel(EUIPanelId.BattlePanel) as IBattleController;
            // 更新累计伤害UI
            controller.BattleUiManager.UpdateCumulativeDamage(false, 0);
        }

        /// <summary>
        /// 终结技释放调度逻辑
        /// 处理终结释放时的通用逻辑
        /// </summary>
        private void OnUltimateCastDispatch(UltimateCastEvent ultimateCastEvent)
        {
            // 关闭目标选择（终结技释放时不再允许手动选择目标）
            DIContainer.GetInstance<ITargetSelectManager>().InActiveSelectTarget();
            // 禁用输入
            DIContainer.GetInstance<IBattleInputHandler>().SetInputState(false);
            var controller = _uiService.GetPanel(EUIPanelId.BattlePanel) as IBattleController;
            controller.BattleUiManager.ClearSelectMarker();
            controller.BattleUiManager.ClearOperator();
            controller.BattleUiManager.SetActTipActive(E_ActTipType.Hide);
        }

        /// <summary>
        /// 更新等待队列事件回调
        /// </summary>
        /// <param name="updateWaitCmdEvent"></param>
        private void OnUpdateWaitCmdDispatch(UpdateWaitCmdEvent updateWaitCmdEvent)
        {
            var controller = _uiService.GetPanel(EUIPanelId.BattlePanel) as IBattleController;
            controller.BattleUiManager.UpdateWaitingCommmand(updateWaitCmdEvent.BattleEntities);
        }
        
        /// <summary>
        /// 回合开始事件调度逻辑
        /// </summary>
        /// <param name="turnStartEvent"></param>
        private async void OnTurnStartDispatch(TurnStartEvent turnStartEvent)
        {
            if (turnStartEvent.CurrentBattleEntity == null)
            {
                Logger.LogError($"{nameof(BattleEventScheduler)}.{nameof(OnTurnStartDispatch)}：当前战斗对象为null");
                return;
            }

            if (turnStartEvent.CurrentBattleEntity is PlayerObject)
            {
                // 先执行战斗点位置变化
                _context.GetProxy().UpdateMonsterPos(turnStartEvent.CurrentBattleEntity);
                // 更新相机显示
                await _context.GetProxy().UpdateCamera(turnStartEvent.CurrentBattleEntity);
            }
            
            var controller = _uiService.GetPanel(EUIPanelId.BattlePanel) as IBattleController;
            switch (turnStartEvent.CurrentBattleEntity)
            {
                case PlayerObject:
                {
                    // 角色行动才激活怪物UI显示
                    controller.MonsterStateUIManager.ActiveMonsterUIs();
                    // 启用输入
                    DIContainer.GetInstance<IBattleInputHandler>().SetInputState(true);
                    // 玩家回合：激活目标选择功能
                    DIContainer.GetInstance<ITargetSelectManager>().ActiveSelectTarget();
                    // 隐藏行动提示
                    controller.BattleUiManager.SetActTipActive(E_ActTipType.Hide);
                    // 获取技能按键UI数据提供器
                    var provider = _skillKeyUIDataProviderFactory.GetCastSkillCondition<BaseSkillKeyUIDataProvider>();
                    // 根据数据更新玩家操作按键，按键触发技能选择事件
                    controller.BattleUiManager.UpdateOperator(turnStartEvent.CurrentBattleEntity, provider);
                    break;
                }
                case MonsterObject:
                    // 怪物回合：关闭目标选择功能
                    DIContainer.GetInstance<ITargetSelectManager>().InActiveSelectTarget();
                    // 清除选中目标的标记UI
                    controller.BattleUiManager.ClearSelectMarker();
                    // 清空操作面板
                    controller.BattleUiManager.ClearOperator();
                    // 显示怪物行动提示
                    controller.BattleUiManager.SetActTipActive(E_ActTipType.Monster);
                    break;
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
                {
                    return;
                }
            
                // 读取技能信息
                var skillInfo = DIContainer.GetInstance<IBinaryDataManager>().GetConfig<SkillInfoContainer>(EConfigLoadType.Excel)
                    .dataDic[selectSkillEvent.SkillId];
                // 获取技能目标类型
                var skillTargetType = (E_SkillTargetType)skillInfo.f_SkillTargetType;
                switch (skillTargetType)
                {
                    case E_SkillTargetType.None:
                        Logger.LogError($"{nameof(BattleEventScheduler)}.{nameof(SelectSkillEventScheduler)}:无效的目标类型，{skillTargetType}");
                        break;
                    case E_SkillTargetType.Friend:
                        // 失活所有怪物UI显示
                        (_uiService.GetPanel(EUIPanelId.BattlePanel) as IBattleController).MonsterStateUIManager.InActiveMonsterUIs();
                        // 更新相机看向玩家
                        // TODO：计算相机世界坐标的位置和看向，数据暂时写死
                        var worldPos = new Vector3(0, 1, 1.7f);
                        var rotation = Quaternion.Euler(0, 180, 0);
                        // 获取遮罩
                        var mask = LayerGeter.GetRoleBitLayer() | LayerGeter.GetPreBitLayer();
                        // 创建相机
                        await DIContainer.GetInstance<IBattleCameraManager>().CreateCamera(null, worldPos, rotation, mask);
                        break;
                    case E_SkillTargetType.Enemy:
                        // 激活所有怪物UI显示
                        (_uiService.GetPanel(EUIPanelId.BattlePanel) as IBattleController).MonsterStateUIManager.ActiveMonsterUIs();
                        // 更新相机看向怪物
                        var roleCameraParent = DIContainer.GetInstance<IBattlePointProxy>().BattlePoint
                            .GetRoleCameraTransByIndex(playerObject.EntityPosIndex);
                        // 设置Mask
                        var mask2 = LayerGeter.GetPreBitLayer() | LayerGeter.GetMonsterBitLayer();
                        // 根据当前玩家位置索引，只渲染符合的角色
                        var roleLayers = LayerGeter.GetRoleLayers();
                        for (var i = playerObject.EntityPosIndex; i < roleLayers.Length; i++)
                        {
                            mask2 |= 1 << roleLayers[i];
                        }
                        await DIContainer.GetInstance<IBattleCameraManager>().CreateCamera(roleCameraParent, Vector3.zero, Quaternion.identity, mask2);
                        break;
                    default:
                        return;
                }
            }
            catch (Exception e)
            {
                Logger.LogError($"{nameof(BattleEventScheduler)}.{nameof(SelectSkillEventScheduler)}:逻辑执行错误，{e.Message}");
            }
        }

        public void OnDestroy()
        {
            // 监听回合开始事件
            _context.GetEventBus().RemoveListener<TurnStartEvent>(OnTurnStartDispatch);
            // 监听角色技能选择事件
            _context.GetEventBus().RemoveListener<SelectSkillEvent>(SelectSkillEventScheduler);
            // 监听技能释放后通用逻辑事件
            _context.GetEventBus().RemoveListener<PostCastEvent>(OnPostCastDispatch);
            // 监听技能释放后通用逻辑事件
            _context.GetEventBus().RemoveListener<UltimateCastEvent>(OnUltimateCastDispatch);
            // 监听更新等待队列事件
            _context.GetEventBus().RemoveListener<UpdateWaitCmdEvent>(OnUpdateWaitCmdDispatch);
            _context = null;
        }
    }
}
