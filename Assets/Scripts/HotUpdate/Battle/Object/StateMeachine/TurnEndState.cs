using HotUpdate.Battle.Event.Turn;
using HotUpdate.Core.Battle;
using HotUpdate.Core.Battle.Object;

namespace HotUpdate.Battle.Object.StateMeachine
{
    /// <summary>
    /// 回合结束状态
    /// </summary>
    public class TurnEndState : TurnState
    {
        public TurnEndState(IBattleEntityObject battleEntity) : base(battleEntity)
        {
            
        }

        public override void Enter()
        {
            // 触发回合结束事件（供外部监听）
            PlayerObject.Context.GetEventBus().TriggerEvent(new TurnEndEvent(PlayerObject.Context, PlayerObject));
            Exit();
        }
        
        public override void Exit()
        {

        }
    }
}
