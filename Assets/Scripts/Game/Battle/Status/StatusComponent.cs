using Framework;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Battle
{
    /// <summary>
    /// 状态管理组件
    /// 角色的状态容器，负责管理所有状态
    /// </summary>
    [ComponentId(nameof(StatusComponent))]
    public class StatusComponent : BattleComponent, IStatusComponent
    {
        // 状态列表
        private readonly List<IStatus> _statuses = new List<IStatus>();
        // 状态总加成数据
        private StatusTotalBonusData statusTotalBonus;

        public override void BattleInit(IBattleEntityObject battleEntity)
        {
            base.BattleInit(battleEntity);

            statusTotalBonus = new StatusTotalBonusData();
            // 订阅回合开始事件
            battleEntity.Context.GetEventBus().AddListener<TurnStartEvent>(OnTurnStart);
            // 订阅回合结束事件
            battleEntity.Context.GetEventBus().AddListener<TurnEndEvent>(OnTurnEnd);
        }

        /// <summary>
        /// 回合开始事件回调
        /// </summary>
        /// <param name="turnStartEvent"></param>
        private void OnTurnStart(TurnStartEvent turnStartEvent)
        {
            OnTurnChanged(turnStartEvent);
        }

        /// <summary>
        /// 回合结束事件回调
        /// </summary>
        /// <param name="turnEndEvent"></param>
        private void OnTurnEnd(TurnEndEvent turnEndEvent)
        {
            OnTurnChanged(turnEndEvent);
        }

        /// <summary>
        /// 回合变化逻辑
        /// </summary>
        /// <param name="battleEvent"></param>
        private void OnTurnChanged(BattleEvent battleEvent)
        {
            if (battleEvent.Context.GetCurrentEntity() != this.BattleEntity)
            {
                return;
            }

            if (battleEvent is TurnStartEvent turnStartEvent)
            {
                // 遍历所有有效状态，调用其TurnStart
                foreach (IStatus status in _statuses)
                {
                    if (status.IsValid)
                    {
                        status.TurnStart(BattleEntity, turnStartEvent.Context);
                    }
                }
            }
            else if(battleEvent is TurnEndEvent turnEndEvent)
            {
                // 遍历所有有效状态，调用其TurnEnd
                foreach (IStatus status in _statuses)
                {
                    if (status.IsValid)
                    {
                        status.TurnEnd(BattleEntity, turnEndEvent.Context);
                    }
                }
            }

            // 移除失效状态
            _statuses.RemoveAll(s => !s.IsValid);
            // 更新状态加成
            UpdateStatusBonus();
            // 更新角色UI状态栏
            this.BattleEntity.Context.GetEventBus().TriggerEvent(new TurnStartStatusChangedEvent(this.BattleEntity.Context, this.BattleEntity));
        }

        /// <summary>
        /// 添加状态
        /// </summary>
        /// <param name="status"></param>
        public void AddStatus(IStatus status)
        {
            // 处理冲突
            switch ((E_ConflictType)status.StatusProperty.StatusInfo.f_conflictType)
            {
                case E_ConflictType.Add:
                    OnConflict_Add(status);
                    break;
                case E_ConflictType.Lonel:
                    OnConflict_Lonel(status);
                    break;
                case E_ConflictType.Cover:
                    OnConflict_Cover(status);
                    break;
            }

            // 更新状态加成
            UpdateStatusBonus();
            // 触发事件，更新状态浮动文本UI
            this.BattleEntity.Context.GetEventBus().TriggerEvent(new StatusAddedEvent(this.BattleEntity.Context, status));
        }

        /// <summary>
        /// 移除状态
        /// </summary>
        /// <param name="removalStrategy"></param>
        public void RemoveStatus(IStatusRemovalStrategy removalStrategy)
        {
            removalStrategy.HandleRemove(_statuses);
        }

        /// <summary>
        /// 更新状态加成
        /// </summary>
        private void UpdateStatusBonus()
        {
            // TODO：待优化，不需要每次都计算，而是变化的时候在计算
            statusTotalBonus.UpdateTotalAtkBonus(_statuses);
            statusTotalBonus.UpdateTotalDefBonus(_statuses);
            statusTotalBonus.UpdateTotalHpBonus(_statuses);
        }

        /// <summary>
        /// 叠加类型处理
        /// </summary>
        /// <param name="newStatus"></param>
        private void OnConflict_Add(IStatus newStatus)
        {
            // 先判断是否存在该ID的状态
            if (this.TryGetStatus(newStatus.StatusProperty.StatusInfo.f_id, _statuses, out IStatus status))
            {
                // 叠加层数，暂时写死，可以配置单位叠加层数
                status.ChangePine(1);
            }
            else
            {
                newStatus.IsValid = true;
                _statuses.Add(newStatus);
            }
        }

        /// <summary>
        /// 独立类型处理
        /// </summary>
        /// <param name="newStatus"></param>
        private void OnConflict_Lonel(IStatus newStatus)
        {
            newStatus.IsValid = true;
            _statuses.Add(newStatus);
        }

        /// <summary>
        /// 覆盖类型处理
        /// </summary>
        /// <param name="newStatus"></param>
        private void OnConflict_Cover(IStatus newStatus)
        {
            // 先判断是否存在该ID的状态
            if (this.TryGetStatus(newStatus.StatusProperty.StatusInfo.f_id, _statuses, out IStatus status))
            {
                status.IsValid = false;
                _statuses.Remove(status);
                newStatus.IsValid = true;
                _statuses.Add(newStatus);
            }
        }
    }
}
