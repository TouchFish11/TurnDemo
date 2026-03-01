using System.Collections;
using Core.Log;
using Core.Reflection;
using Core.Serialize.Binary;
using Core.Service;
using Core.Singleton;
using Core.UI;
using Core.Utility;
using GameHotUpdate.Battle.Context;
using GameHotUpdate.Battle.Event.General;
using GameHotUpdate.Battle.Event.Skill;
using GameHotUpdate.Battle.Event.Turn;
using GameHotUpdate.Battle.Event.UI;
using GameHotUpdate.Battle.Input;
using GameHotUpdate.Battle.Layer;
using GameHotUpdate.Battle.Object;
using GameHotUpdate.Battle.Point;
using GameHotUpdate.Battle.Skill.Enum;
using GameHotUpdate.Battle.TargetSelect;
using GameHotUpdate.Battle.UI.Base;
using GameHotUpdate.Battle.UI.SkillKey;
using GameHotUpdate.Battle.UI.SkillKey.Provider;
using GameHotUpdate.Camera;
using UnityEngine;

namespace GameHotUpdate.Battle.Event
{
    /// <summary>
    /// 战斗事件逻辑调度器
    /// 监听战斗事件，执行其它模块的统一调用
    /// </summary>
    public class BattleEventScheduler : SingletonAutoMono<BattleEventScheduler>, IBattleEventScheduler
    {
        private IBattleContext _context;

        public GameObject GameObject { get; private set; }

        private void Awake()
        {
            GameObject = gameObject;
        }

        public void Init(IBattleContext context)
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
            ServiceLocator.Get<ITargetSelectManager>().ActiveSelectTarget();
            // 启用输入
            ServiceLocator.Get<IBattleInputHandler>().SetInputState(true);
            // 更新UI
            var controller = ServiceLocator.Get<IUIManager>().GetController<BattleController>();
            // 隐藏行动提示
            controller.BattleUiManager.SetActTipActive(E_ActTipType.Hide);
            // 激活怪物血量UI显示
            controller.MonsterStateUIManager.ActiveMonsterUIs();
            // 显示终结技立绘
            yield return controller.BattleUiManager.ShowPaiting((caster as PlayerObject)?.RoleInfo, skillInfo);
            // 获取终结技技能按键UI数据提供器
            var provider = ServiceLocator.Get<IFactoryManager>()
                .GetFactory<ISkillKeyUIDataProviderFactory, SkillKeyUIDataProviderFactory>()
                .GetCastSkillCondition<UltimateSkillKeyUIDataProvider>();
            // 根据数据更新玩家操作按键，按键触发技能选择事件
            controller.BattleUiManager.UpdateOperator(caster, provider);
        }

        /// <summary>
        /// 技能释放后处理逻辑
        /// </summary>
        /// <param name="postCastEvent"></param>
        private void OnPostCastDispatch(PostCastEvent postCastEvent)
        {
            var controller = ServiceLocator.Get<IUIManager>().GetController<BattleController>();
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
            ServiceLocator.Get<ITargetSelectManager>().InActiveSelectTarget();
            // 禁用输入
            ServiceLocator.Get<IBattleInputHandler>().SetInputState(false);
            var controller = ServiceLocator.Get<IUIManager>().GetController<BattleController>();
            controller.BattleUiManager.ClearSelectMarker();
            controller.BattleUiManager.SetOperator(null);
            controller.BattleUiManager.SetActTipActive(E_ActTipType.Hide);
        }

        /// <summary>
        /// 更新等待队列事件回调
        /// </summary>
        /// <param name="updateWaitCmdEvent"></param>
        private void OnUpdateWaitCmdDispatch(UpdateWaitCmdEvent updateWaitCmdEvent)
        {
            var controller = ServiceLocator.Get<IUIManager>().GetController<BattleController>();
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
                LogManager.LogError($"{nameof(BattleEventScheduler)}.{nameof(OnTurnStartDispatch)}：当前战斗对象为null");
                return;
            }

