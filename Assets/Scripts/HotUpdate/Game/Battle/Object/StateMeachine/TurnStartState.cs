using System.Threading.Tasks;
using HotUpdate.Game.Battle.Event.Turn;
using HotUpdate.Game.Battle.Object.Role;

namespace HotUpdate.Game.Battle.Object.StateMeachine
{
    /// <summary>
    /// 回合开始状态
    /// </summary>
    public class TurnStartState : TurnState
    {
        private readonly ITurnStartNode _startNode;
        
        public TurnStartState(IBattleEntityObject battleEntity, ITurnStartNode turnStartNode) : base(battleEntity)
        {
            _startNode =  turnStartNode;
        }

        public override async Task Enter()
        {
            await _startNode.Execute(BattleObject);

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
        
        public override Task Exit()
        {
            return Task.CompletedTask;
        }
    }
}
