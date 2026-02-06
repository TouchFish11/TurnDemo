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
            BattleEntity.Context.GetEventBus().TriggerEvent(new TurnStartEvent(BattleEntity.Context, BattleEntity));

            BattleEntity.ChangeState(EActPhase.Operator);
        }
        
        public override void Execute()
        {

        }

        public override void Exit()
        {

        }
    }
}
