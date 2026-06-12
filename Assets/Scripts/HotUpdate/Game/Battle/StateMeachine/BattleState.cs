using Core.DI;
using HotUpdate.Base.UI;
using HotUpdate.Game.Battle.Context;

namespace HotUpdate.Game.Battle.StateMeachine
{
    /// <summary>
    /// 战斗状态基类
    /// </summary>
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
        
        public abstract void Exit();
        
        public void Dispose()
        {
            OnDispose();
            uiService = null;
            BattleStateMachine = null;
            Context = null;
        }
        
        protected abstract void OnDispose();
    }
}
