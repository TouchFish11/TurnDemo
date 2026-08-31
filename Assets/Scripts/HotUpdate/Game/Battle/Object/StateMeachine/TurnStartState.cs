using System;
using System.Collections.Generic;
using Core.Log;
using HotUpdate.Game.Battle.Event.Turn;
using HotUpdate.Game.Battle.Object.Role;

namespace HotUpdate.Game.Battle.Object.StateMeachine
{
    /// <summary>
    /// 回合开始状态
    /// </summary>
    public class TurnStartState : TurnState
    {
        private readonly IReadOnlyList<ITurnStartNode> _startNodes;
        
        public TurnStartState(IBattleEntityObject battleEntity, IReadOnlyList<ITurnStartNode> startNodes) : base(battleEntity)
        {
            _startNodes = startNodes;
        }

        public override async void Enter()
        {
            try
            {
                foreach (var node in _startNodes)
                {
                    await node.Execute(BattleObject);
                }

                // 判断能否行动
                if (BattleObject.CanAct)
                {
                    // 触发回合开始事件
                    BattleObject.Context.EventBus.TriggerEvent(new TurnStartEvent(BattleObject.Context, BattleObject));
                    BattleObject.ChangeState(EActPhase.Executing);
                }
                else
                {
                    BattleObject.ChangeState(EActPhase.TurnEnd);
                }
            }
            catch (Exception e)
            {
                Logger.LogException(ELogTags.Battle, e);
            }
        }
        
        public override void Exit()
        {

        }
    }
}
