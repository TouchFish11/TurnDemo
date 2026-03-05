using HotUpdate.Battle.Context;
using HotUpdate.Battle.Turn;

namespace HotUpdate.Battle.StateMeachine
{
    public abstract class BattleState : IBattleState
    {
        public IBattleStateMachine BattleStateMachine { get; private set; }
        
        public IBattleContext Context { get; private set; }
        
        protected BattleState(IBattleStateMachine battleStateMachine, IBattleContext context)
        {
            BattleStateMachine = battleStateMachine;
            Context = context;
        }
        
        public abstract void Enter();
        
        public abstract void Execute();
        
        public abstract void Exit();
        
        public virtual void Dispose()
        {
            BattleStateMachine = null;
            Context = null;
        }
    }
}
