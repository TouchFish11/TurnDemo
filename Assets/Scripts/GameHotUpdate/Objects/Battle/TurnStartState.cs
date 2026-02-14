using Game.Battle.Objects;
using GameHotUpdate.Battle.Event.Turn;

namespace GameHotUpdate.Objects.Battle
{
    public class TurnStartState : TurnState
    {
        public TurnStartState(IBattleEntityObject battleEntity) : base(battleEntity)
        {
            
        }

        public override void Enter()
        {
            // 触发回合开始事件
            PlayerObject.Context.GetEventBus().TriggerEvent(new TurnStartEvent(PlayerObject.Context, PlayerObject));

            PlayerObject.ChangeState(EActPhase.Operator);
        }
        

        public override void Exit()
        {

        }
    }
}
