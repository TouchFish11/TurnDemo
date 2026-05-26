using Core.DI;
using HotUpdate.Base;
using HotUpdate.Base.UI;
using HotUpdate.Game.Battle.Context;

namespace HotUpdate.Game.Battle.StateMeachine
{
    public abstract class BattleState : IBattleState
    {
        [Inject] protected IUIService uiService;
        
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
