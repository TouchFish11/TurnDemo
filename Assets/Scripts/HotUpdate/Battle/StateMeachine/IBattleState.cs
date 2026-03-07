using HotUpdate.Core.Battle;
using HotUpdate.Core.Battle.Turn;

namespace HotUpdate.Battle.StateMeachine
{
    /// <summary>
    /// 战斗状态
    /// </summary>
    public interface IBattleState
    {
        public IBattleStateMachine BattleStateMachine { get; }
        
        public IBattleContext Context { get; }
        
        void Enter();
        
        void Execute();
        
        void Exit();

        void Dispose();
    }
}
