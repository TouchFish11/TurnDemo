using System.Collections.Generic;
using Core.Exceptions;
using HotUpdate.Base.ECModule;
using HotUpdate.Game.Battle.Core;
using HotUpdate.Game.Battle.Event.UI;

namespace HotUpdate.Game.Battle.Statuses
{
    /// <summary>
    /// 状态组件 - 管理实体的所有状态效果（Buff/Debuff）
    /// 负责状态的添加、移除、回合结算
    /// </summary>
    [ComponentId]
    public class StatusComponent : BattleComponent
    {
        // 当前生效的状态列表
        private List<IStatus> _statuses = new();

        public IEnumerable<IStatus> GetStatuses()
        {
            foreach (var statuse in _statuses)
            {
                yield return statuse;
            }
        }
        
        /// <summary>
        /// 回合开始时buff的结算
        /// </summary>
        public void SettlementTurnStart()
        {
            UpdateStatus();
            
            foreach (var status in _statuses)
            {
                status.TurnStart(BattleEntity, BattleEntity.Context);
            }
            
            // 移除已失效的状态
            _statuses.RemoveAll(s => !s.IsValid);
            // 通知UI状态发生变更
            BattleEntity.Context.EventBus.TriggerEvent(new TurnStartStatusChangedEvent(BattleEntity.Context, BattleEntity));
        }

        /// <summary>
        /// 更新所有没有参与结算的buff状态为激活（参与结算）状态
        /// </summary>
        private void UpdateStatus()
        {
            foreach (var status in _statuses)
            {
                if (status.StatusState != EStatusState.Active)
                {
                    status.EnableActive();
                }
            }
        }
        
        /// <summary>
        /// 回合结束时buff的结算
        /// </summary>
        public void SettlementTurnEnd()
        {
            // 处理所有已经激活的状态
            foreach (var status in _statuses)
            {
                if (status.StatusState == EStatusState.Active)
                {
                    status.TurnEnd(BattleEntity, BattleEntity.Context);
                }
            }
            
            // 移除已失效的状态
            _statuses.RemoveAll(s => !s.IsValid);
            // 通知UI状态发生变更
            BattleEntity.Context.EventBus.TriggerEvent(new TurnStartStatusChangedEvent(BattleEntity.Context, BattleEntity));
        }

        /// <summary>
        /// 尝试获取状态
        /// </summary>
        /// <param name="statusId">状态ID</param>
        /// <param name="status">输出状态</param>
        /// <returns>独立存在的状态无法准确找到某一个，不存在则返回null</returns>
        public bool TryGetStatus(int statusId, out IStatus status)
        {
            foreach (var cacheStatus in _statuses)
            {
                if (cacheStatus.StatusInfo.f_id != statusId) 
                    continue;
                
                status = cacheStatus;
                return true;
            }

            status = null;
            return false;
        }
        
        /// <summary>
        /// 添加状态
        /// </summary>
        /// <param name="status">要添加的状态</param>
        public void AddStatus(IStatus status)
        {
            // 根据冲突类型处理状态添加
            switch ((EConflictType)status.StatusInfo.f_conflictType)
            {
                case EConflictType.Add:     // 叠加类型
                    AddOnConflict(status);
                    break;
                case EConflictType.Lonely:  // 独立类型（可重复存在）
                    LonelOnConflict(status);
                    break;
                case EConflictType.Cover:   // 覆盖类型（新状态覆盖旧状态）
                    CoverOnConflict(status);
                    break;
                default:
                    throw ExceptionHelper.Throw($"Unknown conflict type({status})");
            }

            // 触发状态添加事件
            BattleEntity.Context.EventBus.TriggerEvent(new StatusAddedEvent(BattleEntity.Context, status));
        }

        /// <summary>
        /// 处理叠加类型的状态冲突
        /// </summary>
        /// <param name="newStatus">新状态</param>
        private void AddOnConflict(IStatus newStatus)
        {
            // 查找是否已存在相同ID的状态
            if (TryGetStatus(newStatus.StatusInfo.f_id, out var status))
            {
                // 存在则叠加层数
                status.ChangePine(newStatus.StatusInfo.f_startPine);
            }
            else
            {
                // 不存在则添加新状态
                newStatus.IsValid = true;
                _statuses.Add(newStatus);
            }
        }

        /// <summary>
        /// 处理独立类型的状态冲突（可重复存在）
        /// </summary>
        /// <param name="newStatus">新状态</param>
        private void LonelOnConflict(IStatus newStatus)
        {
            newStatus.IsValid = true;
            _statuses.Add(newStatus);
        }

        /// <summary>
        /// 处理覆盖类型的状态冲突
        /// </summary>
        /// <param name="newStatus">新状态</param>
        private void CoverOnConflict(IStatus newStatus)
        {
            // 查找是否已存在相同ID的状态
            if (!TryGetStatus(newStatus.StatusInfo.f_id, out var status)) 
                return;
            
            // 存在则移除旧状态，添加新状态
            status.IsValid = false;
            _statuses.Remove(status);
            newStatus.IsValid = true;
            _statuses.Add(newStatus);
        }
        
        protected override void OnBattleDestroy()
        {
            _statuses.Clear();
            _statuses = null;
        }
    }
}