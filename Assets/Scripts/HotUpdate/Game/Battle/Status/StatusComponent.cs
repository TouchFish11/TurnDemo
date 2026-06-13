using System.Collections.Generic;
using Core.Components;
using HotUpdate.Base;
using HotUpdate.Game.Battle.Core;
using HotUpdate.Game.Battle.Event.UI;
using HotUpdate.Game.Battle.Object;
using HotUpdate.Game.Battle.Status.Data;
using HotUpdate.Game.Battle.Status.Enum;

namespace HotUpdate.Game.Battle.Status
{
    /// <summary>
    /// 状态组件 - 管理实体的所有状态效果（Buff/Debuff）
    /// 负责状态的添加、移除、回合结算和数值加成计算
    /// </summary>
    [ComponentId(typeof(StatusComponent))]
    public class StatusComponent : BattleComponent, IStatusComponent
    {
        // 当前生效的状态列表
        private readonly List<IStatus> _statuses = new();
        // 状态总加成数据（攻击/防御/生命等）
        private StatusTotalBonusData statusTotalBonus;

        /// <summary>
        /// 战斗初始化
        /// </summary>
        public override void BattleInit(IBattleEntityObject battleEntity)
        {
            base.BattleInit(battleEntity);

            statusTotalBonus = new StatusTotalBonusData();
        }

        public List<IStatus> GetStatuses()
        {
            return _statuses;
        }

        /// <summary>
        /// 更新状态
        /// </summary>
        public void UpdateStatus()
        {
            // 处理所有有效状态的回合开始逻辑
            foreach (var status in _statuses)
            {
                if (status.IsValid)
                {
                    status.TurnStart(BattleEntity, BattleEntity.Context);
                }
            }
            
            // 移除已失效的状态
            _statuses.RemoveAll(s => !s.IsValid);
            // 更新状态加成数据
            UpdateStatusBonus();
            // 通知UI状态发生变更
            BattleEntity.Context.GetEventBus().TriggerEvent(new TurnStartStatusChangedEvent(BattleEntity.Context, BattleEntity));
        }
        
        /// <summary>
        /// 添加状态
        /// </summary>
        /// <param name="status">要添加的状态</param>
        public void AddStatus(IStatus status)
        {
            // 根据冲突类型处理状态添加
            switch ((E_ConflictType)status.StatusProperty.StatusInfo.f_conflictType)
            {
                case E_ConflictType.Add:     // 叠加类型
                    OnConflict_Add(status);
                    break;
                case E_ConflictType.Lonely:  // 独立类型（可重复存在）
                    OnConflict_Lonel(status);
                    break;
                case E_ConflictType.Cover:   // 覆盖类型（新状态覆盖旧状态）
                    OnConflict_Cover(status);
                    break;
            }

            // 更新状态加成
            UpdateStatusBonus();
            // 触发状态添加事件
            BattleEntity.Context.GetEventBus().TriggerEvent(new StatusAddedEvent(BattleEntity.Context, status));
        }

        /// <summary>
        /// 更新状态加成数据
        /// </summary>
        private void UpdateStatusBonus()
        {
            // 更新所有状态的攻击、防御、生命加成
            statusTotalBonus.UpdateTotalAtkBonus(_statuses);
            statusTotalBonus.UpdateTotalDefBonus(_statuses);
            statusTotalBonus.UpdateTotalHpBonus(_statuses);
        }

        /// <summary>
        /// 处理叠加类型的状态冲突
        /// </summary>
        /// <param name="newStatus">新状态</param>
        private void OnConflict_Add(IStatus newStatus)
        {
            // 查找是否已存在相同ID的状态
            if (this.TryGetStatus(newStatus.StatusProperty.StatusInfo.f_id, _statuses, out IStatus status))
            {
                // 存在则叠加层数
                status.ChangePine(1);
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
        private void OnConflict_Lonel(IStatus newStatus)
        {
            newStatus.IsValid = true;
            _statuses.Add(newStatus);
        }

        /// <summary>
        /// 处理覆盖类型的状态冲突
        /// </summary>
        /// <param name="newStatus">新状态</param>
        private void OnConflict_Cover(IStatus newStatus)
        {
            // 查找是否已存在相同ID的状态
            if (this.TryGetStatus(newStatus.StatusProperty.StatusInfo.f_id, _statuses, out IStatus status))
            {
                // 存在则移除旧状态，添加新状态
                status.IsValid = false;
                _statuses.Remove(status);
                newStatus.IsValid = true;
                _statuses.Add(newStatus);
            }
        }
    }
}