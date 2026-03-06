using Core.Service;
using Core.Time;
using HotUpdate.Battle.Context;
using HotUpdate.Battle.Event.Turn;
using HotUpdate.Battle.Turn;
using HotUpdate.Core.Battle;
using HotUpdate.Core.Battle.Turn;

namespace HotUpdate.Battle.StateMeachine
{
    public class BattleOverState : BattleState
    {
        public BattleOverState(IBattleStateMachine battleStateMachine, IBattleContext context) : base(battleStateMachine, context)
        {
            
        }

        public override void Enter()
        {
            BattleOver();
        }

        public override void Execute()
        {

        }
        
        /// <summary>
        /// 战斗结束
        /// </summary>
        private void BattleOver()
        {
            // 切换为正常倍速
            ServiceLocator.Get<ITimerManager>().SetTimeRate(E_TimeRate.Normal);
            // 触发战斗结束事件
            Context.GetEventBus().TriggerEvent(new BattleOverEvent(Context));
        }

        public override void Exit()
        {

        }
    }
}
