using Core.DI;
using Core.Time;
using HotUpdate.Game.Battle.Context;
using HotUpdate.Game.Battle.Event;

namespace HotUpdate.Game.Battle.StateMeachine
{
    /// <summary>
    /// 战斗结束状态
    /// </summary>
    public class BattleOverState : BattleState
    {
        [Inject] private ITimerManager _timerManager;
        
        public BattleOverState(IBattleStateMachine battleStateMachine, IBattleContext context) : base(battleStateMachine, context)
        {
            
        }

        public override void Enter()
        {
            BattleOver();
        }
        
        /// <summary>
        /// 战斗结束
        /// </summary>
        private void BattleOver()
        {
            // 切换为正常倍速
            _timerManager.SetTimeRate(E_TimeRate.Normal);
            // 触发战斗结束事件
            Context.GetEventBus().TriggerEvent(new BattleOverEvent(Context));
        }

        public override void Exit()
        {

        }

        protected override void OnDispose()
        {
            _timerManager = null;
        }
    }
}
