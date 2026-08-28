using System;
using System.Collections.Generic;
using Core.GlobalEvent;
using Core.GlobalEvent.Events.ViewOperation;
using Core.Log;
using HotUpdate.Game.Battle.Context;
using HotUpdate.Game.Battle.Core;
using HotUpdate.Game.Battle.Event.UI;
using HotUpdate.Game.Battle.Layer;
using HotUpdate.Game.Battle.Object;
using HotUpdate.Game.Battle.Skill;

namespace HotUpdate.Game.Battle.TargetSelect
{
    /// <summary>
    /// 目标选择流程
    /// 将目标选择的流程独立出一个逻辑类，由协调器调用
    /// </summary>
    public class TargetSelectFlow
    {
        private readonly ITargetSelectManager _targetSelectManager;
        private readonly IBattleCameraManager _battleCameraManager;
        private readonly IEventCenter _eventCenter;
        private OperationState _operationState;
        private IBattleContext _battleContext;

        private TargetSelectFlow(IEventCenter eventCenter, ITargetSelectManager targetSelectManager, IBattleCameraManager battleCameraManager)
        {
            eventCenter.SubscribeEvent<ViewLeftDragEvent>(OnLeftDrag);   // 左拖拽：切换上一个主目标
            eventCenter.SubscribeEvent<ViewRightDragEvent>(OnRightDrag);     // 右拖拽：切换下一个主目标
            eventCenter.SubscribeEvent<ViewClickEvent>(OnClick);
            _eventCenter = eventCenter;
            _targetSelectManager = targetSelectManager;
            _battleCameraManager = battleCameraManager;
        }

        public void Init(IBattleContext battleContext, OperationState operationState)
        {
            _battleContext = battleContext;
            _operationState = operationState;
        }
        
        private void OnLeftDrag(ViewLeftDragEvent e)
        {
            if(!_operationState.IsActiveTargetSelect)
                return;
            
            // 选择上一个目标
            _targetSelectManager.SelectPreviousMainTarget();
            _targetSelectManager.SelectAllTargets(_operationState.CurrentSkillInfo.f_skillRangeType);
            var newMainTarget = _targetSelectManager.GetMainTarget();
            var newAllTarget = _targetSelectManager.GetTargets();
            // 满足条件才去更新UI相关逻辑
            if (CanUpdateTargetSelect(newMainTarget, newAllTarget))
            {
                _operationState.LastTarget = newMainTarget;
                _operationState.LastTargets = newAllTarget;
                // 触发目标选择变更事件，通知UI更新选中状态
                _battleContext.EventBus.TriggerEvent(new SelectTargetEvent(_battleContext, _battleContext.CurrentCommand?.Sender ?? _battleContext.CurrentTurnOwner, _targetSelectManager.GetMainTarget(), _targetSelectManager.GetTargets()));
                // 更新相机选择基准
                _battleCameraManager.UpdateBaseRotation(); 
            }
        }
        
        private void OnRightDrag(ViewRightDragEvent e)
        {
            if(!_operationState.IsActiveTargetSelect)
                return;
            
            // 选择下一个目标
            _targetSelectManager.SelectNextMainTarget();
            _targetSelectManager.SelectAllTargets(_operationState.CurrentSkillInfo.f_skillRangeType);
            var newMainTarget = _targetSelectManager.GetMainTarget();
            var newAllTarget = _targetSelectManager.GetTargets();
            // 满足条件才去更新UI相关逻辑
            if (CanUpdateTargetSelect(newMainTarget, newAllTarget))
            {
                _operationState.LastTarget = newMainTarget;
                _operationState.LastTargets = newAllTarget;
                // 触发目标选择变更事件，通知UI更新选中状态
                _battleContext.EventBus.TriggerEvent(new SelectTargetEvent(_battleContext, _battleContext.CurrentCommand?.Sender ?? _battleContext.CurrentTurnOwner, _targetSelectManager.GetMainTarget(), _targetSelectManager.GetTargets()));
                // 更新相机选择基准
                _battleCameraManager.UpdateBaseRotation();
            }
        }
        
        private void OnClick(ViewClickEvent e)
        {
            if(!_operationState.IsActiveTargetSelect)
                return;
            
            // 执行相机进行射线检测
            // 根据选中的技能ID获取技能配置信息
            // 将技能范围类型转换为技能目标类型（友方/敌方）
            var targetType = (E_SkillTargetType)_operationState.CurrentSkillInfo.f_SkillTargetType;

            // 根据技能目标类型设置射线检测的层级掩码（只检测对应层级的对象）
            int layerMask;
            switch (targetType)
            {
                case E_SkillTargetType.Friend:
                    // 检测玩家对象层级
                    layerMask = LayerGeter.GetRoleBitLayer();
                    break;
                case E_SkillTargetType.Enemy:
                    // 检测怪物对象层级
                    layerMask = LayerGeter.GetMonsterBitLayer();
                    break;
                case E_SkillTargetType.None:
                default:
                    Logger.LogWarning(ELogTags.Battle, $"未处理的技能目标类型：{targetType}");
                    return;
            }
            
            var hitObj = _battleCameraManager.RayCast(layerMask);
            if (hitObj is BattleObject battleObject)
            {
                // 根据点击到的目标作为主目标
                _targetSelectManager.SelectMainTarget(battleObject);
                _targetSelectManager.SelectAllTargets(_operationState.CurrentSkillInfo.f_skillRangeType);
                var allTargets = _targetSelectManager.GetTargets();
                if (CanUpdateTargetSelect(battleObject, allTargets))
                {
                    _operationState.LastTarget = battleObject;
                    _operationState.LastTargets = allTargets;
                    // 触发目标选择变更事件，通知UI更新选中状态
                    _battleContext.EventBus.TriggerEvent(new SelectTargetEvent(_battleContext, _battleContext.CurrentCommand?.Sender ?? _battleContext.CurrentTurnOwner, _targetSelectManager.GetMainTarget(), _targetSelectManager.GetTargets()));
                }
            }
        }
        
        /// <summary>
        /// 能否更新目标目标标记UI
        /// </summary>
        /// <param name="mainTarget"></param>
        /// <param name="allTargets"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private bool CanUpdateTargetSelect(IBattleEntityObject mainTarget, List<IBattleEntityObject> allTargets)
        {
            return (E_SkillRangeType)_operationState.CurrentSkillInfo.f_skillRangeType switch
            {
                E_SkillRangeType.Single => _operationState.LastTarget == null || _operationState.LastTarget != mainTarget,
                E_SkillRangeType.Diffusion => _operationState.LastTarget == null || _operationState.LastTarget != mainTarget || !IsSameTargets(allTargets),
                E_SkillRangeType.All => !IsSameTargets(allTargets),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
        
        /// <summary>
        /// 目标列表是否相同
        /// </summary>
        /// <param name="newTargets"></param>
        /// <returns></returns>
        private bool IsSameTargets(List<IBattleEntityObject> newTargets)
        {
            if (_operationState.LastTargets == null)
            {
                _operationState.LastTargets = newTargets;
                return false;
            }
            
            foreach (var battleEntityObject in newTargets)
            {
                if (!_operationState.LastTargets.Contains(battleEntityObject))
                {
                    return false;
                }
            }

            return true;
        }

        public void Reset()
        {
            _eventCenter.UnsubscribeEvent<ViewLeftDragEvent>(OnLeftDrag);
            _eventCenter.UnsubscribeEvent<ViewRightDragEvent>(OnRightDrag);
            _eventCenter.UnsubscribeEvent<ViewClickEvent>(OnClick);
        }
    }
}
