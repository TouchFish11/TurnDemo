using Core.DI;
using Core.Time;
using HotUpdate.Base.Battle;
using HotUpdate.Base.Battle.Event;
using HotUpdate.Base.Battle.Turn;

namespace HotUpdate.Game.Battle.StateMeachine
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
            DIContainer.GetInstance<ITimerManager>().SetTimeRate(E_TimeRate.Normal);
            // 触发战斗结束事件
            Context.GetEventBus().TriggerEvent(new BattleOverEvent(Context));
        }

        public override void Exit()
        {

        }
    }
}
