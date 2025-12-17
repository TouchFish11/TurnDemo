using Framework;
using System.Collections.Generic;

namespace Game.Battle
{
    /// <summary>
    /// 状态管理组件（角色的状态容器，负责管理所有状态）
    /// </summary>
    public class StatusComponent : BattleComponent, IStatusComponent
    {
        // 状态列表
        private readonly List<IStatus> _statuses = new List<IStatus>();

        public override void BattleInit(IBattleEntityObject battleEntity)
        {
            base.BattleInit(battleEntity);

            // 订阅“回合开始事件”（核心：模块主动订阅，无需核心流程修改）
            battleEntity.Context.GetEventBus().AddListener<TurnStartEvent>(OnTurnStartHandler);
        }

        /// <summary>
        /// 事件回调：接收回合开始通知，调用状态API
        /// </summary>
        /// <param name="evt"></param>
        private void OnTurnStartHandler(TurnStartEvent turnStartEvent)
        {
            // 只处理当前行动角色的状态（避免给其他角色触发）
            if (turnStartEvent.CurrentBattleObject != BattleEntity)
            {
                return;
            }

            // 遍历所有有效状态，调用其OnTurnStart API（执行具体逻辑）
            for (int i = 0; i < _statuses.Count; i++)
            {
                if (_statuses[i].IsValid)
                {
                    _statuses[i].OnTurnStart(BattleEntity, turnStartEvent.Context);
                }
            }

            // 移除失效状态
            _statuses.RemoveAll(s => !s.IsValid);
        }

        /// <summary>
        /// 对外提供API：添加状态（其他模块通过此API添加状态，而非直接操作列表）
        /// </summary>
        /// <param name="status"></param>
        public void AddStatus(IStatus status)
        {
            _statuses.Add(status);
            LogManager.Log($"{BattleEntity.Name}获得状态：{status.GetType().Name}");
        }
    }
}
