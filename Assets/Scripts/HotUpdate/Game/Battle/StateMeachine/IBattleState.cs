using HotUpdate.Base;
using HotUpdate.Game.Battle.Context;
using HotUpdate.Game.Battle.Turn;

namespace HotUpdate.Game.Battle.StateMeachine
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
