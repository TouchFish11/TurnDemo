using HotUpdate.Game.Battle.Event.Turn;
using HotUpdate.Game.Battle.Object.Role;

namespace HotUpdate.Game.Battle.Object.StateMeachine
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
        }
        
        public override void Exit()
        {
            // 重置角色行动状态
            PlayerObject.CurrentActPhase = EActPhase.SettlementBuff;
        }
    }
}
