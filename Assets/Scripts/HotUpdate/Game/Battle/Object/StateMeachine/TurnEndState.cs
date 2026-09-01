using System.Threading.Tasks;
using HotUpdate.Game.Battle.Event.Turn;
using HotUpdate.Game.Battle.Statuses;

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

        public override Task Enter()
        {
            // 结算buff
            BattleObject.GetComponent<StatusComponent>().SettlementTurnEnd();
            // 触发回合结束事件（供外部监听）
            BattleObject.Context.EventBus.TriggerEvent(new TurnEndEvent(BattleObject.Context, BattleObject));
            return Task.CompletedTask;
        }
        
        public override Task Exit()
        {
            return Task.CompletedTask;
        }
    }
}
