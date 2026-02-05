using System.Collections.Generic;
using Core.Components;
using Game.Battle.Component;
using Game.Battle.Event;
using Game.Battle.Objects;
using Game.Battle.Status;
using Game.Battle.Status.Data;
using Game.Battle.Status.Enum;
using GameHotUpdate.Battle.Event.Turn;
using GameHotUpdate.Battle.Event.UI;

namespace GameHotUpdate.Status
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
            // 监听回合开始事件
            //battleEntity.Context.GetEventBus().AddListener<TurnStartEvent>(OnTurnStart);
            // 监听回合结束事件
            //battleEntity.Context.GetEventBus().AddListener<TurnEndEvent>(OnTurnEnd);
        }

        // /// <summary>
        // /// 回合开始事件处理
        // /// </summary>
        // /// <param name="turnStartEvent">回合开始事件</param>
        // private void OnTurnStart(TurnStartEvent turnStartEvent)
        // {
        //     OnTurnChanged(turnStartEvent);
        // }

        // /// <summary>
        // /// 回合结束事件处理
        // /// </summary>
        // /// <param name="turnEndEvent">回合结束事件</param>
        // private void OnTurnEnd(TurnEndEvent turnEndEvent)
        // {
        //     OnTurnChanged(turnEndEvent);
        // }

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
        }

        /// <summary>
        /// 回合变更处理（开始或结束）
        /// </summary>
        /// <param name="battleEvent">回合事件</param>
        private void OnTurnChanged(BattleEvent battleEvent)
        {
            // 只处理当前实体的回合事件
            if (battleEvent.Context.GetCurrentEntity() != BattleEntity)
            {
                return;
            }

            // if (battleEvent is TurnStartEvent turnStartEvent)
            // {
            //     // 处理所有有效状态的回合开始逻辑
            //     foreach (IStatus status in _statuses)
            //     {
            //         if (status.IsValid)
            //         {
            //             status.TurnStart(BattleEntity, turnStartEvent.Context);
            //         }
            //     }
            // }
            // else if(battleEvent is TurnEndEvent turnEndEvent)
            // {
            //     // 处理所有有效状态的回合结束逻辑
            //     foreach (IStatus status in _statuses)
            //     {
            //         if (status.IsValid)
            //         {
            //             status.TurnEnd(BattleEntity, turnEndEvent.Context);
            //         }
            //     }
            // }

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
        /// 移除状态
        /// </summary>
        /// <param name="removalStrategy">移除策略</param>
        public void RemoveStatus(IStatusRemovalStrategy removalStrategy)
        {
            removalStrategy.HandleRemove(_statuses);
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