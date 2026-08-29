using HotUpdate.Game.Battle.Event.Turn;
using HotUpdate.Game.Battle.Object.Role;

namespace HotUpdate.Game.Battle.Object.StateMeachine
{
    public class TurnStartState : TurnState
    {
        public TurnStartState(IBattleEntityObject battleEntity) : base(battleEntity)
        {
            
        }

        public override void Enter()
        {
            // 触发回合开始事件
            RoleObject.Context.EventBus.TriggerEvent(new TurnStartEvent(RoleObject.Context, RoleObject));
            RoleObject.ChangeState(EActPhase.Operator);
        }
        

        public override void Exit()
        {

        }
    }
}