            if (turnStartEvent.CurrentBattleEntity is PlayerObject)
            {
                // 先执行战斗点位置变化
                _context.GetProxy().UpdateMonsterPos(turnStartEvent.CurrentBattleEntity);
                // 更新相机显示
                await _context.GetProxy().UpdateCamera(turnStartEvent.CurrentBattleEntity);
            }
            
            var controller = ServiceLocator.Get<IUIManager>().GetController<BattleController>();
            switch (turnStartEvent.CurrentBattleEntity)
            {
                case PlayerObject:
                {
                    // 角色行动才激活怪物UI显示
                    controller.MonsterStateUIManager.ActiveMonsterUIs();
                    // 启用输入
                    ServiceLocator.Get<IBattleInputHandler>().SetInputState(true);
                    // 玩家回合：激活目标选择功能
                    ServiceLocator.Get<ITargetSelectManager>().ActiveSelectTarget();
                    // 隐藏行动提示
                    controller.BattleUiManager.SetActTipActive(E_ActTipType.Hide);
                    // 获取技能按键UI数据提供器
                    var provider = ServiceLocator.Get<IFactoryManager>().
                        GetFactory<ISkillKeyUIDataProviderFactory, SkillKeyUIDataProviderFactory>().
                        GetCastSkillCondition<BaseSkillKeyUIDataProvider>();
                    
                    // 根据数据更新玩家操作按键，按键触发技能选择事件
                    controller.BattleUiManager.UpdateOperator(turnStartEvent.CurrentBattleEntity, provider);
                    break;
                }
                case MonsterObject:
                    // 怪物回合：关闭目标选择功能
                    ServiceLocator.Get<ITargetSelectManager>().InActiveSelectTarget();
                    // 清除选中目标的标记UI
                    controller.BattleUiManager.ClearSelectMarker();
                    // 清空操作面板
                    controller.BattleUiManager.SetOperator(null);
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
            if (selectSkillEvent.Caster is not PlayerObject playerObject)
            {
                return;
            }
            
            // 读取技能信息
            var skillInfo = ServiceLocator.Get<IBinaryDataManager>().GetConfig<SkillInfoContainer>(EConfigLoadType.Excel)
                .dataDic[selectSkillEvent.SkillId];
            // 获取技能目标类型
            var skillTargetType = (E_SkillTargetType)skillInfo.f_SkillTargetType;
            switch (skillTargetType)
            {
                case E_SkillTargetType.None:
                    LogManager.LogError($"{nameof(BattleEventScheduler)}.{nameof(SelectSkillEventScheduler)}：无效的目标类型，{skillTargetType}");
                    break;
                case E_SkillTargetType.Friend:
                    // 更新相机看向玩家
                    // TODO：计算相机世界坐标的位置和看向，数据暂时写死
                    var worldPos = new Vector3(0, 1, 1.7f);
                    var rotation = Quaternion.Euler(0, 180, 0);
                    // 获取遮罩
                    var mask = LayerGeter.GetRoleBitLayer() | LayerGeter.GetPreBitLayer();
                    // 创建相机
                    await ServiceLocator.Get<IBattleCameraManager>().CreateCamera(null, worldPos, rotation, mask);
                    break;
                case E_SkillTargetType.Enemy:
                    // 更新相机看向怪物
                    var roleCameraParent = ServiceLocator.Get<IBattlePointProxy>().BattlePoint
                        .GetRoleCameraTransByIndex(playerObject.EntityPosIndex);
                    // 设置Mask
                    var mask2 = LayerGeter.GetPreBitLayer() | LayerGeter.GetMonsterBitLayer();
                    // 根据当前玩家位置索引，只渲染符合的角色
                    var roleLayers = LayerGeter.GetRoleLayers();
                    for (var i = playerObject.EntityPosIndex; i < roleLayers.Length; i++)
                    {
                        mask2 |= 1 << roleLayers[i];
                    }
                    await ServiceLocator.Get<IBattleCameraManager>().CreateCamera(roleCameraParent, Vector3.zero, Quaternion.identity, mask2);
                    break;
                default:
                    return;
            }
        }
    }
}
